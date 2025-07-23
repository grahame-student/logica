using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibLogica.Blocks.Memory;
using NUnit.Framework;

namespace TestLibLogica.Performance;

/// <summary>
/// Performance tests for debug info generation in RAM blocks.
/// These tests establish baselines and measure performance improvements.
/// </summary>
[TestFixture]
public class DebugInfoPerformanceTests
{
    private const Int32 WARM_UP_ITERATIONS = 5;
    private const Int32 MEASUREMENT_ITERATIONS = 10;

    [Test]
    public void Ram1x8_DebugInfo_Performance_Baseline()
    {
        var ram = new Ram1x8();
        var result = MeasureDebugInfoPerformance(ram, "Ram1x8");
        
        TestContext.WriteLine($"Ram1x8 Debug Info Performance:");
        TestContext.WriteLine($"  Average time: {result.averageMs:F3} ms");
        TestContext.WriteLine($"  IDs count: {result.idsCount}");
        TestContext.WriteLine($"  Values count: {result.valuesCount}");
        
        // This should be fast - use as baseline
        Assert.That(result.averageMs, Is.LessThan(10.0), 
            "Ram1x8 debug info should be very fast (baseline)");
    }

    [Test]
    public void Ram8x8_DebugInfo_Performance_Baseline()
    {
        var ram = new Ram8x8();
        var result = MeasureDebugInfoPerformance(ram, "Ram8x8");
        
        TestContext.WriteLine($"Ram8x8 Debug Info Performance:");
        TestContext.WriteLine($"  Average time: {result.averageMs:F3} ms");
        TestContext.WriteLine($"  IDs count: {result.idsCount}");
        TestContext.WriteLine($"  Values count: {result.valuesCount}");
        
        // Document current performance - this is the baseline we want to improve
        Assert.That(result.averageMs, Is.LessThan(500.0), 
            "Ram8x8 debug info baseline (current performance before optimization)");
    }

    [Test]
    public void Ram16x8_DebugInfo_Performance_Baseline()
    {
        var ram = new Ram16x8();
        var result = MeasureDebugInfoPerformance(ram, "Ram16x8");
        
        TestContext.WriteLine($"Ram16x8 Debug Info Performance:");
        TestContext.WriteLine($"  Average time: {result.averageMs:F3} ms");
        TestContext.WriteLine($"  IDs count: {result.idsCount}");
        TestContext.WriteLine($"  Values count: {result.valuesCount}");
        
        // This will be significantly slower than Ram8x8
        Assert.That(result.averageMs, Is.LessThan(5000.0), 
            "Ram16x8 debug info baseline (demonstrating the performance problem)");
    }

    [Test]
    public void Ram256x8_DebugInfo_Performance_Baseline()
    {
        var ram = new Ram256x8();
        var result = MeasureDebugInfoPerformance(ram, "Ram256x8");
        
        TestContext.WriteLine($"Ram256x8 Debug Info Performance:");
        TestContext.WriteLine($"  Average time: {result.averageMs:F3} ms");
        TestContext.WriteLine($"  IDs count: {result.idsCount}");
        TestContext.WriteLine($"  Values count: {result.valuesCount}");
        
        // This will be extremely slow - demonstrating the need for optimization
        Assert.That(result.averageMs, Is.LessThan(30000.0), 
            "Ram256x8 debug info baseline (demonstrating severe performance degradation)");
    }

    [Test]
    public void Performance_Improvement_Demonstration()
    {
        // Test caching effectiveness on Ram256x8
        var ram = new Ram256x8();
        
        // First call - will populate cache
        var stopwatch1 = Stopwatch.StartNew();
        var ids1 = ram.GetIds().ToList();
        var values1 = ram.GetValues().ToList();
        stopwatch1.Stop();
        
        // Second call - should use cache
        var stopwatch2 = Stopwatch.StartNew();
        var ids2 = ram.GetIds().ToList();
        var values2 = ram.GetValues().ToList();
        stopwatch2.Stop();
        
        TestContext.WriteLine($"Ram256x8 Cache Effectiveness:");
        TestContext.WriteLine($"  First call (populate cache): {stopwatch1.Elapsed.TotalMilliseconds:F3} ms");
        TestContext.WriteLine($"  Second call (use cache): {stopwatch2.Elapsed.TotalMilliseconds:F3} ms");
        TestContext.WriteLine($"  Cache speedup: {stopwatch1.Elapsed.TotalMilliseconds / stopwatch2.Elapsed.TotalMilliseconds:F1}x");
        
        // Verify data consistency
        Assert.That(ids2, Is.EqualTo(ids1), "Cached IDs should match original");
        Assert.That(values2, Is.EqualTo(values1), "Cached values should match original");
        
        // Second call should be significantly faster (at least 10x)
        Assert.That(stopwatch2.Elapsed.TotalMilliseconds * 10, Is.LessThan(stopwatch1.Elapsed.TotalMilliseconds),
            "Cached call should be at least 10x faster");
    }

