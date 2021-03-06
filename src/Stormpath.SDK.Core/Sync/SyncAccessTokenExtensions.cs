﻿// <copyright file="SyncAccessTokenExtensions.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Oauth;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on <see cref="IAccessToken"/>.
    /// </summary>
    public static class SyncAccessTokenExtensions
    {
        /// <summary>
        /// Synchronously retrieves the <see cref="IAccount">Account</see> associated with this <see cref="IAccessToken">AccessToken</see>.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The <see cref="IAccount">Account</see> associated with this <see cref="IAccessToken">AccessToken</see>.</returns>
        public static IAccount GetAccount(this IAccessToken accessToken)
            => (accessToken as IAccessTokenSync).GetAccount();

        /// <summary>
        /// Synchronously retrieves the <see cref="IAccount">Account</see> associated with this <see cref="IAccessToken">AccessToken</see>
        /// with the specified retrieval options.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="retrievalOptions">The retrieval options.</param>
        /// <returns>The <see cref="IAccount">Account</see> associated with this <see cref="IAccessToken">AccessToken</see>.</returns>
        public static IAccount GetAccount(this IAccessToken accessToken, Action<IRetrievalOptions<IAccount>> retrievalOptions)
            => (accessToken as IAccessTokenSync).GetAccount(retrievalOptions);

        /// <summary>
        /// Synchronously retrieves the <see cref="IApplication">Application</see> associated with this <see cref="IAccessToken">AccessToken</see>.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The <see cref="IApplication">Application</see> associated with this <see cref="IAccessToken">AccessToken</see>.</returns>
        public static IApplication GetApplication(this IAccessToken accessToken)
            => (accessToken as IAccessTokenSync).GetApplication();

        /// <summary>
        /// Synchronously retrieves the <see cref="IApplication">Application</see> associated with this <see cref="IAccessToken">AccessToken</see>
        /// with the specified retrieval options.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="retrievalOptions">The retrieval options.</param>
        /// <returns>The <see cref="IApplication">Application</see> associated with this <see cref="IAccessToken">AccessToken</see>.</returns>
        public static IApplication GetApplication(this IAccessToken accessToken, Action<IRetrievalOptions<IApplication>> retrievalOptions)
            => (accessToken as IAccessTokenSync).GetApplication(retrievalOptions);
    }
}
