using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibLogica.Blocks.Memory;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

public class TestRamPerformance
{
    [Test]
    public void Ram1x8_DebugInfo_Performance_Baseline()
    {
        var ram = new Ram1x8();
        MeasureDebugInfoPerformance("Ram1x8", ram, 1000);
    }

    [Test]
    public void Ram16x8_DebugInfo_Performance_Baseline()
    {
        var ram = new Ram16x8();
        MeasureDebugInfoPerformance("Ram16x8", ram, 100);
    }

    [Test]
    public void Ram256x8_DebugInfo_Performance_Baseline()
    {
        var ram = new Ram256x8();
        MeasureDebugInfoPerformance("Ram256x8", ram, 10);
    }

    [Test]
    public void Ram256x8_DebugInfo_Performance_Option1_Caching()
    {
        var ram = new Ram256x8();
        
        // Test the new caching behavior by clearing cache between operations
        TestContext.Out.WriteLine("=== Testing Option 1: Caching Performance ===");
        
        // First call should populate cache
        var sw1 = Stopwatch.StartNew();
        var ids1 = ram.GetIds().ToList();
        var values1 = ram.GetValues().ToList();
        sw1.Stop();
        
        // Second call should use cache and be much faster  
        var sw2 = Stopwatch.StartNew();
        var ids2 = ram.GetIds().ToList();
        var values2 = ram.GetValues().ToList();
        sw2.Stop();
        
        TestContext.Out.WriteLine($"Ram256x8 first call (cache miss): {sw1.ElapsedMilliseconds}ms");
        TestContext.Out.WriteLine($"Ram256x8 second call (cache hit): {sw2.ElapsedMilliseconds}ms");
        TestContext.Out.WriteLine($"Cache speedup: {(Double)sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds:F1}x");
        
        // Verify results are identical
        Assert.That(ids1.SequenceEqual(ids2), Is.True, "Cached IDs should match original");
        Assert.That(values1.SequenceEqual(values2), Is.True, "Cached values should match original");
        Assert.That(ids1.Count, Is.EqualTo(values1.Count), "IDs and values must be aligned");
        
        // Cache hit should be significantly faster
        Assert.That(sw2.ElapsedMilliseconds, Is.LessThan(sw1.ElapsedMilliseconds / 2), 
                   "Cache hit should be at least 2x faster than cache miss");
    }

    private static void MeasureDebugInfoPerformance(String ramType, LibLogica.Gates.LogicElement ram, Int32 iterations)
    {
        // Warmup
        var warmupIds = ram.GetIds().ToList();
        var warmupValues = ram.GetValues().ToList();
        
        TestContext.Out.WriteLine($"{ramType} debug info count: {warmupIds.Count}");

        // Measure GetIds() performance
        var sw1 = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            var ids = ram.GetIds().ToList();
        }
        sw1.Stop();

        // Measure GetValues() performance
        var sw2 = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            var values = ram.GetValues().ToList();
        }
        sw2.Stop();

        // Measure combined performance
        var sw3 = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            var ids = ram.GetIds().ToList();
            var values = ram.GetValues().ToList();
        }
        sw3.Stop();

        Double idsPerMs = sw1.ElapsedMilliseconds > 0 ? (Double)iterations / sw1.ElapsedMilliseconds : iterations;
        Double valuesPerMs = sw2.ElapsedMilliseconds > 0 ? (Double)iterations / sw2.ElapsedMilliseconds : iterations;
        Double combinedPerMs = sw3.ElapsedMilliseconds > 0 ? (Double)iterations / sw3.ElapsedMilliseconds : iterations;

        TestContext.Out.WriteLine($"{ramType} GetIds(): {sw1.ElapsedMilliseconds}ms for {iterations} iterations ({idsPerMs:F1} ops/ms)");
        TestContext.Out.WriteLine($"{ramType} GetValues(): {sw2.ElapsedMilliseconds}ms for {iterations} iterations ({valuesPerMs:F1} ops/ms)");
        TestContext.Out.WriteLine($"{ramType} Combined: {sw3.ElapsedMilliseconds}ms for {iterations} iterations ({combinedPerMs:F1} ops/ms)");

        // Single operation timing
        var singleSw = Stopwatch.StartNew();
        var singleIds = ram.GetIds().ToList();
        var singleValues = ram.GetValues().ToList();
        singleSw.Stop();

        TestContext.Out.WriteLine($"{ramType} Single operation: {singleSw.ElapsedMilliseconds}ms ({singleIds.Count} entries)");
        
        // Verify alignment
        Assert.That(singleIds.Count, Is.EqualTo(singleValues.Count), "IDs and values must be aligned");
    }
}