﻿// <copyright file="Entity_tests.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Tests.Integration.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Integration.Async
{
    [Collection("Live tenant tests")]
    public class Entity_tests
    {
        private readonly IntegrationTestFixture fixture;

        public Entity_tests(IntegrationTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Multiple_instances_reference_same_data(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var account = await application.GetAccounts().FirstAsync();
            var anotherAccount = await application.GetAccounts().FirstAsync();

            var updatedEmail = account.Email + "-foobar";
            account.SetEmail(updatedEmail);
            anotherAccount.Email.ShouldBe(updatedEmail);
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Reference_is_updated_after_saving(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var newAccount = client.Instantiate<IAccount>();
            newAccount.SetEmail("identity-maps-are-useful@test.foo");
            newAccount.SetPassword("Changeme123!");
            newAccount.SetGivenName("Testing");
            newAccount.SetSurname("IdentityMaps");

            var created = await application.CreateAccountAsync(newAccount, opt => opt.RegistrationWorkflowEnabled = false);
            this.fixture.CreatedAccountHrefs.Add(created.Href);

            created.SetMiddleName("these");
            var updated = await created.SaveAsync();

            updated.SetEmail("different");
            created.Email.ShouldBe("different");

            await updated.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Original_object_is_updated_after_creating(TestClientBuilder clientBuilder)
        {
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var newAccount = client.Instantiate<IAccount>();
            newAccount.SetEmail("super-smart-objects@test.foo");
            newAccount.SetPassword("Changeme123!");
            newAccount.SetGivenName("Testing");
            newAccount.SetSurname("InitialProxy");

            var created = await application.CreateAccountAsync(newAccount, opt => opt.RegistrationWorkflowEnabled = false);
            this.fixture.CreatedAccountHrefs.Add(created.Href);

            created.SetMiddleName("these");
            newAccount.MiddleName.ShouldBe("these");

            await created.DeleteAsync();
        }

        [Theory]
        [MemberData(nameof(IntegrationTestClients.GetClients), MemberType = typeof(IntegrationTestClients))]
        public async Task Not_capturing_save_result_works(TestClientBuilder clientBuilder)
        {
            // This test is a little redundant, but explicitly tests a style
            // that will be common among consumers of the SDK.
            var client = clientBuilder.Build();
            var application = await client.GetResourceAsync<IApplication>(this.fixture.PrimaryApplicationHref);

            var newAccount = client.Instantiate<IAccount>();
            newAccount.SetEmail("indistinguishable-from-magic@test.foo");
            newAccount.SetPassword("Changeme123!");
            newAccount.SetGivenName("Testing");
            newAccount.SetSurname("InitialProxy-NonCaptureWorkflow");

            newAccount.Href.ShouldBeNullOrEmpty();

            // Instead of capturing result = ...
            // Just execute the method and expect the original object to be updated
            await application.CreateAccountAsync(newAccount, opt => opt.RegistrationWorkflowEnabled = false);
            newAccount.Href.ShouldNotBeNullOrEmpty();
            this.fixture.CreatedAccountHrefs.Add(newAccount.Href);

            await newAccount.DeleteAsync();
        }
    }
}