﻿using Casbin.UnitTests.Fixtures;
using Casbin.UnitTests.Mock;
using Xunit;
using Xunit.Abstractions;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public class CachedEnforcerTest
{
    private readonly TestModelFixture TestModelFixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public CachedEnforcerTest(ITestOutputHelper testOutputHelper, TestModelFixture testModelFixture)
    {
        _testOutputHelper = testOutputHelper;
        TestModelFixture = testModelFixture;
    }

    [Fact]
    public void TestEnforceWithCache()
    {
#if !NET452
        Enforcer e = new(TestModelFixture.GetBasicTestModel())
        {
            Logger = new MockLogger<Enforcer>(_testOutputHelper)
        };
#else
            var e = new Enforcer(TestModelFixture.GetBasicTestModel());
#endif
        e.EnableCache(true);
        e.EnableAutoCleanEnforceCache(false);

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);

        // The cache is enabled, so even if we remove a policy rule, the decision
        // for ("alice", "data1", "read") will still be true, as it uses the cached result.
        _ = e.RemovePolicy("alice", "data1", "read");

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);

        // Now we invalidate the cache, then all first-coming Enforce() has to be evaluated in real-time.
        // The decision for ("alice", "data1", "read") will be false now.
        e.EnforceCache.Clear();

        TestEnforce(e, "alice", "data1", "read", false);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
    }

    [Fact]
    public void TestAutoCleanCache()
    {
#if !NET452
        Enforcer e = new(TestModelFixture.GetBasicTestModel())
        {
            Logger = new MockLogger<Enforcer>(_testOutputHelper)
        };
#else
            var e = new Enforcer(TestModelFixture.GetBasicTestModel());
#endif
        e.EnableCache(true);

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);

        // The cache is enabled, so even if we remove a policy rule, the decision
        // for ("alice", "data1", "read") will still be true, as it uses the cached result.
        _ = e.RemovePolicy("alice", "data1", "read");

        TestEnforce(e, "alice", "data1", "read", false);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
    }
}
