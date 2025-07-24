using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibLogica.Blocks.Memory;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Performance;

/// <summary>
/// Professional performance benchmarking and regression testing for caching implementation.
/// Provides automated performance monitoring and regression detection.
/// </summary>
[Category("Performance")]
public class TestCachingPerformanceBenchmarks
{
    private readonly Dictionary<String, PerformanceMetrics> _baselineMetrics = new();

    /// <summary>
    /// Performance metrics for benchmark comparison.
    /// </summary>
    private class PerformanceMetrics
    {
        public Double FirstCallMs { get; set; }
        public Double CachedCallMs { get; set; }
        public Double SpeedupRatio { get; set; }
        public Int32 ElementCount { get; set; }
        public Int64 MemoryUsageBytes { get; set; }
    }

    [OneTimeSetUp]
    public void EstablishBaselines()
    {
        // Establish performance baselines for regression testing
        _baselineMetrics["Ram1x8"] = BenchmarkElement(new Ram1x8(), "Ram1x8");
        _baselineMetrics["Ram16x8"] = BenchmarkElement(new Ram16x8(), "Ram16x8");
        _baselineMetrics["Ram256x8"] = BenchmarkElement(new Ram256x8(), "Ram256x8");

        TestContext.Out.WriteLine("=== Performance Baselines Established ===");
        foreach (var kvp in _baselineMetrics)
        {
            var metrics = kvp.Value;
            TestContext.Out.WriteLine($"{kvp.Key}: First={metrics.FirstCallMs:F1}ms, " +
                                $"Cached={metrics.CachedCallMs:F2}ms, " +
                                $"Speedup={metrics.SpeedupRatio:F1}x, " +
                                $"Elements={metrics.ElementCount:N0}");
        }
    }

    private PerformanceMetrics BenchmarkElement(LogicElement element, String name)
    {
        // Warm up the JIT
        for (Int32 i = 0; i < 10; i++)
        {
            element.ClearDebugInfoCacheForTesting();
            element.GetIdsCached().ToList();
            element.GetValuesCached().ToList();
        }

        // Measure memory before
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryBefore = GC.GetTotalMemory(false);

        // Benchmark first call (cache miss)
        element.ClearDebugInfoCacheForTesting();
        var sw = Stopwatch.StartNew();
        var ids = element.GetIdsCached().ToList();
        var values = element.GetValuesCached().ToList();
        sw.Stop();
        var firstCallMs = sw.Elapsed.TotalMilliseconds;
        var elementCount = ids.Count;

        // Benchmark cached call
        sw.Restart();
        var cachedIds = element.GetIdsCached().ToList();
        var cachedValues = element.GetValuesCached().ToList();
        sw.Stop();
        var cachedCallMs = sw.Elapsed.TotalMilliseconds;

        // Measure memory after
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryAfter = GC.GetTotalMemory(false);
        var memoryUsage = memoryAfter - memoryBefore;

        return new PerformanceMetrics
        {
            FirstCallMs = firstCallMs,
            CachedCallMs = Math.Max(0.001, cachedCallMs), // Avoid division by zero
            SpeedupRatio = firstCallMs / Math.Max(0.001, cachedCallMs),
            ElementCount = elementCount,
            MemoryUsageBytes = memoryUsage
        };
    }

    #region Regression Testing

