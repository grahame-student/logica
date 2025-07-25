using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibLogica.Gates;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Gates;

/// <summary>
/// Professional-grade testing suite for LogicElement caching implementation.
/// Uses systematic testing techniques including boundary value analysis,
/// state transition testing, concurrency testing, and stress testing.
/// </summary>
public class TestLogicElementCachingProfessional
{
    /// <summary>
    /// Test element with controllable behavior for advanced testing scenarios.
    /// </summary>
    private class ControllableTestElement : LogicElement
    {
        public Input A { get; } = new();
        public Input B { get; } = new();
        public Output O { get; } = new();

        public Int32 GetIdsCallCount { get; private set; }
        public Int32 GetValuesCallCount { get; private set; }
        public Int32 UpdateCallCount { get; private set; }

        // Control behavior for testing edge cases
        public Boolean ThrowOnGetIds { get; set; }
        public Boolean ThrowOnGetValues { get; set; }
        public Boolean DelayOnGetIds { get; set; }
        public Boolean DelayOnGetValues { get; set; }

        public override void Update()
        {
            UpdateCallCount++;
            ClearValuesCache();
            O.Value = A.Value && B.Value;
        }

        public override IEnumerable<String> GetIds()
        {
            GetIdsCallCount++;
            if (ThrowOnGetIds)
                throw new InvalidOperationException("Test exception in GetIds");
            if (DelayOnGetIds)
                Thread.Sleep(10);
            return new[] { $"{IdPrefix()}A", $"{IdPrefix()}B", $"{IdPrefix()}O" };
        }

        public override IEnumerable<Boolean> GetValues()
        {
            GetValuesCallCount++;
            if (ThrowOnGetValues)
                throw new InvalidOperationException("Test exception in GetValues");
            if (DelayOnGetValues)
                Thread.Sleep(10);
            return new[] { A.Value, B.Value, O.Value };
        }

        public void ResetCounters()
        {
            GetIdsCallCount = 0;
            GetValuesCallCount = 0;
            UpdateCallCount = 0;
        }
    }

    /// <summary>
    /// Test element that creates deep hierarchies for stress testing.
    /// </summary>
    private class HierarchicalTestElement : LogicElement
    {
        private readonly HierarchicalTestElement[] _children;
        public Input A { get; } = new();
        public Output O { get; } = new();

        public Int32 Depth { get; }
        public Int32 ChildCount { get; }

        public HierarchicalTestElement(Int32 depth, Int32 childCount = 2)
        {
            Depth = depth;
            ChildCount = childCount;

            if (depth > 0)
            {
                _children = new HierarchicalTestElement[childCount];
                for (Int32 i = 0; i < childCount; i++)
                {
                    _children[i] = new HierarchicalTestElement(depth - 1, childCount);
                }
            }
            else
            {
                _children = Array.Empty<HierarchicalTestElement>();
            }
        }

        public override void Update()
        {
            ClearValuesCache();
            foreach (var child in _children)
            {
                child.Update();
            }
            O.Value = A.Value;
        }

        public override IEnumerable<String> GetIds()
        {
            var builder = DebugInfo().AddLocals((nameof(A), A), (nameof(O), O));
            foreach (var child in _children)
            {
                builder.AddChild(child);
            }
            return builder.Build().ids;
        }

        public override IEnumerable<Boolean> GetValues()
        {
            var builder = DebugInfo().AddLocals((nameof(A), A), (nameof(O), O));
            foreach (var child in _children)
            {
                builder.AddChild(child);
            }
            return builder.Build().values;
        }
    }

    #region Boundary Value Analysis

    [Test]
    public void CachingBehavior_EmptyElement_HandlesGracefully()
    {
        var element = new EmptyTestElement();

        var ids = element.GetIdsCached();
        var values = element.GetValuesCached();

        Assert.That(ids.Count(), Is.EqualTo(0));
        Assert.That(values.Count(), Is.EqualTo(0));
    }

    [Test]
    public void CachingBehavior_SingleElement_HandlesCorrectly()
    {
        var element = new SingleElementTest();

        var ids = element.GetIdsCached().ToList();
        var values = element.GetValuesCached().ToList();

        Assert.That(ids, Has.Count.EqualTo(1));
        Assert.That(values, Has.Count.EqualTo(1));
        Assert.That(ids[0], Does.EndWith("A"));
    }

    private class EmptyTestElement : LogicElement
    {
        public override void Update() => ClearValuesCache();
        public override IEnumerable<String> GetIds() => Enumerable.Empty<String>();
        public override IEnumerable<Boolean> GetValues() => Enumerable.Empty<Boolean>();
    }

