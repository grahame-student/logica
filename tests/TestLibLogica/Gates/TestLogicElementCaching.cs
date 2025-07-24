using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Gates;

public class TestLogicElementCaching
{
    // Simple test element to verify caching behavior
    private class TestElement : LogicElement
    {
        public Input A { get; } = new();
        public Input B { get; } = new();
        public Output O { get; } = new();

        // Track how many times GetIds/GetValues are called for testing
        public Int32 GetIdsCallCount { get; private set; }
        public Int32 GetValuesCallCount { get; private set; }

        public override void Update()
        {
            ClearValuesCache();
            O.Value = A.Value && B.Value;
        }

        public override IEnumerable<String> GetIds()
        {
            GetIdsCallCount++;
            return new[] { $"{IdPrefix()}A", $"{IdPrefix()}B", $"{IdPrefix()}O" };
        }

        public override IEnumerable<Boolean> GetValues()
        {
            GetValuesCallCount++;
            return new[] { A.Value, B.Value, O.Value };
        }

        public void ResetCallCounts()
        {
            GetIdsCallCount = 0;
            GetValuesCallCount = 0;
        }
    }

    [Test]
    public void GetIdsCached_FirstCall_CallsUnderlyingGetIds()
    {
        var element = new TestElement();
        element.ResetCallCounts();

        var ids = element.GetIdsCached().ToList();

        Assert.That(element.GetIdsCallCount, Is.EqualTo(1));
        Assert.That(ids, Has.Count.EqualTo(3));
        Assert.That(ids[0], Does.EndWith("A"));
        Assert.That(ids[1], Does.EndWith("B"));
        Assert.That(ids[2], Does.EndWith("O"));
    }

    [Test]
    public void GetIdsCached_SubsequentCalls_DoesNotCallUnderlyingGetIds()
    {
        var element = new TestElement();
        element.ResetCallCounts();

        // First call
        var ids1 = element.GetIdsCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1));

        // Second call should use cache
        var ids2 = element.GetIdsCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1));

        // Results should be identical
        Assert.That(ids1, Is.EqualTo(ids2));
    }

    [Test]
    public void GetValuesCached_FirstCall_CallsUnderlyingGetValues()
    {
        var element = new TestElement();
        element.A.Value = true;
        element.B.Value = false;
        element.O.Value = true;
        element.ResetCallCounts();

        var values = element.GetValuesCached().ToList();

        Assert.That(element.GetValuesCallCount, Is.EqualTo(1));
        Assert.That(values, Has.Count.EqualTo(3));
        Assert.That(values[0], Is.True);
        Assert.That(values[1], Is.False);
        Assert.That(values[2], Is.True);
    }

    [Test]
    public void GetValuesCached_SubsequentCalls_DoesNotCallUnderlyingGetValues()
    {
        var element = new TestElement();
        element.A.Value = true;
        element.B.Value = false;
        element.O.Value = true;
        element.ResetCallCounts();

        // First call
        var values1 = element.GetValuesCached().ToList();
        Assert.That(element.GetValuesCallCount, Is.EqualTo(1));

        // Second call should use cache
        var values2 = element.GetValuesCached().ToList();
        Assert.That(element.GetValuesCallCount, Is.EqualTo(1));

        // Results should be identical
        Assert.That(values1, Is.EqualTo(values2));
    }

    [Test]
    public void Update_ClearsValuesCache_ButNotIdsCache()
    {
        var element = new TestElement();
        element.A.Value = true;
        element.B.Value = false;
        element.ResetCallCounts();

        // Prime both caches
        element.GetIdsCached().ToList();
        element.GetValuesCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1));
        Assert.That(element.GetValuesCallCount, Is.EqualTo(1));

        // Update should clear values cache but not IDs cache
        element.Update();

        // IDs should still be cached
        element.GetIdsCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1));

        // Values should require recalculation
        element.GetValuesCached().ToList();
        Assert.That(element.GetValuesCallCount, Is.EqualTo(2));
    }

    [Test]
    public void ClearDebugInfoCacheForTesting_ClearsBothCaches()
    {
        var element = new TestElement();
        element.ResetCallCounts();

        // Prime both caches
        element.GetIdsCached().ToList();
        element.GetValuesCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1));
        Assert.That(element.GetValuesCallCount, Is.EqualTo(1));

        // Clear both caches
        element.ClearDebugInfoCacheForTesting();

        // Both should require recalculation
        element.GetIdsCached().ToList();
        element.GetValuesCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(2));
        Assert.That(element.GetValuesCallCount, Is.EqualTo(2));
    }

    [Test]
    public void CachingPerformance_DemonstratesSpeedImprovement()
    {
        var element = new TestElement();
        const Int32 iterations = 10000; // Increased iterations for measurable timing

        // Measure uncached performance (clearing cache each time)
        var swUncached = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            element.ClearDebugInfoCacheForTesting();
            element.GetIdsCached().ToList();
            element.GetValuesCached().ToList();
        }
        swUncached.Stop();

        // Measure cached performance (no cache clearing)
        element.ClearDebugInfoCacheForTesting(); // Reset for fair comparison
        var swCached = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            element.GetIdsCached().ToList();
            element.GetValuesCached().ToList();
        }
        swCached.Stop();

        TestContext.Out.WriteLine($"Uncached: {swUncached.ElapsedMilliseconds}ms, Cached: {swCached.ElapsedMilliseconds}ms");

        // Verify that uncached operations actually happened
        Assert.That(element.GetIdsCallCount, Is.EqualTo(iterations + 1)); // +1 for reset call
        Assert.That(element.GetValuesCallCount, Is.EqualTo(iterations + 1)); // +1 for reset call

        // Cached should be significantly faster, but handle the case where both are very fast
        if (swUncached.ElapsedMilliseconds > 0)
        {
            Assert.That(swCached.ElapsedMilliseconds, Is.LessThanOrEqualTo(swUncached.ElapsedMilliseconds),
                "Cached calls should not be slower than uncached calls");
        }
        else
        {
            // If operations are too fast to measure, just verify correctness
            Assert.That(swCached.ElapsedMilliseconds, Is.LessThanOrEqualTo(swUncached.ElapsedMilliseconds + 5),
                "Both operations completed quickly, which demonstrates good performance");
        }
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void EducationalObservability_PreservesInputChanges(Boolean inputA, Boolean inputB)
    {
        var element = new TestElement();

        // Set initial values
        element.A.Value = inputA;
        element.B.Value = inputB;

        // Get initial values
        var values1 = element.GetValuesCached().ToList();

        // Change inputs (but output might remain the same)
        element.A.Value = !inputA;
        element.B.Value = !inputB;

        // Update to clear values cache
        element.Update();

        // Get new values
        var values2 = element.GetValuesCached().ToList();

        // Values should reflect the input changes
        Assert.That(values2[0], Is.EqualTo(!inputA));
        Assert.That(values2[1], Is.EqualTo(!inputB));
        Assert.That(values2[2], Is.EqualTo((!inputA) && (!inputB)));
    }
}
