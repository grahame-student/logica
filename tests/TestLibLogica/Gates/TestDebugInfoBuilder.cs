using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Gates;

public class TestDebugInfoBuilder
{
    // Test element with simple inputs/outputs
    private class SimpleTestElement : LogicElement
    {
        public Input A { get; } = new();
        public Input B { get; } = new();
        public Output O { get; } = new();

        public override void Update() { }

        protected (IEnumerable<string> ids, IEnumerable<bool> values) BuildDebugInfo() =>
            DebugInfo()
                .AddLocal(nameof(A), A)
                .AddLocal(nameof(B), B)
                .AddLocal(nameof(O), O)
                .Build();

        public override IEnumerable<string> GetIds() => BuildDebugInfo().ids;

        public override IEnumerable<bool> GetValues() => BuildDebugInfo().values;
    }

    // Test element with child elements
    private class CompositeTestElement : LogicElement
    {
        public Input A { get; } = new();
        public Output O { get; } = new();

        private readonly SimpleTestElement _child1 = new();
        private readonly SimpleTestElement _child2 = new();

        public override void Update() { }

        protected (IEnumerable<string> ids, IEnumerable<bool> values) BuildDebugInfo() =>
            DebugInfo()
                .AddLocal(nameof(A), A)
                .AddLocal(nameof(O), O)
                .AddChild(_child1)
                .AddChild(_child2)
                .Build();

        public override IEnumerable<string> GetIds() => BuildDebugInfo().ids;

        public override IEnumerable<bool> GetValues() => BuildDebugInfo().values;
    }

    // Test element using the combined Build() approach
    private class CombinedBuildTestElement : LogicElement
    {
        public Input A { get; } = new();
        public Input B { get; } = new();
        public Output O { get; } = new();

        public override void Update() { }

        protected (IEnumerable<string> ids, IEnumerable<bool> values) BuildDebugInfo() =>
            DebugInfo()
                .AddLocal(nameof(A), A)
                .AddLocal(nameof(B), B)
                .AddLocal(nameof(O), O)
                .Build();

        public override IEnumerable<string> GetIds() => BuildDebugInfo().ids;

        public override IEnumerable<bool> GetValues() => BuildDebugInfo().values;
    }
    private class ArrayTestElement : LogicElement
    {
        public LogicArray<Input> A { get; } = new(3);
        public Output O { get; } = new();

        public override void Update() { }

        protected (IEnumerable<string> ids, IEnumerable<bool> values) BuildDebugInfo() =>
            DebugInfo()
                .AddArray(nameof(A), A)
                .AddLocal(nameof(O), O)
                .Build();

        public override IEnumerable<string> GetIds() => BuildDebugInfo().ids;

        public override IEnumerable<bool> GetValues() => BuildDebugInfo().values;
    }

    [Test]
    public void DebugInfoBuilder_SimpleElement_GeneratesCorrectIdsAndValues()
    {
        var element = new SimpleTestElement();
        element.A.Value = true;
        element.B.Value = false;
        element.O.Value = true;

        var ids = element.GetIds().ToList();
        var values = element.GetValues().ToList();

        Assert.That(ids.Count, Is.EqualTo(3));
        Assert.That(values.Count, Is.EqualTo(3));
        Assert.That(ids.Count, Is.EqualTo(values.Count), "IDs and values must be aligned");

        // Check that IDs contain expected names
        Assert.That(ids[0], Does.EndWith("A"));
        Assert.That(ids[1], Does.EndWith("B"));
        Assert.That(ids[2], Does.EndWith("O"));

        // Check that values are correct
        Assert.That(values[0], Is.True);  // A
        Assert.That(values[1], Is.False); // B
        Assert.That(values[2], Is.True);  // O
    }

    [Test]
    public void DebugInfoBuilder_CompositeElement_GeneratesCorrectIdsAndValues()
    {
        var element = new CompositeTestElement();
        element.A.Value = true;
        element.O.Value = false;

        var ids = element.GetIds().ToList();
        var values = element.GetValues().ToList();

        Assert.That(ids.Count, Is.EqualTo(values.Count), "IDs and values must be aligned");
        Assert.That(ids.Count, Is.EqualTo(8), "Should have 2 local + 3 child1 + 3 child2 = 8 items");

        // Check that local items come first
        Assert.That(ids[0], Does.EndWith("A"));
        Assert.That(ids[1], Does.EndWith("O"));
        Assert.That(values[0], Is.True);  // A
        Assert.That(values[1], Is.False); // O

        // Check that child prefixes are applied
        var childIds = ids.Skip(2).ToList();
        Assert.That(childIds.All(id => id.Contains("SimpleTestElement_")), "Child IDs should be properly prefixed");
    }

    [Test]
    public void DebugInfoBuilder_ArrayElement_GeneratesCorrectIdsAndValues()
    {
        var element = new ArrayTestElement();
        element.A[0].Value = true;
        element.A[1].Value = false;
        element.A[2].Value = true;
        element.O.Value = false;

        var ids = element.GetIds().ToList();
        var values = element.GetValues().ToList();

        Assert.That(ids.Count, Is.EqualTo(4));
        Assert.That(values.Count, Is.EqualTo(4));
        Assert.That(ids.Count, Is.EqualTo(values.Count), "IDs and values must be aligned");

        // Check array ordering (should be reverse: A2, A1, A0)
        Assert.That(ids[0], Does.EndWith("A2"));
        Assert.That(ids[1], Does.EndWith("A1"));
        Assert.That(ids[2], Does.EndWith("A0"));
        Assert.That(ids[3], Does.EndWith("O"));

        // Check values in corresponding order
        Assert.That(values[0], Is.True);  // A[2]
        Assert.That(values[1], Is.False); // A[1]
        Assert.That(values[2], Is.True);  // A[0]
        Assert.That(values[3], Is.False); // O
    }

    [Test]
    public void DebugInfoBuilder_Build_ReturnsSameResultsAsIndividualBuilders()
    {
        var element = new SimpleTestElement();
        element.A.Value = true;
        element.B.Value = false;
        element.O.Value = true;

        // Test via the public interface methods that use the builder internally
        var ids = element.GetIds().ToList();
        var values = element.GetValues().ToList();

        Assert.That(ids.Count, Is.EqualTo(values.Count), "IDs and values must be aligned");
        Assert.That(ids.Count, Is.EqualTo(3));
    }

    [Test]
    public void DebugInfoBuilder_CombinedBuild_EnsuresIdValueAlignment()
    {
        var element = new CombinedBuildTestElement();
        element.A.Value = true;
        element.B.Value = false;
        element.O.Value = true;

        var ids = element.GetIds().ToList();
        var values = element.GetValues().ToList();

        Assert.That(ids.Count, Is.EqualTo(3));
        Assert.That(values.Count, Is.EqualTo(3));
        Assert.That(ids.Count, Is.EqualTo(values.Count), "IDs and values must be aligned");

        // Verify the combined approach gives same results as separate calls
        Assert.That(values[0], Is.True);  // A
        Assert.That(values[1], Is.False); // B
        Assert.That(values[2], Is.True);  // O
    }

    [Test]
    public void DebugInfoBuilder_AddChildren_WorksWithMultipleElements()
    {
        var composite = new CompositeTestElement();
        composite.A.Value = true;
        composite.O.Value = false;

        var ids = composite.GetIds().ToList();
        var values = composite.GetValues().ToList();

        Assert.That(ids.Count, Is.EqualTo(8)); // 2 local + 3 child1 + 3 child2
        Assert.That(ids.Count, Is.EqualTo(values.Count), "IDs and values must be aligned");
    }
}
