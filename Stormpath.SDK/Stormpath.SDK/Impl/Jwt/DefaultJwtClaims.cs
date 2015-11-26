﻿// <copyright file="DefaultJwtClaims.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Jwt;

namespace Stormpath.SDK.Impl.Jwt
{
    internal sealed class DefaultJwtClaims : IJwtClaims
    {
        public static readonly string Issuer = "iss";
        public static readonly string Subject = "sub";
        public static readonly string Audience = "aud";
        public static readonly string Expiration = "exp";
        public static readonly string NotBefore = "nbf";
        public static readonly string IssuedAt = "iat";
        public static readonly string Id = "jti";

        private readonly IDictionary<string, object> claims;

        public DefaultJwtClaims(IDictionary<string, object> claims)
        {
            this.claims = claims;
        }

        private object GetClaim(string claimName)
        {
            object value = null;
            this.claims.TryGetValue(claimName, out value);

            return value;
        }

        private string GetStringClaim(string claimName)
        {
            return (string)this.GetClaim(claimName);
        }

        private DateTimeOffset? GetDateClaim(string claimName)
        {
            var unixTimestamp = (long?)this.GetClaim(claimName);

            return UnixDate.FromLong(unixTimestamp);
        }

        string IJwtClaims.Audience => this.GetStringClaim(Audience);

        DateTimeOffset? IJwtClaims.Expiration => this.GetDateClaim(Expiration);

        string IJwtClaims.Id => this.GetStringClaim(Id);

        DateTimeOffset? IJwtClaims.IssuedAt => this.GetDateClaim(IssuedAt);

        string IJwtClaims.Issuer => this.GetStringClaim(Issuer);

        DateTimeOffset? IJwtClaims.NotBefore => this.GetDateClaim(NotBefore);

        string IJwtClaims.Subject => this.GetStringClaim(Subject);

        IDictionary<string, object> IJwtClaims.ToDictionary()
            => new Dictionary<string, object>(this.claims);
    }
}