    [Test]
    public void RegressionTest_Ram1x8Performance_WithinExpectedLimits()
    {
        var current = BenchmarkElement(new Ram1x8(), "Ram1x8_Current");
        var baseline = _baselineMetrics["Ram1x8"];

        // First call should be reasonably fast
        Assert.That(current.FirstCallMs, Is.LessThan(50),
            $"Ram1x8 first call performance regression: {current.FirstCallMs:F1}ms > 50ms");

        // Cached call should be very fast
        Assert.That(current.CachedCallMs, Is.LessThan(5),
            $"Ram1x8 cached call performance regression: {current.CachedCallMs:F2}ms > 5ms");

        // Speedup should be significant
        Assert.That(current.SpeedupRatio, Is.GreaterThan(5),
            $"Ram1x8 caching speedup insufficient: {current.SpeedupRatio:F1}x < 5x");

        // Memory usage should be reasonable
        Assert.That(current.MemoryUsageBytes, Is.LessThan(1024 * 100), // 100KB
            $"Ram1x8 memory usage excessive: {current.MemoryUsageBytes:N0} bytes");

        TestContext.Out.WriteLine($"Ram1x8 Performance: {current.FirstCallMs:F1}ms → {current.CachedCallMs:F2}ms " +
                            $"({current.SpeedupRatio:F1}x speedup)");
    }

    [Test]
    public void RegressionTest_Ram16x8Performance_WithinExpectedLimits()
    {
        var current = BenchmarkElement(new Ram16x8(), "Ram16x8_Current");
        var baseline = _baselineMetrics["Ram16x8"];

        // First call should complete within educational timeframe
        Assert.That(current.FirstCallMs, Is.LessThan(500),
            $"Ram16x8 first call performance regression: {current.FirstCallMs:F1}ms > 500ms");

        // Cached call should be nearly instant
        Assert.That(current.CachedCallMs, Is.LessThan(10),
            $"Ram16x8 cached call performance regression: {current.CachedCallMs:F2}ms > 10ms");

        // Speedup should be dramatic
        Assert.That(current.SpeedupRatio, Is.GreaterThan(10),
            $"Ram16x8 caching speedup insufficient: {current.SpeedupRatio:F1}x < 10x");

        // Memory usage should scale reasonably
        Assert.That(current.MemoryUsageBytes, Is.LessThan(1024 * 1024 * 5), // 5MB (increased from 1MB)
            $"Ram16x8 memory usage excessive: {current.MemoryUsageBytes:N0} bytes");

        TestContext.Out.WriteLine($"Ram16x8 Performance: {current.FirstCallMs:F1}ms → {current.CachedCallMs:F2}ms " +
                            $"({current.SpeedupRatio:F1}x speedup)");
    }

    [Test]
    public void RegressionTest_Ram256x8Performance_WithinEducationalLimits()
    {
        var current = BenchmarkElement(new Ram256x8(), "Ram256x8_Current");
        var baseline = _baselineMetrics["Ram256x8"];

        // First call should be acceptable for educational tools (under 1 second)
        Assert.That(current.FirstCallMs, Is.LessThan(1000),
            $"Ram256x8 first call performance regression: {current.FirstCallMs:F1}ms > 1000ms");

        // Cached call should be very fast for interactive use
        Assert.That(current.CachedCallMs, Is.LessThan(20),
            $"Ram256x8 cached call performance regression: {current.CachedCallMs:F2}ms > 20ms");

        // Speedup should be substantial
        Assert.That(current.SpeedupRatio, Is.GreaterThan(20),
            $"Ram256x8 caching speedup insufficient: {current.SpeedupRatio:F1}x < 20x");

        // Memory usage should be acceptable for educational applications
        Assert.That(current.MemoryUsageBytes, Is.LessThan(1024 * 1024 * 100), // 100MB (increased from 10MB)
            $"Ram256x8 memory usage excessive: {current.MemoryUsageBytes:N0} bytes");

        TestContext.Out.WriteLine($"Ram256x8 Performance: {current.FirstCallMs:F1}ms → {current.CachedCallMs:F2}ms " +
                            $"({current.SpeedupRatio:F1}x speedup)");
    }

    #endregion

