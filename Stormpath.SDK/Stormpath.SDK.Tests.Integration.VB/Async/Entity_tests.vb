' <copyright file="Entity_tests.vb" company="Stormpath, Inc.">
' Copyright (c) 2015 Stormpath, Inc.
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'      http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
' </copyright>

Option Strict On
Option Explicit On
Option Infer On
Imports Shouldly
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Application
Imports Stormpath.SDK.Tests.Common.Integration
Imports Xunit

Namespace Stormpath.SDK.Tests.Integration.VB.Async
    <Collection(NameOf(IntegrationTestCollection))>
    Public Class Entity_tests
        Private ReadOnly fixture As TestFixture

        Public Sub New(fixture As TestFixture)
            Me.fixture = fixture
        End Sub

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Multiple_instances_reference_same_data(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim application = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim account = Await application.GetAccounts().FirstAsync()
            Dim anotherAccount = Await application.GetAccounts().FirstAsync()

            Dim updatedEmail = account.Email + "-foobar"
            account.SetEmail(updatedEmail)
            anotherAccount.Email.ShouldBe(updatedEmail)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Reference_is_updated_after_saving(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim application = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim newAccount = client.Instantiate(Of IAccount)()
            newAccount.SetEmail("identity-maps-are-useful@test.foo")
            newAccount.SetPassword("Changeme123!")
            newAccount.SetGivenName("Testing")
            newAccount.SetSurname("IdentityMaps")

            Dim created = Await application.CreateAccountAsync(newAccount, Function(opt) InlineAssignHelper(opt.RegistrationWorkflowEnabled, False))
            Me.fixture.CreatedAccountHrefs.Add(created.Href)

            created.SetMiddleName("these")
            Dim updated = Await created.SaveAsync()

            updated.SetEmail("different")
            created.Email.ShouldBe("different")

            Assert.True(Await updated.DeleteAsync())
            Me.fixture.CreatedAccountHrefs.Remove(updated.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Original_object_is_updated_after_creating(clientBuilder As TestClientProvider) As Task
            Dim client = clientBuilder.GetClient()
            Dim application = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim newAccount = client.Instantiate(Of IAccount)()
            newAccount.SetEmail("super-smart-objects@test.foo")
            newAccount.SetPassword("Changeme123!")
            newAccount.SetGivenName("Testing")
            newAccount.SetSurname("InitialProxy")

            Dim created = Await application.CreateAccountAsync(newAccount, Function(opt) InlineAssignHelper(opt.RegistrationWorkflowEnabled, False))
            Me.fixture.CreatedAccountHrefs.Add(created.Href)

            created.SetMiddleName("these")
            newAccount.MiddleName.ShouldBe("these")

            Assert.True(Await created.DeleteAsync())
            Me.fixture.CreatedAccountHrefs.Remove(created.Href)
        End Function

        <Theory>
        <MemberData(NameOf(TestClients.GetClients), MemberType:=GetType(TestClients))>
        Public Async Function Not_capturing_save_result_works(clientBuilder As TestClientProvider) As Task
            ' This test is a little redundant, but explicitly tests a style
            ' that will be common among consumers of the SDK.
            Dim client = clientBuilder.GetClient()
            Dim application = Await client.GetResourceAsync(Of IApplication)(Me.fixture.PrimaryApplicationHref)

            Dim newAccount = client.Instantiate(Of IAccount)()
            newAccount.SetEmail("indistinguishable-from-magic@test.foo")
            newAccount.SetPassword("Changeme123!")
            newAccount.SetGivenName("Testing")
            newAccount.SetSurname("InitialProxy-NonCaptureWorkflow")

            newAccount.Href.ShouldBeNullOrEmpty()

            ' Instead of capturing result = ...
            ' Just execute the method and expect the original object to be updated
            Await application.CreateAccountAsync(newAccount, Function(opt) InlineAssignHelper(opt.RegistrationWorkflowEnabled, False))
            newAccount.Href.ShouldNotBeNullOrEmpty()
            Me.fixture.CreatedAccountHrefs.Add(newAccount.Href)

            Assert.True(Await newAccount.DeleteAsync())
            Me.fixture.CreatedAccountHrefs.Remove(newAccount.Href)
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace