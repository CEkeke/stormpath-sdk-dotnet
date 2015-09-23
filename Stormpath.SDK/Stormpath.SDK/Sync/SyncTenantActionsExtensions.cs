﻿// <copyright file="SyncTenantActionsExtensions.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Tenant;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Sync
{
    public static class SyncTenantActionsExtensions
    {
        /// <summary>
        /// Synchronously creates a new <see cref="IApplication"/> resource in the current tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <param name="creationOptionsAction">An inline builder for an instance of <see cref="ApplicationCreationOptionsBuilder"/>,
        /// which will be used when sending the request.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        public static IApplication CreateApplication(this ITenantActions tenantActions, IApplication application, Action<ApplicationCreationOptionsBuilder> creationOptionsAction)
            => (tenantActions as ITenantActionsSync).CreateApplication(application, creationOptionsAction);

        /// <summary>
        /// Synchronously creates a new <see cref="IApplication"/> resource in the current tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <param name="creationOptions">An <see cref="IApplicationCreationOptions"/> instance to use when sending the request.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        public static IApplication CreateApplication(this ITenantActions tenantActions, IApplication application, IApplicationCreationOptions creationOptions)
            => (tenantActions as ITenantActionsSync).CreateApplication(application, creationOptions);

        /// <summary>
        /// Synchronously creates a new <see cref="IApplication"/> resource in the current tenant, with the default creation options.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="application">The <see cref="IApplication"/> to create.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        public static IApplication CreateApplication(this ITenantActions tenantActions, IApplication application)
            => (tenantActions as ITenantActionsSync).CreateApplication(application);

        /// <summary>
        /// Synchronously creates a new <see cref="IApplication"/> resource in the current tenant.
        /// </summary>
        /// <param name="tenantActions">The object supporting the <see cref="ITenantActions"/> interface.</param>
        /// <param name="name">The name of the application.</param>
        /// <param name="createDirectory">Whether a default directory should be created automatically.</param>
        /// <returns>The created <see cref="IApplication"/>.</returns>
        /// <exception cref="Error.ResourceException">There was a problem creating the application.</exception>
        public static IApplication CreateApplication(this ITenantActions tenantActions, string name, bool createDirectory)
            => (tenantActions as ITenantActionsSync).CreateApplication(name, createDirectory);
    }
}