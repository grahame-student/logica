using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibLogica.Blocks.Memory;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Gates;

/// <summary>
/// Advanced mutation and data integrity testing for caching implementation.
/// Verifies that the caching mechanism actually provides the intended benefits
/// and maintains data consistency under complex scenarios.
/// </summary>
public class TestLogicElementCachingMutation
{
    /// <summary>
    /// Test element that can simulate cache-disabled behavior for mutation testing.
    /// </summary>
    private class MutationTestElement : LogicElement
    {
        private readonly List<String> _ids;
        private readonly List<Boolean> _values;

        public Boolean CachingEnabled { get; set; } = true;
        public Int32 GetIdsCallCount { get; private set; }
        public Int32 GetValuesCallCount { get; private set; }

        public MutationTestElement()
        {
            _ids = new List<String> { $"{IdPrefix()}Input", $"{IdPrefix()}Output" };
            _values = new List<Boolean> { false, false };
        }

        public void SetValues(Boolean input, Boolean output)
        {
            _values[0] = input;
            _values[1] = output;
        }

        public override void Update()
        {
            if (CachingEnabled)
                ClearValuesCache();
        }

        public override IEnumerable<String> GetIds()
        {
            GetIdsCallCount++;
            return _ids.ToList(); // Return copy to prevent external modification
        }

        public override IEnumerable<Boolean> GetValues()
        {
            GetValuesCallCount++;
            return _values.ToList(); // Return copy to prevent external modification
        }

        // Override cached methods to simulate disabled caching for mutation testing
        public new IEnumerable<String> GetIdsCached()
        {
            if (CachingEnabled)
                return base.GetIdsCached();
            else
                return GetIds(); // Always call underlying method when caching disabled
        }

        public new IEnumerable<Boolean> GetValuesCached()
        {
            if (CachingEnabled)
                return base.GetValuesCached();
            else
                return GetValues(); // Always call underlying method when caching disabled
        }

        public void ResetCounters()
        {
            GetIdsCallCount = 0;
            GetValuesCallCount = 0;
        }
    }

    #region Mutation Testing - Verify Caching Actually Works

    [Test]
    public void MutationTest_CachingEnabled_ReducesMethodCalls()
    {
        var element = new MutationTestElement { CachingEnabled = true };
        element.ResetCounters();
        const Int32 operations = 100;

        // Perform multiple operations
        for (Int32 i = 0; i < operations; i++)
        {
            element.GetIdsCached().ToList();
            element.GetValuesCached().ToList();
        }

        // With caching, underlying methods should be called only once each
        Assert.That(element.GetIdsCallCount, Is.EqualTo(1),
            "Caching should reduce GetIds calls to 1");
        Assert.That(element.GetValuesCallCount, Is.EqualTo(1),
            "Caching should reduce GetValues calls to 1");
    }

    [Test]
    public void MutationTest_CachingDisabled_CallsUnderlyingMethods()
    {
        var element = new MutationTestElement { CachingEnabled = false };
        element.ResetCounters();
        const Int32 operations = 100;

        // Perform multiple operations
        for (Int32 i = 0; i < operations; i++)
        {
            element.GetIdsCached().ToList();
            element.GetValuesCached().ToList();
        }

        // Without caching, underlying methods should be called every time
        Assert.That(element.GetIdsCallCount, Is.EqualTo(operations),
            "Without caching, GetIds should be called every time");
        Assert.That(element.GetValuesCallCount, Is.EqualTo(operations),
            "Without caching, GetValues should be called every time");
    }

    [Test]
    public void MutationTest_PerformanceImpactOfCaching_MeasurableDifference()
    {
        const Int32 iterations = 1000;

        // Test with caching enabled
        var elementCached = new MutationTestElement { CachingEnabled = true };
        var swCached = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            elementCached.GetIdsCached().ToList();
            elementCached.GetValuesCached().ToList();
        }
        swCached.Stop();

