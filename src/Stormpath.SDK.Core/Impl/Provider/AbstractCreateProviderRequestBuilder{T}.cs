﻿// <copyright file="AbstractCreateProviderRequestBuilder{T}.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.Provider
{
    internal abstract class AbstractCreateProviderRequestBuilder<T> : ICreateProviderRequestBuilder<T>
        where T : class, ICreateProviderRequestBuilder<T>
    {
        protected readonly IInternalDataStore dataStore;

        protected string clientId;
        protected string clientSecret;

        public AbstractCreateProviderRequestBuilder(IInternalDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        T ICreateProviderRequestBuilder<T>.SetClientId(string clientId)
        {
            this.clientId = clientId;
            return this as T;
        }

        T ICreateProviderRequestBuilder<T>.SetClientSecret(string clientSecret)
        {
            this.clientSecret = clientSecret;
            return this as T;
        }

        ICreateProviderRequest ICreateProviderRequestBuilder<T>.Build()
        {
            if (string.IsNullOrEmpty(this.clientId))
            {
                throw new Exception($"{nameof(this.clientId)} is a required property. It must be provided before building.");
            }

            if (string.IsNullOrEmpty(this.clientSecret))
            {
                throw new Exception($"{nameof(this.clientSecret)} is a required property. It must be provided before building.");
            }

            return this.BuildConcrete();
        }

        protected abstract string ConcreteProviderId { get; }

        protected abstract ICreateProviderRequest BuildConcrete();
    }
}