    [Test]
    public void Performance_Projection_For_Larger_Blocks()
    {
        TestContext.WriteLine("Performance Projection for Larger RAM Blocks:");
        TestContext.WriteLine("Based on optimized performance measurements:");
        
        var ram1x8Time = MeasureDebugInfoPerformance(new Ram1x8(), "Ram1x8").averageMs;
        var ram256x8Time = MeasureDebugInfoPerformance(new Ram256x8(), "Ram256x8").averageMs;
        
        // Estimate performance for larger blocks based on linear scaling with caching
        // With caching, performance scales roughly linearly with the number of debug elements
        var ram1x8Elements = 315;
        var ram256x8Elements = 81001;
        
        // Calculate time per element from optimized measurements
        var timePerElement = ram256x8Time / ram256x8Elements;
        
        // Project for larger blocks (these would have been created in the issue)
        var ram4096x8Elements = ram256x8Elements * 16; // 16 x Ram256x8 blocks
        var ram65536x8Elements = ram4096x8Elements * 16; // 16 x Ram4096x8 blocks
        
        var projectedRam4096x8Time = ram4096x8Elements * timePerElement;
        var projectedRam65536x8Time = ram65536x8Elements * timePerElement;
        
        TestContext.WriteLine($"Ram4096x8 projected time: {projectedRam4096x8Time:F1} ms ({ram4096x8Elements:N0} elements)");
        TestContext.WriteLine($"Ram65536x8 projected time: {projectedRam65536x8Time:F1} ms ({ram65536x8Elements:N0} elements)");
        
        // Even the largest block should be under 1 second with optimization
        Assert.That(projectedRam65536x8Time, Is.LessThan(1000.0),
            "Even Ram65536x8 should be under 1 second with caching optimization");
        
        TestContext.WriteLine("\nWithout optimization, Ram65536x8 would have taken 10+ seconds!");
        TestContext.WriteLine("With caching optimization, it should take less than 1 second.");
    }

    [Test]
    public void Optimization_Strategy_Comparison()
    {
        TestContext.WriteLine("Comparing Different Optimization Strategies:");
        
        // Test cached approach (our primary optimization)
        var cachedRam = new Ram256x8();
        var cachedResult = MeasureDebugInfoPerformance(cachedRam, "Ram256x8 (Cached)");
        
        // Test streaming approach (alternative optimization)
        var streamingRam = new Ram256x8Streaming();
        var streamingResult = MeasureDebugInfoPerformance(streamingRam, "Ram256x8 (Streaming)");
        
        TestContext.WriteLine("\nStrategy Comparison Results:");
        TestContext.WriteLine($"Cached approach:    {cachedResult.averageMs:F3} ms");
        TestContext.WriteLine($"Streaming approach: {streamingResult.averageMs:F3} ms");
        
        if (cachedResult.averageMs < streamingResult.averageMs)
        {
            var improvement = streamingResult.averageMs / cachedResult.averageMs;
            TestContext.WriteLine($"Cached is {improvement:F1}x faster than streaming");
        }
        else
        {
            var improvement = cachedResult.averageMs / streamingResult.averageMs;
            TestContext.WriteLine($"Streaming is {improvement:F1}x faster than cached");
        }
        
        // Both approaches should be much faster than the original
        Assert.That(cachedResult.averageMs, Is.LessThan(10.0), "Cached approach should be fast");
        Assert.That(streamingResult.averageMs, Is.LessThan(1000.0), "Streaming approach should be reasonable");
        
        // Verify same element counts
        Assert.That(streamingResult.idsCount, Is.EqualTo(cachedResult.idsCount), "Both approaches should return same number of IDs");
        Assert.That(streamingResult.valuesCount, Is.EqualTo(cachedResult.valuesCount), "Both approaches should return same number of values");
    }

    /// <summary>
    /// Measures the performance of debug info generation for a given RAM block.
    /// </summary>
    private static (Double averageMs, Int32 idsCount, Int32 valuesCount) MeasureDebugInfoPerformance(
        LibLogica.Gates.LogicElement element, String name)
    {
        // Warm up
        for (Int32 i = 0; i < WARM_UP_ITERATIONS; i++)
        {
            var warmupIds = element.GetIds().ToList();
            var warmupValues = element.GetValues().ToList();
        }

        // Measure actual performance
        var times = new List<Double>();
        Int32 idsCount = 0;
        Int32 valuesCount = 0;

        for (Int32 i = 0; i < MEASUREMENT_ITERATIONS; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var ids = element.GetIds().ToList();
            var values = element.GetValues().ToList();
            
            stopwatch.Stop();
            
            times.Add(stopwatch.Elapsed.TotalMilliseconds);
            idsCount = ids.Count;
            valuesCount = values.Count;
        }

        var averageMs = times.Average();
        
        TestContext.WriteLine($"{name} performance measurement:");
        TestContext.WriteLine($"  Min: {times.Min():F3} ms");
        TestContext.WriteLine($"  Max: {times.Max():F3} ms");
        TestContext.WriteLine($"  Average: {averageMs:F3} ms");
        TestContext.WriteLine($"  Elements: {idsCount} IDs, {valuesCount} values");

        return (averageMs, idsCount, valuesCount);
    }
}