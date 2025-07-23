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