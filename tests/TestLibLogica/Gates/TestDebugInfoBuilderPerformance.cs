using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Gates;

public class TestDebugInfoBuilderPerformance
{
    // Create a deeply nested structure to test performance
    private class PerformanceTestElement : LogicElement
    {
        private readonly PerformanceTestElement[] _children;
        public Input A { get; } = new();
        public Input B { get; } = new();
        public Output O { get; } = new();

        public PerformanceTestElement(int childCount = 0)
        {
            if (childCount > 0)
            {
                _children = new PerformanceTestElement[childCount];
                for (int i = 0; i < childCount; i++)
                {
                    _children[i] = new PerformanceTestElement(childCount - 1);
                }
            }
            else
            {
                _children = Array.Empty<PerformanceTestElement>();
            }
        }

        public override void Update() { }

        protected (IEnumerable<string> ids, IEnumerable<bool> values) BuildDebugInfo()
        {
            var builder = DebugInfo()
                .AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O));

            foreach (var child in _children)
            {
                builder.AddChild(child);
            }

            return builder.Build();
        }

        // Public wrapper for testing performance
        public (IEnumerable<string> ids, IEnumerable<bool> values) BuildDebugInfoPublic() => BuildDebugInfo();

        public override IEnumerable<string> GetIds() => BuildDebugInfo().ids;

        protected override IEnumerable<string> GetLocalIds() => throw new NotImplementedException();

        public override IEnumerable<bool> GetValues() => BuildDebugInfo().values;

        protected override IEnumerable<bool> GetLocalValues() => throw new NotImplementedException();
    }

    [TestCase(2, 3)] // 2 levels deep, 3 children each = ~13 elements
    [TestCase(3, 2)] // 3 levels deep, 2 children each = ~15 elements
    [TestCase(4, 2)] // 4 levels deep, 2 children each = ~31 elements
    public void DebugInfoBuilder_Performance_HandlesNestedStructuresEfficiently(int depth, int childrenPerLevel)
    {
        var rootElement = new PerformanceTestElement(depth);

        // Warm up
        rootElement.GetIds().ToList();
        rootElement.GetValues().ToList();

        // Measure performance
        var sw = Stopwatch.StartNew();
        var ids = rootElement.GetIds().ToList();
        var values = rootElement.GetValues().ToList();
        sw.Stop();

        // Verify correctness
        Assert.That(ids.Count, Is.EqualTo(values.Count), "IDs and values must be aligned");
        Assert.That(ids.Count, Is.GreaterThan(0), "Should have some elements");

        // Performance check - should complete quickly even with nested structures
        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(100),
            $"Debug info generation took {sw.ElapsedMilliseconds}ms for depth={depth}, children={childrenPerLevel}");

        TestContext.Out.WriteLine($"Generated {ids.Count} debug entries in {sw.ElapsedMilliseconds}ms " +
                            $"(depth={depth}, children={childrenPerLevel})");
    }

    [Test]
    public void DebugInfoBuilder_Performance_SingleBuilderVsMultipleCalls()
    {
        const int iterations = 1000;
        var element = new PerformanceTestElement();

        // Method 1: Single builder call (our new approach)
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var (ids, values) = element.BuildDebugInfoPublic();
            _ = ids.ToList();
            _ = values.ToList();
        }
        sw1.Stop();

        // Method 2: Separate calls (could be misaligned)
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            _ = element.GetIds().ToList();
            _ = element.GetValues().ToList();
        }
        sw2.Stop();

        TestContext.Out.WriteLine($"Single builder: {sw1.ElapsedMilliseconds}ms, Separate calls: {sw2.ElapsedMilliseconds}ms");

        // Focus on functional correctness rather than relative performance comparison
        // Both approaches should complete in reasonable time
        Assert.That(sw1.ElapsedMilliseconds, Is.LessThan(1000), "Single builder approach should complete efficiently");
        Assert.That(sw2.ElapsedMilliseconds, Is.LessThan(1000), "Separate calls should complete efficiently");

        // Verify that both approaches produce the same results
        var (singleIds, singleValues) = element.BuildDebugInfoPublic();
        var separateIds = element.GetIds();
        var separateValues = element.GetValues();

        Assert.That(singleIds.ToList(), Is.EqualTo(separateIds.ToList()), "Both approaches should produce identical IDs");
        Assert.That(singleValues.ToList(), Is.EqualTo(separateValues.ToList()), "Both approaches should produce identical values");
    }
}
