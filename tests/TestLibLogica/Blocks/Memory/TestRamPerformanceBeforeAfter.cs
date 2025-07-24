using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibLogica.Blocks.Memory;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

public class TestRamPerformanceBeforeAfter
{
    [Test]
    public void Ram256x8_Performance_CacheMissVsCacheHit()
    {
        TestContext.Out.WriteLine("=== Demonstrating Cache Performance Impact ===");
        
        var ram = new Ram256x8();
        
        // Force cache miss by clearing cache before each measurement
        var sw1 = Stopwatch.StartNew();
        ClearDebugInfoCache(ram);
        var ids1 = ram.GetIds().ToList();
        var values1 = ram.GetValues().ToList();
        sw1.Stop();
        
        // This should be a cache hit
        var sw2 = Stopwatch.StartNew();
        var ids2 = ram.GetIds().ToList();
        var values2 = ram.GetValues().ToList();
        sw2.Stop();
        
        // Force another cache miss
        var sw3 = Stopwatch.StartNew();
        ClearDebugInfoCache(ram);
        var ids3 = ram.GetIds().ToList();
        var values3 = ram.GetValues().ToList();
        sw3.Stop();
        
        TestContext.Out.WriteLine($"Ram256x8 with {ids1.Count:N0} debug entries:");
        TestContext.Out.WriteLine($"  Cache miss #1: {sw1.ElapsedMilliseconds}ms");
        TestContext.Out.WriteLine($"  Cache hit:     {sw2.ElapsedMilliseconds}ms");
        TestContext.Out.WriteLine($"  Cache miss #2: {sw3.ElapsedMilliseconds}ms");
        
        Double speedup = sw1.ElapsedMilliseconds > 0 ? 
            (Double)sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1) : 
            Double.PositiveInfinity;
            
        TestContext.Out.WriteLine($"  Cache speedup: {speedup:F1}x");
        
        // Verify correctness
        Assert.That(ids1.SequenceEqual(ids2), Is.True, "Cache hit should return identical IDs");
        Assert.That(values1.SequenceEqual(values2), Is.True, "Cache hit should return identical values");
        Assert.That(ids1.SequenceEqual(ids3), Is.True, "Second cache miss should return identical IDs");
        Assert.That(values1.SequenceEqual(values3), Is.True, "Second cache miss should return identical values");
        
        // Performance should be significantly better for cache hits
        Assert.That(sw2.ElapsedMilliseconds, Is.LessThanOrEqualTo(sw1.ElapsedMilliseconds / 2), 
                   "Cache hits should be at least 2x faster than cache misses");
    }

    [Test] 
    public void MultipleLevels_CacheClearing_VerifiesIndependentCaching()
    {
        TestContext.Out.WriteLine("=== Testing Independent Caching at Multiple Levels ===");
        
        var ram = new Ram256x8();
        
        // First access - should populate all caches in hierarchy
        var sw1 = Stopwatch.StartNew();
        var allIds = ram.GetIds().ToList();
        sw1.Stop();
        
        TestContext.Out.WriteLine($"Initial population: {sw1.ElapsedMilliseconds}ms for {allIds.Count:N0} entries");
        
        // Clear only the top-level cache
        ClearDebugInfoCache(ram);
        
        // This should be faster than initial but slower than full cache hit
        // because child caches are still populated
        var sw2 = Stopwatch.StartNew();
        var ids2 = ram.GetIds().ToList();
        sw2.Stop();
        
        TestContext.Out.WriteLine($"Top-level cache miss (children cached): {sw2.ElapsedMilliseconds}ms");
        
        // Full cache hit
        var sw3 = Stopwatch.StartNew();
        var ids3 = ram.GetIds().ToList();
        sw3.Stop();
        
        TestContext.Out.WriteLine($"Full cache hit: {sw3.ElapsedMilliseconds}ms");
        
        Assert.That(allIds.SequenceEqual(ids2), Is.True, "Results should be identical");
        Assert.That(allIds.SequenceEqual(ids3), Is.True, "Results should be identical");
        
        // Partial cache miss should be faster than initial but not as fast as full hit
        Assert.That(sw2.ElapsedMilliseconds, Is.LessThanOrEqualTo(sw1.ElapsedMilliseconds), 
                   "Partial cache miss should be faster than cold start");
    }

    /// <summary>
    /// Clear the debug info cache for testing purposes using the public API
    /// </summary>
    private static void ClearDebugInfoCache(LogicElement element)
    {
        element.ClearDebugInfoCacheForTesting();
    }
}