﻿// <copyright file="DefaultIdSiteSyncCallbackHandler.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Jwt;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class DefaultIdSiteSyncCallbackHandler : IIdSiteSyncCallbackHandler
    {
        private readonly IInternalDataStore internalDataStore;
        private readonly string jwtResponse;

        private INonceStore nonceStore;
        private ISynchronousNonceStore syncNonceStore;

        private IIdSiteResultSyncListener resultListener;

        public DefaultIdSiteSyncCallbackHandler(IInternalDataStore internalDataStore, IHttpRequest httpRequest)
        {
            if (internalDataStore == null)
                throw new ArgumentNullException(nameof(internalDataStore));
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            this.internalDataStore = internalDataStore;
            this.jwtResponse = GetJwtResponse(httpRequest);

            this.nonceStore = new DefaultNonceStore(internalDataStore.CacheResolver);
            this.syncNonceStore = this.nonceStore as ISynchronousNonceStore;
        }

        private static string GetJwtResponse(IHttpRequest request)
        {
            if (request.Method != HttpMethod.Get)
                throw new ApplicationException("Only HTTP GET method is supported.");

            var jwtResponse = request.CanonicalUri.QueryString[IdSiteClaims.JwtResponse];

            if (string.IsNullOrEmpty(jwtResponse))
                throw InvalidJwtException.JwtRequired;

            return jwtResponse;
        }

        IIdSiteSyncCallbackHandler IIdSiteSyncCallbackHandler.SetNonceStore(INonceStore nonceStore)
        {
            if (nonceStore == null)
                throw new ArgumentNullException(nameof(nonceStore));

            this.nonceStore = nonceStore;

            return this;
        }

        IIdSiteSyncCallbackHandler IIdSiteSyncCallbackHandler.SetResultListener(IIdSiteResultSyncListener resultListener)
        {
            this.resultListener = resultListener;

            return this;
        }

        IAccountResult IIdSiteSyncCallbackHandler.GetAccountResult()
        {
            var dataStoreApiKey = this.internalDataStore.ApiKey;

            var jwt = JsonWebToken.Decode(this.jwtResponse, this.internalDataStore.Serializer);

            ThrowIfRequiredParametersMissing(jwt.Payload);

            string apiKeyFromJwt = null;
            if (IsError(jwt.Payload))
                jwt.Header.TryGetValueAsString(JwtHeaderParameters.KeyId, out apiKeyFromJwt);
            else
                jwt.Payload.TryGetValueAsString(DefaultJwtClaims.Audience, out apiKeyFromJwt);

            ThrowIfJwtSignatureInvalid(apiKeyFromJwt, dataStoreApiKey, jwt);
            ThrowIfJwtIsExpired(jwt.Payload);

            IfErrorThrowIdSiteException(jwt.Payload);

            if (!this.nonceStore.IsAsynchronousSupported || this.syncNonceStore == null)
                throw new ApplicationException("The current nonce store does not support synchronous operations.");

            var responseNonce = (string)jwt.Payload[IdSiteClaims.ResponseId];
            this.ThrowIfNonceIsAlreadyUsed(responseNonce);
            this.syncNonceStore.PutNonce(responseNonce);

            ThrowIfSubjectIsMissing(jwt.Payload);

            var accountResult = this.CreateAccountResult(jwt.Payload);
            var resultStatus = IdSiteResultStatus.Parse((string)jwt.Payload[IdSiteClaims.Status]);

            if (this.resultListener != null)
                this.DispatchResponseStatus(resultStatus, accountResult);

            return accountResult;
        }

        private void DispatchResponseStatus(IdSiteResultStatus status, IAccountResult accountResult)
        {
            if (status == IdSiteResultStatus.Registered)
            {
                this.resultListener.OnRegistered(accountResult);
                return;
            }
            else if (status == IdSiteResultStatus.Authenticated)
            {
                this.resultListener.OnAuthenticated(accountResult);
                return;
            }
            else if (status == IdSiteResultStatus.Logout)
            {
                this.resultListener.OnLogout(accountResult);
                return;
            }

            throw new ArgumentException($"Encountered unknown ID Site result status: {status}");
        }

        private static bool IsError(Map payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            object error = null;

            return payload.TryGetValue(IdSiteClaims.Error, out error)
                && error != null;
        }

        private static string GetAccountHref(Map payload)
        {
            string accountHref = null;
            payload.TryGetValueAsString(DefaultJwtClaims.Subject, out accountHref);

            return accountHref;
        }

        private static void ThrowIfRequiredParametersMissing(Map payload)
        {
            var requiredKeys = new string[]
            {
                DefaultJwtClaims.Id,
                DefaultJwtClaims.Audience,
                DefaultJwtClaims.Expiration,
                DefaultJwtClaims.Issuer,
                IdSiteClaims.ResponseId,
                IdSiteClaims.Status,
                IdSiteClaims.IsNewSubject,
            };

            bool isError = IsError(payload);
            bool valid = requiredKeys?.All(x => payload.ContainsKey(x)) ?? false;
            if (!isError && !valid)
                throw InvalidJwtException.ResponseMissingParameter;
        }

        private static void ThrowIfJwtSignatureInvalid(string jwtApiKey, IClientApiKey clientApiKey, JsonWebToken jwt)
        {
            if (!clientApiKey.GetId().Equals(jwtApiKey, StringComparison.InvariantCultureIgnoreCase))
                throw InvalidJwtException.ResponseInvalidApiKeyId;

            if (!new JwtSignatureValidator(clientApiKey).IsValid(jwt))
                throw InvalidJwtException.SignatureError;
        }

        private static void ThrowIfJwtIsExpired(Map payload)
        {
            var expiration = Convert.ToInt64(payload[DefaultJwtClaims.Expiration]);
            var now = UnixDate.ToLong(DateTimeOffset.Now);

            if (now > expiration)
                throw InvalidJwtException.Expired;
        }

        private static void IfErrorThrowIdSiteException(Map payload)
        {
            if (!IsError(payload))
                return;

            var errorData = payload[IdSiteClaims.Error] as Map;
            if (errorData == null)
                throw new ApplicationException("Error parsing ID Site error response.");

            object codeRaw;
            int code;
            if (!errorData.TryGetValue("code", out codeRaw) ||
                !int.TryParse(codeRaw.ToString(), out code))
                throw new ApplicationException($"Error type is unrecognized: '{codeRaw ?? "<null>"}'");

            if (code == 10011
                || code == 10012
                || code == 11001
                || code == 11002
                || code == 11003)
            {
                throw new InvalidIdSiteTokenException(new Error.DefaultError(errorData));
            }

            if (code == 12001)
            {
                throw new IdSiteSessionTimeoutException(new Error.DefaultError(errorData));
            }

            // Default/fallback
            throw new IdSiteRuntimeException(new Error.DefaultError(errorData));
        }

        private void ThrowIfNonceIsAlreadyUsed(string nonce)
        {
            bool alreadyUsed = this.syncNonceStore.ContainsNonce(nonce);
            if (alreadyUsed)
                throw InvalidJwtException.AlreadyUsed;
        }

        private static void ThrowIfSubjectIsMissing(Map payload)
        {
            var sub = GetAccountHref(payload);
            bool subMissing = string.IsNullOrEmpty(sub);
            var resultStatus = IdSiteResultStatus.Parse((string)payload[IdSiteClaims.Status]);

            // The 'sub' claim (accountHref) can be null if calling /sso/logout when the subject is already logged out,
            // but this is only legal during the logout scenario, so assert:
            if (subMissing && resultStatus != IdSiteResultStatus.Logout)
                throw InvalidJwtException.ResponseMissingParameter;
        }

        private IAccountResult CreateAccountResult(Map payload)
        {
            object state = null;
            payload.TryGetValue(IdSiteClaims.State, out state);
            bool isNewAccount = (bool)payload[IdSiteClaims.IsNewSubject];

            var properties = new Dictionary<string, object>()
            {
                [DefaultAccountResult.NewAccountPropertyName] = isNewAccount,
                [DefaultAccountResult.StatePropertyName] = state
            };

            var accountHref = GetAccountHref(payload);
            if (!string.IsNullOrEmpty(accountHref))
                properties[DefaultAccountResult.AccountPropertyName] = new LinkProperty(accountHref);

            return this.internalDataStore.InstantiateWithData<IAccountResult>(properties);
        }
    }
}
