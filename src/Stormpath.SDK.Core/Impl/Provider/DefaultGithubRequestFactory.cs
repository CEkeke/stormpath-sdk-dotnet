﻿// <copyright file="DefaultGithubRequestFactory.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal sealed class DefaultGithubRequestFactory : IGithubRequestFactory
    {
        private readonly IInternalDataStore dataStore;

        public DefaultGithubRequestFactory(IInternalDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        IGithubAccountRequestBuilder IProviderRequestFactory<IGithubAccountRequestBuilder, IGithubCreateProviderRequestBuilder>.Account()
            => new DefaultGithubAccountRequestBuilder(this.dataStore);

        IGithubCreateProviderRequestBuilder IProviderRequestFactory<IGithubAccountRequestBuilder, IGithubCreateProviderRequestBuilder>.Builder()
            => new DefaultGithubCreateProviderRequestBuilder(this.dataStore);
    }
}
