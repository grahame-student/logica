using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica;

/// <summary>
/// Base class providing comprehensive tests for GetIds() and GetValues() methods
/// of LogicElement implementations.
/// </summary>
public abstract class LogicElementTestBase<T> where T : LogicElement, new()
{
    /// <summary>
    /// Regex pattern that validates ID format: ClassName_Number.NestedClassName_Number.PropertyName
    /// Allows for nested components with dot separators and alphanumeric characters with underscores
    /// </summary>
    private const string ID_FORMAT_PATTERN = @"^[A-Za-z0-9_]+\.[A-Za-z0-9_.]*[A-Za-z0-9]+$";

    protected T _element;

    [SetUp]
    public virtual void Setup()
    {
        _element = new T();
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        var ids = _element.GetIds().ToList();
        var values = _element.GetValues().ToList();

        Assert.That(ids.Count, Is.EqualTo(values.Count),
            "GetIds() and GetValues() must return the same number of elements");
    }

    [Test]
    public void GetIds_ReturnsNonEmptyCollection()
    {
        var ids = _element.GetIds().ToList();

        Assert.That(ids, Is.Not.Empty,
            "GetIds() should return at least one element");
    }

    [Test]
    public void GetValues_ReturnsNonEmptyCollection()
    {
        var values = _element.GetValues().ToList();

        Assert.That(values, Is.Not.Empty,
            "GetValues() should return at least one element");
    }

    [Test]
    public void GetIds_AllIdsFollowCorrectFormat()
    {
        var ids = _element.GetIds().ToList();
        var expectedPattern = new Regex(ID_FORMAT_PATTERN);

        foreach (var id in ids)
        {
            Assert.That(expectedPattern.IsMatch(id), Is.True,
                $"ID '{id}' does not follow expected format with proper class prefixes and property names");
        }
    }

    [Test]
    public void GetIds_AllIdsAreUnique()
    {
        var ids = _element.GetIds().ToList();
        var uniqueIds = new HashSet<string>(ids);

        Assert.That(uniqueIds.Count, Is.EqualTo(ids.Count),
            "All IDs returned by GetIds() should be unique");
    }

    [Test]
    public void GetIds_ContainCorrectClassNamePrefix()
    {
        var ids = _element.GetIds().ToList();
        var expectedPrefix = _element.GetType().Name + "_";

        // All IDs should start with the main class name prefix
        // Some may also have nested component prefixes after that
        foreach (var id in ids)
        {
            Assert.That(id.StartsWith(expectedPrefix), Is.True,
                $"ID '{id}' should start with class name prefix '{expectedPrefix}'");
        }
    }

    [Test]
    public void GetIdsAndGetValues_MaintainConsistentOrder()
    {
        // Get multiple snapshots to ensure order consistency
        var ids1 = _element.GetIds().ToList();
        var values1 = _element.GetValues().ToList();
        var ids2 = _element.GetIds().ToList();
        var values2 = _element.GetValues().ToList();

        Assert.That(ids2, Is.EqualTo(ids1),
            "GetIds() should return IDs in consistent order across multiple calls");
        Assert.That(values2, Is.EqualTo(values1),
            "GetValues() should return values in consistent order across multiple calls");
    }

    [Test]
    public void GetValues_ReturnsOnlyBooleanValues()
    {
        var values = _element.GetValues().ToList();

        foreach (var value in values)
        {
            Assert.That(value, Is.TypeOf<bool>(),
                "GetValues() should only return boolean values");
        }
    }

    /// <summary>
    /// Test that IDs and values can be accessed consistently by the same index position.
    /// This ensures the collections maintain proper index-based correspondence.
    /// </summary>
    [Test]
    public void GetIdsAndGetValues_CorrespondByPosition()
    {
        var ids = _element.GetIds().ToList();
        var values = _element.GetValues().ToList();

        // Ensure they have the same count (already tested above, but critical for this test)
        Assert.That(ids.Count, Is.EqualTo(values.Count),
            "IDs and values must have the same count to correspond by position");

        // Verify that for each index i, we can consistently access both ids[i] and values[i]
        // and that they represent valid data
        for (int i = 0; i < ids.Count; i++)
        {
            Assert.That(ids[i], Is.Not.Null.And.Not.Empty,
                $"ID at position {i} should be valid");
            Assert.That(values[i], Is.TypeOf<bool>(),
                $"Value at position {i} should be a boolean");

            // Verify consistency: accessing the same index multiple times should yield the same results
            var idsAgain = _element.GetIds().ToList();
            var valuesAgain = _element.GetValues().ToList();
            Assert.That(idsAgain[i], Is.EqualTo(ids[i]),
                $"ID at position {i} should be consistent across multiple calls");
            Assert.That(valuesAgain[i], Is.EqualTo(values[i]),
                $"Value at position {i} should be consistent across multiple calls");
        }
    }
}
