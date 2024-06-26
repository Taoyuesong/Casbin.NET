﻿using System.Threading.Tasks;
using Casbin.UnitTests.Fixtures;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public class RbacApiWithDomainsTest
{
    private readonly TestModelFixture TestModelFixture;

    public RbacApiWithDomainsTest(TestModelFixture testModelFixture) => TestModelFixture = testModelFixture;

    [Fact]
    public void TestGetDomainsForUser()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacWithDomainsModelText,
            TestModelFixture.RbacWithDomainsPolicy2Text));

        e.BuildRoleLinks();

        e.TestGetDomainsForUser("alice", new[] { "domain1", "domain2" });
        e.TestGetDomainsForUser("bob", new[] { "domain2", "domain3" });
        e.TestGetDomainsForUser("user", new[] { "domain3" });
    }

    [Fact]
    public void TestGetRolesFromUserWithDomains()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacWithDomainsModelText,
            TestModelFixture.RbacWithHierarchyWithDomainsPolicyText));

        e.BuildRoleLinks();

        // This is only able to retrieve the first level of roles.
        TestGetRolesInDomain(e, "alice", "domain1", AsList("role:global_admin"));

        // Retrieve all inherit roles. It supports domains as well.
        TestGetImplicitRolesInDomain(e, "alice", "domain1", AsList("role:global_admin", "role:reader", "role:writer"));
    }

    [Fact]
    public void TestRoleApiWithDomains()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
        e.BuildRoleLinks();

        TestGetRolesInDomain(e, "alice", "domain1", AsList("admin"));
        TestGetRolesInDomain(e, "bob", "domain1", AsList());
        TestGetRolesInDomain(e, "admin", "domain1", AsList());
        TestGetRolesInDomain(e, "non_exist", "domain1", AsList());

        TestGetRolesInDomain(e, "alice", "domain2", AsList());
        TestGetRolesInDomain(e, "bob", "domain2", AsList("admin"));
        TestGetRolesInDomain(e, "admin", "domain2", AsList());
        TestGetRolesInDomain(e, "non_exist", "domain2", AsList());

        e.DeleteRoleForUserInDomain("alice", "admin", "domain1");
        e.AddRoleForUserInDomain("bob", "admin", "domain1");

        TestGetRolesInDomain(e, "alice", "domain1", AsList());
        TestGetRolesInDomain(e, "bob", "domain1", AsList("admin"));
        TestGetRolesInDomain(e, "admin", "domain1", AsList());
        TestGetRolesInDomain(e, "non_exist", "domain1", AsList());

        TestGetRolesInDomain(e, "alice", "domain2", AsList());
        TestGetRolesInDomain(e, "bob", "domain2", AsList("admin"));
        TestGetRolesInDomain(e, "admin", "domain2", AsList());
        TestGetRolesInDomain(e, "non_exist", "domain2", AsList());
    }

    [Fact]
    public async Task TestRoleApiWithDomainsAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
        e.BuildRoleLinks();

        TestGetRolesInDomain(e, "alice", "domain1", AsList("admin"));
        TestGetRolesInDomain(e, "bob", "domain1", AsList());
        TestGetRolesInDomain(e, "admin", "domain1", AsList());
        TestGetRolesInDomain(e, "non_exist", "domain1", AsList());

        TestGetRolesInDomain(e, "alice", "domain2", AsList());
        TestGetRolesInDomain(e, "bob", "domain2", AsList("admin"));
        TestGetRolesInDomain(e, "admin", "domain2", AsList());
        TestGetRolesInDomain(e, "non_exist", "domain2", AsList());

        await e.DeleteRoleForUserInDomainAsync("alice", "admin", "domain1");
        await e.AddRoleForUserInDomainAsync("bob", "admin", "domain1");

        TestGetRolesInDomain(e, "alice", "domain1", AsList());
        TestGetRolesInDomain(e, "bob", "domain1", AsList("admin"));
        TestGetRolesInDomain(e, "admin", "domain1", AsList());
        TestGetRolesInDomain(e, "non_exist", "domain1", AsList());

        TestGetRolesInDomain(e, "alice", "domain2", AsList());
        TestGetRolesInDomain(e, "bob", "domain2", AsList("admin"));
        TestGetRolesInDomain(e, "admin", "domain2", AsList());
        TestGetRolesInDomain(e, "non_exist", "domain2", AsList());
    }
}
