﻿// <copyright file="DefaultApplication.AccountCreationActions.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Impl.Application
{
    internal sealed partial class DefaultApplication
    {
        Task<IAccount> IAccountCreationActions.CreateAccountAsync(IAccount account, IAccountCreationOptions creationOptions, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, account, creationOptions, cancellationToken);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(IAccount account, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, account, cancellationToken);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(string givenName, string surname, string email, string password, object customData, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, givenName, surname, email, password, customData, cancellationToken);

        Task<IAccount> IAccountCreationActions.CreateAccountAsync(string givenName, string surname, string email, string password, CancellationToken cancellationToken)
            => AccountCreationActionsShared.CreateAccountAsync(this.GetInternalAsyncDataStore(), this.Accounts.Href, givenName, surname, email, password, null, cancellationToken);
    }
}