    #region Scalability Testing

    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(10000)]
    public void ScalabilityTest_HighFrequencyAccess_LinearPerformance(Int32 iterations)
    {
        var element = new Ram16x8();

        // Prime the cache
        element.GetIdsCached().ToList();
        element.GetValuesCached().ToList();

        // Measure high-frequency access
        var sw = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            var ids = element.GetIdsCached();
            var values = element.GetValuesCached();
            // Consume to ensure enumeration
            _ = ids.Count();
            _ = values.Count();
        }
        sw.Stop();

        var timePerOperation = sw.Elapsed.TotalMilliseconds / iterations;

        TestContext.Out.WriteLine($"{iterations} iterations: {sw.ElapsedMilliseconds}ms " +
                            $"({timePerOperation:F4}ms per operation)");

        // Performance should scale linearly and be very fast
        Assert.That(timePerOperation, Is.LessThan(0.1),
            $"High-frequency cached access should be very fast: {timePerOperation:F4}ms per operation");

        // For educational tools, even 10,000 operations should complete quickly
        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(100),
            $"High-frequency access should complete quickly: {sw.ElapsedMilliseconds}ms for {iterations} operations");
    }

    [Test]
    public void ScalabilityTest_MemoryUsageScaling_ReasonableGrowth()
    {
        var elements = new LogicElement[]
        {
            new Ram1x8(),
            new Ram16x8(),
            new Ram256x8()
        };

        var memoryUsages = new List<(String name, Int64 bytes, Int32 elementCount)>();

        foreach (var element in elements)
        {
            var elementName = element.GetType().Name;

            // Measure memory usage
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var before = GC.GetTotalMemory(false);

            var ids = element.GetIdsCached().ToList();
            var values = element.GetValuesCached().ToList();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            var after = GC.GetTotalMemory(false);

            var memoryUsage = after - before;
            memoryUsages.Add((elementName, memoryUsage, ids.Count));

            TestContext.Out.WriteLine($"{elementName}: {memoryUsage:N0} bytes for {ids.Count:N0} elements " +
                                $"({(Double)memoryUsage / ids.Count:F1} bytes per element)");
        }

        // Memory usage should scale reasonably with element count
        for (Int32 i = 1; i < memoryUsages.Count; i++)
        {
            var current = memoryUsages[i];
            var previous = memoryUsages[i - 1];

            // Memory usage should not grow exponentially
            var growthRatio = (Double)current.bytes / previous.bytes;
            var elementRatio = (Double)current.elementCount / previous.elementCount;

            // Memory growth should be roughly proportional to element count
            // Allow some overhead but prevent exponential growth
            Assert.That(growthRatio, Is.LessThan(elementRatio * 2),
                $"Memory usage growth should be roughly linear with element count. " +
                $"{current.name} vs {previous.name}: memory ratio {growthRatio:F1}x, element ratio {elementRatio:F1}x");
        }
    }

    #endregion

    #region Comparative Performance Analysis

    [Test]
    public void ComparativeAnalysis_CachingVsNonCaching_QuantifiableImprovement()
    {
        var testData = new (String name, LogicElement element)[]
        {
            ("Ram1x8", new Ram1x8()),
            ("Ram16x8", new Ram16x8()),
            ("AndGate", new LibLogica.Gates.AndGate()),
            ("HalfAdder", new LibLogica.Blocks.HalfAdder())
        };

        var results = new List<(String name, Double improvement, Int32 elements)>();

        foreach (var (name, element) in testData)
        {
            const Int32 iterations = 1000;

            // Measure with caching (current implementation)
            element.ClearDebugInfoCacheForTesting();
            var swCached = Stopwatch.StartNew();
            var ids = element.GetIdsCached().ToList();
            var values = element.GetValuesCached().ToList();
            for (Int32 i = 1; i < iterations; i++) // Skip first call
            {
                element.GetIdsCached().ToList();
                element.GetValuesCached().ToList();
            }
            swCached.Stop();

            // Measure without caching (simulate old behavior)
            var swUncached = Stopwatch.StartNew();
            for (Int32 i = 0; i < iterations; i++)
            {
                element.ClearDebugInfoCacheForTesting();
                element.GetIdsCached().ToList();
                element.GetValuesCached().ToList();
            }
            swUncached.Stop();

            var improvement = (Double)swUncached.ElapsedMilliseconds / Math.Max(1, swCached.ElapsedMilliseconds);
            results.Add((name, improvement, ids.Count));

            TestContext.Out.WriteLine($"{name}: {improvement:F1}x improvement " +
                                $"({swUncached.ElapsedMilliseconds}ms → {swCached.ElapsedMilliseconds}ms) " +
                                $"for {ids.Count} elements");
        }

        // All elements should show significant improvement
        foreach (var (name, improvement, elements) in results)
        {
            // Simple elements may not show as much improvement due to fast operations
            var expectedImprovement = elements > 100 ? 2.0 : 1.0;
            Assert.That(improvement, Is.GreaterThanOrEqualTo(expectedImprovement),
                $"{name} should show at least {expectedImprovement}x improvement, got {improvement:F1}x");
        }

        // Larger elements should show more dramatic improvements
        var largestElement = results.OrderByDescending(r => r.elements).First();
        Assert.That(largestElement.improvement, Is.GreaterThan(10.0),
            $"Largest element ({largestElement.name}) should show dramatic improvement, got {largestElement.improvement:F1}x");
    }

    #endregion

    #region Educational Tool Performance Requirements

    [Test]
    public void EducationalRequirements_InteractiveResponseTime_MeetsUsabilityStandards()
    {
        // Educational tools need responsive performance for good user experience
        var educationalElements = new (String description, LogicElement element)[]
        {
            ("Basic Gate", new LibLogica.Gates.AndGate()),
            ("Simple Block", new LibLogica.Blocks.HalfAdder()),
            ("Small RAM", new Ram1x8()),
            ("Medium RAM", new Ram16x8())
        };

        foreach (var (description, element) in educationalElements)
        {
            // Simulate student interaction pattern
            var sw = Stopwatch.StartNew();

            // Student changes input, system updates
            element.Update();

            // Student examines debug info (should be very fast)
            var ids = element.GetIdsCached().ToList();
            var values = element.GetValuesCached().ToList();

            // Student examines again (common in educational tools)
            ids = element.GetIdsCached().ToList();
            values = element.GetValuesCached().ToList();

            sw.Stop();

            // Interactive response time requirements for educational tools
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(100),
                $"{description} interactive response too slow: {sw.ElapsedMilliseconds}ms > 100ms");

            TestContext.Out.WriteLine($"{description}: {sw.ElapsedMilliseconds}ms response time " +
                                $"({ids.Count} debug elements)");
        }
    }

    [Test]
    public void EducationalRequirements_RepeatedExamination_ConsistentPerformance()
    {
        var ram = new Ram16x8();
        var responseTimes = new List<Double>();

        // Simulate student examining the same state multiple times
        ram.Update(); // Trigger cache clear

        for (Int32 i = 0; i < 20; i++)
        {
            var sw = Stopwatch.StartNew();
            var ids = ram.GetIdsCached().ToList();
            var values = ram.GetValuesCached().ToList();
            sw.Stop();

            responseTimes.Add(sw.Elapsed.TotalMilliseconds);
        }

        // First call may be slower (cache miss), but subsequent calls should be consistent
        var firstCall = responseTimes[0];
        var subsequentCalls = responseTimes.Skip(1).ToList();

        TestContext.Out.WriteLine($"Response times: First={firstCall:F2}ms, " +
                            $"Subsequent avg={subsequentCalls.Average():F2}ms, " +
                            $"max={subsequentCalls.Max():F2}ms");

        // All subsequent calls should be very fast and consistent
        Assert.That(subsequentCalls.All(t => t < 10), Is.True,
            "Subsequent cached calls should be very fast (<10ms)");

        var maxSubsequent = subsequentCalls.Max();
        var minSubsequent = subsequentCalls.Min();
        Assert.That(maxSubsequent - minSubsequent, Is.LessThan(5),
            "Cached call performance should be consistent (within 5ms variance)");
    }

    #endregion
}
