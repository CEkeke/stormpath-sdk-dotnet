﻿// <copyright file="ProviderAccountResultFilter.cs" company="Stormpath, Inc.">
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

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Provider;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed class ProviderAccountResultFilter : IAsynchronousFilter, ISynchronousFilter
    {
        IResourceDataResult ISynchronousFilter.Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            var result = chain.Filter(request, logger);

            return FilterCore(result);
        }

        async Task<IResourceDataResult> IAsynchronousFilter.FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
        {
            var result = await chain.FilterAsync(request, logger, cancellationToken).ConfigureAwait(false);

            return FilterCore(result);
        }

        private static IResourceDataResult FilterCore(IResourceDataResult result)
        {
            if (typeof(IProviderAccountResult).GetTypeInfo().IsAssignableFrom(result.Type.GetTypeInfo()))
            {
                result.Body.Add("isNewAccount", result.HttpStatus == 201);
            }

            return result;
        }
    }
}
