﻿// <copyright file="DefaultClient.IDataStore.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.DataStore;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Sync;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed partial class DefaultClient
    {
        T IDataStore.Instantiate<T>() => this.dataStore.Instantiate<T>();

        Task<T> IDataStore.GetResourceAsync<T>(string href, CancellationToken cancellationToken)
            => this.dataStoreAsync.GetResourceAsync<T>(href, cancellationToken);

        Task<T> IDataStore.GetResourceAsync<T>(string href, Action<SDK.Resource.IRetrievalOptions<T>> options, CancellationToken cancellationToken)
            => this.dataStoreAsync.GetResourceAsync(href, options, cancellationToken);

        T IDataStoreSync.GetResource<T>(string href)
            => this.dataStoreSync.GetResource<T>(href);

        T IDataStoreSync.GetResource<T>(string href, Action<SDK.Resource.IRetrievalOptions<T>> options)
            => this.dataStoreAsync.GetResource(href, options);
    }
}