    private class SingleElementTest : LogicElement
    {
        public Input A { get; } = new();
        public override void Update() => ClearValuesCache();
        public override IEnumerable<String> GetIds() => new[] { $"{IdPrefix()}A" };
        public override IEnumerable<Boolean> GetValues() => new[] { A.Value };
    }

    #endregion

    #region State Transition Testing

    [TestCase(false, true, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, true)]
    [TestCase(false, false, false)]
    public void StateTransition_ComplexUpdateSequence_MaintainsConsistency(Boolean initialA, Boolean initialB, Boolean finalA)
    {
        var element = new ControllableTestElement();
        element.A.Value = initialA;
        element.B.Value = initialB;
        element.ResetCounters();

        // Step 1: Prime caches
        var ids1 = element.GetIdsCached().ToList();
        var values1 = element.GetValuesCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1));
        Assert.That(element.GetValuesCallCount, Is.EqualTo(1));

        // Step 2: Update without changing values
        element.Update();
        Assert.That(element.UpdateCallCount, Is.EqualTo(1));

        // Step 3: Cache hit for IDs, miss for values
        var ids2 = element.GetIdsCached().ToList();
        var values2 = element.GetValuesCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1)); // Still cached
        Assert.That(element.GetValuesCallCount, Is.EqualTo(2)); // Recalculated

        // Step 4: Change state and verify consistency
        element.A.Value = finalA;
        element.Update();
        var values3 = element.GetValuesCached().ToList();

        Assert.That(values3[0], Is.EqualTo(finalA));
        Assert.That(values3[2], Is.EqualTo(finalA && initialB)); // Output should reflect new state
    }

    [Test]
    public void StateTransition_MultipleUpdatesWithoutAccess_MaintainsCorrectState()
    {
        var element = new ControllableTestElement();
        element.ResetCounters();

        // Multiple updates without accessing cache
        element.Update();
        element.Update();
        element.Update();
        Assert.That(element.UpdateCallCount, Is.EqualTo(3));

        // First access should still work correctly
        var ids = element.GetIdsCached().ToList();
        var values = element.GetValuesCached().ToList();

        Assert.That(element.GetIdsCallCount, Is.EqualTo(1));
        Assert.That(element.GetValuesCallCount, Is.EqualTo(1));
        Assert.That(ids, Has.Count.EqualTo(3));
        Assert.That(values, Has.Count.EqualTo(3));
    }

    #endregion

    #region Concurrency Testing

    [Test]
    public void ConcurrencyTest_MultipleThreadsAccessingCache_ThreadSafe()
    {
        var element = new ControllableTestElement();
        const Int32 threadCount = 10;
        const Int32 operationsPerThread = 100;
        var exceptions = new List<Exception>();
        var results = new List<(Int32 idsCount, Int32 valuesCount)>();

        var tasks = new Task[threadCount];
        for (Int32 i = 0; i < threadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                try
                {
                    for (Int32 j = 0; j < operationsPerThread; j++)
                    {
                        var ids = element.GetIdsCached();
                        var values = element.GetValuesCached();

                        // Ensure ids and values are not null before accessing
                        if (ids != null && values != null)
                        {
                            var idsList = ids.ToList();
                            var valuesList = values.ToList();

                            lock (results)
                            {
                                results.Add((idsList.Count, valuesList.Count));
                            }
                        }

                        // Randomly trigger updates to test cache invalidation
                        if (j % 10 == 0)
                        {
                            element.Update();
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            });
        }

        Task.WaitAll(tasks);

        Assert.That(exceptions, Is.Empty, $"Exceptions occurred: {String.Join(", ", exceptions.Select(e => e.Message))}");
        Assert.That(results, Has.Count.EqualTo(threadCount * operationsPerThread));
        Assert.That(results.All(r => r.idsCount == 3 && r.valuesCount == 3), Is.True);
    }

    [Test]
    public void ConcurrencyTest_SimultaneousUpdateAndAccess_NoRaceConditions()
    {
        var element = new ControllableTestElement();
        var barrier = new Barrier(2);
        var exceptions = new List<Exception>();

        var updateTask = Task.Run(() =>
        {
            try
            {
                barrier.SignalAndWait();
                for (Int32 i = 0; i < 1000; i++)
                {
                    element.A.Value = i % 2 == 0;
                    element.Update();
                    Thread.Yield();
                }
            }
            catch (Exception ex)
            {
                lock (exceptions) { exceptions.Add(ex); }
            }
        });

        var accessTask = Task.Run(() =>
        {
            try
            {
                barrier.SignalAndWait();
                for (Int32 i = 0; i < 1000; i++)
                {
                    var ids = element.GetIdsCached().ToList();
                    var values = element.GetValuesCached().ToList();
                    Assert.That(ids.Count, Is.EqualTo(values.Count));
                    Thread.Yield();
                }
            }
            catch (Exception ex)
            {
                lock (exceptions) { exceptions.Add(ex); }
            }
        });

        Task.WaitAll(updateTask, accessTask);
        Assert.That(exceptions, Is.Empty, $"Race conditions detected: {String.Join(", ", exceptions.Select(e => e.Message))}");
    }

    #endregion

    #region Stress Testing

    [TestCase(5, 2)] // 5 levels deep, 2 children each = 63 elements
    [TestCase(6, 2)] // 6 levels deep, 2 children each = 127 elements
    [TestCase(4, 3)] // 4 levels deep, 3 children each = 121 elements
    public void StressTest_DeepHierarchy_PerformsWithinLimits(Int32 depth, Int32 childCount)
    {
        var root = new HierarchicalTestElement(depth, childCount);
        var sw = Stopwatch.StartNew();

        // First call - should establish cache
        var ids1 = root.GetIdsCached().ToList();
        var values1 = root.GetValuesCached().ToList();
        var firstCallTime = sw.ElapsedMilliseconds;
        sw.Restart();

        // Second call - should use cache
        var ids2 = root.GetIdsCached().ToList();
        var values2 = root.GetValuesCached().ToList();
        var secondCallTime = sw.ElapsedMilliseconds;

        TestContext.Out.WriteLine($"Depth {depth}, Children {childCount}: " +
                            $"Elements: {ids1.Count}, First: {firstCallTime}ms, Second: {secondCallTime}ms");

        Assert.That(ids1.Count, Is.EqualTo(values1.Count));
        Assert.That(ids1, Is.EqualTo(ids2));
        Assert.That(values1, Is.EqualTo(values2));
        Assert.That(firstCallTime, Is.LessThan(1000), "First call should complete within 1 second");
        Assert.That(secondCallTime, Is.LessThanOrEqualTo(firstCallTime), "Cached call should not be slower");
    }

    [Test]
    public void StressTest_HighFrequencyOperations_MaintainsPerformance()
    {
        var element = new ControllableTestElement();
        const Int32 iterations = 10000;

        // Prime the cache
        element.GetIdsCached();
        element.GetValuesCached();
        element.ResetCounters();

        var sw = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            var ids = element.GetIdsCached();
            var values = element.GetValuesCached();
            // Consume to ensure enumeration happens
            _ = ids.Count();
            _ = values.Count();
        }
        sw.Stop();

        TestContext.Out.WriteLine($"{iterations} high-frequency operations: {sw.ElapsedMilliseconds}ms");

        // Should use cache for all operations
        Assert.That(element.GetIdsCallCount, Is.EqualTo(0));
        Assert.That(element.GetValuesCallCount, Is.EqualTo(0));
        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(100), "High-frequency cached operations should be very fast");
    }

    #endregion

    #region Error Handling Testing

    [Test]
    public void ErrorHandling_ExceptionInGetIds_DoesNotCorruptCache()
    {
        var element = new ControllableTestElement();
        element.ThrowOnGetIds = true;

        // First call should throw
        Assert.Throws<InvalidOperationException>(() => element.GetIdsCached().ToList());

        // Cache should not be corrupted
        element.ThrowOnGetIds = false;
        var ids = element.GetIdsCached().ToList();

        Assert.That(ids, Has.Count.EqualTo(3));
        Assert.That(element.GetIdsCallCount, Is.EqualTo(2)); // One failed, one successful
    }

    [Test]
    public void ErrorHandling_ExceptionInGetValues_DoesNotCorruptCache()
    {
        var element = new ControllableTestElement();
        element.ThrowOnGetValues = true;

        // First call should throw
        Assert.Throws<InvalidOperationException>(() => element.GetValuesCached().ToList());

        // Cache should not be corrupted
        element.ThrowOnGetValues = false;
        var values = element.GetValuesCached().ToList();

        Assert.That(values, Has.Count.EqualTo(3));
        Assert.That(element.GetValuesCallCount, Is.EqualTo(2)); // One failed, one successful
    }

    [Test]
    public void ErrorHandling_CacheStateAfterException_RecoversProperly()
    {
        var element = new ControllableTestElement();

        // Prime IDs cache successfully
        var ids1 = element.GetIdsCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1));

        // Cause exception in values
        element.ThrowOnGetValues = true;
        Assert.Throws<InvalidOperationException>(() => element.GetValuesCached().ToList());

        // IDs cache should still work
        var ids2 = element.GetIdsCached().ToList();
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1)); // Still cached
        Assert.That(ids1, Is.EqualTo(ids2));

        // Values should recover after fixing the exception
        element.ThrowOnGetValues = false;
        var values = element.GetValuesCached().ToList();
        Assert.That(values, Has.Count.EqualTo(3));
    }

    #endregion

    #region Memory and Performance Regression Testing

    [Test]
    public void MemoryTest_RepeatedOperations_NoMemoryLeak()
    {
        var element = new ControllableTestElement();
        const Int32 iterations = 1000;

        // Get baseline memory usage
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var initialMemory = GC.GetTotalMemory(false);

        for (Int32 i = 0; i < iterations; i++)
        {
            // Simulate real usage pattern
            element.GetIdsCached().ToList();
            element.GetValuesCached().ToList();
            element.Update();
            element.GetValuesCached().ToList(); // Should recalculate

            // Occasional cache clear
            if (i % 100 == 0)
            {
                element.ClearDebugInfoCacheForTesting();
            }
        }

        // Check memory usage
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(false);
        var memoryIncrease = finalMemory - initialMemory;

        TestContext.Out.WriteLine($"Memory increase after {iterations} operations: {memoryIncrease} bytes");

        // Allow some reasonable memory increase but detect major leaks
        Assert.That(memoryIncrease, Is.LessThan(1024 * 1024), // Less than 1MB increase
            $"Potential memory leak detected: {memoryIncrease} bytes increase");
    }

    [Test]
    public void PerformanceRegression_CacheVsNonCache_MeetsExpectedRatio()
    {
        var element = new ControllableTestElement();
        const Int32 iterations = 1000;

        // Measure uncached performance
        var swUncached = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            element.ClearDebugInfoCacheForTesting();
            _ = element.GetIdsCached().ToList();
            _ = element.GetValuesCached().ToList();
        }
        swUncached.Stop();

        // Measure cached performance
        element.ClearDebugInfoCacheForTesting();
        var swCached = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            _ = element.GetIdsCached().ToList();
            _ = element.GetValuesCached().ToList();
        }
        swCached.Stop();

        TestContext.Out.WriteLine($"Performance: Uncached={swUncached.ElapsedMilliseconds}ms, " +
                            $"Cached={swCached.ElapsedMilliseconds}ms");

        // Verify expected call counts
        Assert.That(element.GetIdsCallCount, Is.EqualTo(iterations + 1)); // +1 for final cache prime
        Assert.That(element.GetValuesCallCount, Is.EqualTo(iterations + 1));

        // Performance assertion with reasonable tolerance
        if (swUncached.ElapsedMilliseconds > 10) // Only if measurable
        {
            Double speedupRatio = (Double)swUncached.ElapsedMilliseconds / Math.Max(1, swCached.ElapsedMilliseconds);
            Assert.That(speedupRatio, Is.GreaterThan(2.0),
                $"Caching should provide at least 2x speedup, got {speedupRatio:F2}x");
        }
    }

    #endregion

    #region Integration Testing

    [Test]
    public void Integration_RealWorldUsagePattern_EducationalTool()
    {
        var element = new ControllableTestElement();

        // Simulate educational tool usage pattern:
        // 1. Student sets inputs
        element.A.Value = true;
        element.B.Value = false;

        // 2. System updates circuit
        element.Update();

        // 3. Student examines debug info multiple times (common in educational tools)
        for (Int32 i = 0; i < 5; i++)
        {
            var ids = element.GetIdsCached().ToList();
            var values = element.GetValuesCached().ToList();

            // Verify educational observability - inputs are visible even if output doesn't change
            Assert.That(values[0], Is.True); // A
            Assert.That(values[1], Is.False); // B
            Assert.That(values[2], Is.False); // O = A && B
        }

        // 4. Student changes inputs
        element.A.Value = false;
        element.B.Value = true;
        element.Update();

        // 5. Student examines again - should see changes
        var newValues = element.GetValuesCached().ToList();
        Assert.That(newValues[0], Is.False); // A changed
        Assert.That(newValues[1], Is.True);  // B changed
        Assert.That(newValues[2], Is.False); // O still false, but for different reason

        // Verify performance characteristic
        element.ResetCounters();
        for (Int32 i = 0; i < 10; i++)
        {
            element.GetIdsCached().ToList();
            element.GetValuesCached().ToList();
        }

        // Should use cache for all these calls
        Assert.That(element.GetIdsCallCount, Is.EqualTo(0));
        Assert.That(element.GetValuesCallCount, Is.EqualTo(0));
    }

    #endregion
}
