﻿// <copyright file="MethodNameTranslator.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Linq.Parsing.Translators
{
    internal class MethodNameTranslator : AbstractNameTranslator
    {
        private static Dictionary<string, string> methodNameMap = new Dictionary<string, string>()
        {
            ["GetDirectoryAsync"] = "directory",
            ["GetDirectory"] = "directory",
            ["GetTenantAsync"] = "tenant",
            ["GetTenant"] = "tenant",
            ["GetCustomDataAsync"] = "customData",
            ["GetCustomData"] = "customData",
            ["GetProviderDataAsync"] = "providerData",
            ["GetProviderData"] = "providerData",
            ["GetDefaultAccountStoreAsync"] = "defaultAccountStoreMapping",
            ["GetDefaultAccountStore"] = "defaultAccountStoreMapping",
            ["GetDefaultGroupStoreAsync"] = "defaultGroupStoreMapping",
            ["GetDefaultGroupStore"] = "defaultGroupStoreMapping",
            ["GetAccountStoreAsync"] = "accountStore",
            ["GetAccountStore"] = "accountStore",
            ["GetProviderAsync"] = "provider",
            ["GetProvider"] = "provider",
            ["GetAccountAsync"] = "account",
            ["GetAccount"] = "account",
            ["GetGroupAsync"] = "group",
            ["GetGroup"] = "group",
            ["GetApplicationAsync"] = "application",
            ["GetApplication"] = "application",

            ["GetGroups"] = "groups",
            ["GetGroupMemberships"] = "groupMemberships",
            ["GetAccountMemberships"] = "accountMemberships",
            ["GetAccounts"] = "accounts",
            ["GetAccountStoreMappings"] = "accountStoreMappings",
            ["GetApplications"] = "applications",
            ["GetDirectories"] = "directories",
        };

        public MethodNameTranslator()
            : base(methodNameMap)
        {
        }
    }
}