        // Test with caching disabled
        var elementUncached = new MutationTestElement { CachingEnabled = false };
        var swUncached = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            elementUncached.GetIdsCached().ToList();
            elementUncached.GetValuesCached().ToList();
        }
        swUncached.Stop();

        TestContext.Out.WriteLine($"Cached: {swCached.ElapsedMilliseconds}ms, " +
                            $"Uncached: {swUncached.ElapsedMilliseconds}ms");

        // Verify method call counts confirm the mutation
        Assert.That(elementCached.GetIdsCallCount, Is.EqualTo(1));
        Assert.That(elementCached.GetValuesCallCount, Is.EqualTo(1));
        Assert.That(elementUncached.GetIdsCallCount, Is.EqualTo(iterations));
        Assert.That(elementUncached.GetValuesCallCount, Is.EqualTo(iterations));

        // Performance should be measurably different
        Assert.That(swCached.ElapsedMilliseconds, Is.LessThanOrEqualTo(swUncached.ElapsedMilliseconds),
            "Cached version should not be slower than uncached");
    }

    #endregion

    #region Data Integrity Testing

    [Test]
    public void DataIntegrity_CacheConsistency_AlignedIdsAndValues()
    {
        var element = new MutationTestElement();
        element.SetValues(true, false);

        // Get cached results multiple times
        var results = new List<(String[] ids, Boolean[] values)>();
        for (Int32 i = 0; i < 10; i++)
        {
            var ids = element.GetIdsCached().ToArray();
            var values = element.GetValuesCached().ToArray();
            results.Add((ids, values));
        }

        // All results should be identical and aligned
        var firstResult = results[0];
        foreach (var result in results.Skip(1))
        {
            Assert.That(result.ids, Is.EqualTo(firstResult.ids),
                "Cached IDs should be consistent across calls");
            Assert.That(result.values, Is.EqualTo(firstResult.values),
                "Cached values should be consistent across calls");
            Assert.That(result.ids.Length, Is.EqualTo(result.values.Length),
                "IDs and values must be aligned");
        }
    }

    [Test]
    public void DataIntegrity_StateChange_ValuesUpdateCorrectly()
    {
        var element = new MutationTestElement();

        // Initial state
        element.SetValues(false, false);
        var initialValues = element.GetValuesCached().ToArray();
        Assert.That(initialValues, Is.EqualTo(new[] { false, false }));

        // Change state and update
        element.SetValues(true, true);
        element.Update(); // Should clear values cache
        var updatedValues = element.GetValuesCached().ToArray();

        Assert.That(updatedValues, Is.EqualTo(new[] { true, true }));
        Assert.That(updatedValues, Is.Not.EqualTo(initialValues),
            "Values should reflect state changes after update");
    }

    [Test]
    public void DataIntegrity_IdsNeverChange_ConsistentAcrossUpdates()
    {
        var element = new MutationTestElement();

        // Get initial IDs
        var initialIds = element.GetIdsCached().ToArray();

        // Perform multiple updates and state changes
        for (Int32 i = 0; i < 10; i++)
        {
            element.SetValues(i % 2 == 0, i % 3 == 0);
            element.Update();

            var currentIds = element.GetIdsCached().ToArray();
            Assert.That(currentIds, Is.EqualTo(initialIds),
                $"IDs should never change, iteration {i}");
        }
    }

    [Test]
    public void DataIntegrity_CacheInvalidation_PreservesCorrectnessUnderComplexUpdates()
    {
        var element = new MutationTestElement();

        // Complex update pattern that might confuse caching logic
        var expectedStates = new[]
        {
            (false, false),
            (true, false),
            (false, true),
            (true, true),
            (false, false)
        };

        foreach (var (input, output) in expectedStates)
        {
            element.SetValues(input, output);
            element.Update();

            // Access cache multiple times to ensure consistency
            for (Int32 i = 0; i < 3; i++)
            {
                var values = element.GetValuesCached().ToArray();
                Assert.That(values[0], Is.EqualTo(input),
                    $"Input value should be {input}");
                Assert.That(values[1], Is.EqualTo(output),
                    $"Output value should be {output}");
            }
        }
    }

    #endregion

    #region Real-world Integration Testing

    [Test]
    public void Integration_ActualRamBlock_CachingBehaviorIsCorrect()
    {
        var ram = new Ram1x8();

        // Track initial method call counts using the testing method
        ram.ClearDebugInfoCacheForTesting(); // Ensure clean state

        // First access should populate cache
        var ids1 = ram.GetIdsCached().ToList();
        var values1 = ram.GetValuesCached().ToList();

        // Subsequent accesses should use cache
        var ids2 = ram.GetIdsCached().ToList();
        var values2 = ram.GetValuesCached().ToList();

        // Results should be identical
        Assert.That(ids2, Is.EqualTo(ids1));
        Assert.That(values2, Is.EqualTo(values1));
        Assert.That(ids1.Count, Is.EqualTo(values1.Count));
        Assert.That(ids1.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Integration_ComplexRamHierarchy_CachingWorksAtAllLevels()
    {
        var ram = new Ram16x8();
        var sw = Stopwatch.StartNew();

        // First call - establishes cache hierarchy
        var ids1 = ram.GetIdsCached().ToList();
        var values1 = ram.GetValuesCached().ToList();
        var firstCallTime = sw.ElapsedMilliseconds;
        sw.Restart();

        // Second call - should use cached results
        var ids2 = ram.GetIdsCached().ToList();
        var values2 = ram.GetValuesCached().ToList();
        var secondCallTime = sw.ElapsedMilliseconds;

        TestContext.Out.WriteLine($"Ram16x8 caching: First={firstCallTime}ms, Second={secondCallTime}ms, " +
                            $"Entries={ids1.Count}");

        // Verify correctness
        Assert.That(ids2, Is.EqualTo(ids1));
        Assert.That(values2, Is.EqualTo(values1));
        Assert.That(ids1.Count, Is.EqualTo(values1.Count));

        // Verify performance benefit
        Assert.That(secondCallTime, Is.LessThanOrEqualTo(firstCallTime),
            "Cached call should not be slower than initial call");

        // For educational use, this should complete quickly
        Assert.That(firstCallTime, Is.LessThan(1000),
            "Even first call should complete within reasonable time for educational use");
    }

    [TestCase(5)]
    [TestCase(10)]
    [TestCase(50)]
    public void Integration_EducationalUsageSimulation_PerformanceIsAcceptable(Int32 studentInteractions)
    {
        var ram = new Ram16x8();

        // Simulate educational scenario
        var sw = Stopwatch.StartNew();

        // Student changes input
        ram.Address[0].Value = true;
        ram.Update();

        // Student examines state multiple times (typical in educational tools)
        for (Int32 i = 0; i < studentInteractions; i++)
        {
            var ids = ram.GetIdsCached();
            var values = ram.GetValuesCached();

            // Consume results to simulate actual usage
            var idCount = ids.Count();
            var valueCount = values.Count();

            Assert.That(idCount, Is.EqualTo(valueCount));
        }

        sw.Stop();

        TestContext.Out.WriteLine($"{studentInteractions} educational interactions: {sw.ElapsedMilliseconds}ms");

        // Educational tools need responsive performance
        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(100),
            $"Educational interactions should be responsive: {sw.ElapsedMilliseconds}ms for {studentInteractions} interactions");
    }

    #endregion

    #region Compatibility and Inheritance Testing

    [Test]
    public void Compatibility_AllLogicElementSubclasses_ImplementCachingCorrectly()
    {
        // Test various LogicElement subclasses to ensure caching works universally
        var elements = new LogicElement[]
        {
            new LibLogica.Gates.AndGate(),
            new LibLogica.Gates.OrGate(),
            new LibLogica.Gates.NotGate(),
            new LibLogica.Gates.XorGate(),
            new LibLogica.Blocks.Memory.Ram1x8(),
            new LibLogica.Blocks.HalfAdder(),
            new LibLogica.Blocks.FullAdder()
        };

        foreach (var element in elements)
        {
            var elementType = element.GetType().Name;

            // Clear cache to ensure clean test
            element.ClearDebugInfoCacheForTesting();

            // Test basic caching functionality
            var ids1 = element.GetIdsCached().ToList();
            var values1 = element.GetValuesCached().ToList();
            var ids2 = element.GetIdsCached().ToList();
            var values2 = element.GetValuesCached().ToList();

            Assert.That(ids2, Is.EqualTo(ids1), $"{elementType}: IDs should be cached");
            Assert.That(values2, Is.EqualTo(values1), $"{elementType}: Values should be cached");
            Assert.That(ids1.Count, Is.EqualTo(values1.Count), $"{elementType}: IDs and values must be aligned");
            Assert.That(ids1.Count, Is.GreaterThan(0), $"{elementType}: Should have some debug entries");

            // Test update behavior
            element.Update();
            var valuesAfterUpdate = element.GetValuesCached().ToList();
            var idsAfterUpdate = element.GetIdsCached().ToList();

            Assert.That(idsAfterUpdate, Is.EqualTo(ids1), $"{elementType}: IDs should remain cached after update");
            Assert.That(valuesAfterUpdate.Count, Is.EqualTo(values1.Count), $"{elementType}: Values count should be consistent");
        }
    }

    #endregion
}
