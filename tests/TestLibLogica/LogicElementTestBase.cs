using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibLogica.Gates;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica;

/// <summary>
/// Base class providing comprehensive tests for GetIds() and GetValues() methods
/// of LogicElement implementations.
/// </summary>
public abstract class LogicElementTestBase<T> where T : LogicElement, new()
{
    /// <summary>
    /// Pattern for validating the main class identifier: ClassName_Number
    /// </summary>
    private const String CLASS_NAME_PATTERN = @"^[A-Za-z][A-Za-z0-9]*_[0-9]+";
    /// <summary>
    /// Pattern for validating property names (alphanumeric, may have numbers)
    /// </summary>
    private const String PROPERTY_NAME_PATTERN = @"[A-Za-z][A-Za-z0-9]*$";

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

        foreach (String? id in ids)
        {
            ValidateIdFormat(id);
        }
    }

    /// <summary>
    /// Validates that an ID follows the expected format with clear error reporting.
    /// Supports both simple IDs (ClassName_Number.PropertyName) and nested IDs
    /// (ClassName_Number.NestedClass_Number.PropertyName) with arbitrary nesting depth.
    /// </summary>
    private static void ValidateIdFormat(String id)
    {
        Assert.That(id, Is.Not.Null.And.Not.Empty, $"ID should not be null or empty");

        String[] parts = id.Split('.');
        Assert.That(parts.Length, Is.GreaterThanOrEqualTo(2),
            $"ID '{id}' should have at least main class and property (format: ClassName_Number.PropertyName)");

        // First part should be the main class with number
        var mainClassMatch = Regex.Match(parts[0], CLASS_NAME_PATTERN);
        Assert.That(mainClassMatch.Success, Is.True,
            $"ID '{id}' should start with ClassName_Number format, but got '{parts[0]}'");

        // Last part should be a valid property name
        var propertyMatch = Regex.Match(parts[^1], PROPERTY_NAME_PATTERN);
        Assert.That(propertyMatch.Success, Is.True,
            $"ID '{id}' should end with a valid property name, but got '{parts[^1]}'");

        // Middle parts (if any) should be nested class identifiers
        for (Int32 i = 1; i < parts.Length - 1; i++)
        {
            var nestedClassMatch = Regex.Match(parts[i], CLASS_NAME_PATTERN);
            Assert.That(nestedClassMatch.Success, Is.True,
                $"ID '{id}' has invalid nested class identifier '{parts[i]}' - should follow ClassName_Number format");
        }
    }

    [Test]
    public void GetIds_AllIdsAreUnique()
    {
        var ids = _element.GetIds().ToList();
        var uniqueIds = new HashSet<String>(ids);

        Assert.That(uniqueIds.Count, Is.EqualTo(ids.Count),
            "All IDs returned by GetIds() should be unique");
    }

    [Test]
    public void GetIds_ContainCorrectClassNamePrefix()
    {
        var ids = _element.GetIds().ToList();
        String expectedPrefix = _element.GetType().Name + "_";

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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ids2, Is.EqualTo(ids1),
                    "GetIds() should return IDs in consistent order across multiple calls");
            Assert.That(values2, Is.EqualTo(values1),
                "GetValues() should return values in consistent order across multiple calls");
        }
    }

    [Test]
    public void GetValues_ReturnsOnlyBooleanValues()
    {
        var values = _element.GetValues().ToList();

        foreach (Boolean value in values)
        {
            Assert.That(value, Is.TypeOf<Boolean>(),
                "GetValues() should only return boolean values");
        }
    }

    /// <summary>
    /// Test that validates multiple aspects of ID and value correspondence:
    /// 1. IDs and values can be accessed consistently by the same index position
    /// 2. All IDs at each position are non-null and non-empty
    /// 3. All values at each position are valid boolean types
    /// 4. Collections maintain consistency across multiple calls
    /// This ensures proper index-based correspondence and data validity.
    /// </summary>
    [Test]
    public void GetIdsAndGetValues_CorrespondByPosition()
    {
        var ids = _element.GetIds().ToList();
        var values = _element.GetValues().ToList();

        // Ensure they have the same count (already tested above, but critical for this test)
        Assert.That(ids.Count, Is.EqualTo(values.Count),
            "IDs and values must have the same count to correspond by position");

        // Get second snapshots once to verify consistency across multiple calls
        var idsAgain = _element.GetIds().ToList();
        var valuesAgain = _element.GetValues().ToList();

        // Verify that for each index i, we can consistently access both ids[i] and values[i]
        // and that they represent valid data
        for (Int32 i = 0; i < ids.Count; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ids[i], Is.Not.Null.And.Not.Empty,
                            $"ID at position {i} should be valid");
                Assert.That(values[i], Is.TypeOf<Boolean>(),
                    $"Value at position {i} should be a boolean");

                // Verify consistency: accessing the same index multiple times should yield the same results
                Assert.That(idsAgain[i], Is.EqualTo(ids[i]),
                    $"ID at position {i} should be consistent across multiple calls");
                Assert.That(valuesAgain[i], Is.EqualTo(values[i]),
                    $"Value at position {i} should be consistent across multiple calls");
            }
        }
    }
}

/// <summary>
/// Shared validation helper for LogicElement testing.
/// </summary>
public static class LogicElementTestHelper
{
    /// <summary>
    /// Validates that GetIds() and GetValues() correspond by position for any LogicElement.
    /// This helper can be used by test classes that cannot inherit from LogicElementTestBase
    /// due to constructor constraints.
    /// </summary>
    public static void ValidateIdsAndValuesCorrespondence(LogicElement element, String? elementName = null)
    {
        var ids = element.GetIds().ToList();
        var values = element.GetValues().ToList();
        String name = elementName ?? element.GetType().Name;

        Assert.That(ids.Count, Is.EqualTo(values.Count),
            $"{name}: IDs and values must have the same count to correspond by position");

        // Verify that for each index i, we can consistently access both ids[i] and values[i]
        // and that they represent valid data
        for (Int32 i = 0; i < ids.Count; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ids[i], Is.Not.Null.And.Not.Empty,
                            $"{name}: ID at position {i} should be valid");
                Assert.That(values[i], Is.TypeOf<Boolean>(),
                    $"{name}: Value at position {i} should be a boolean");
            }
        }

        // Test consistency across multiple calls
        var idsAgain = element.GetIds().ToList();
        var valuesAgain = element.GetValues().ToList();

        for (Int32 i = 0; i < ids.Count; i++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(idsAgain[i], Is.EqualTo(ids[i]),
                            $"{name}: ID at position {i} should be consistent across multiple calls");
                Assert.That(valuesAgain[i], Is.EqualTo(values[i]),
                    $"{name}: Value at position {i} should be consistent across multiple calls");
            }
        }
    }

    public static void SetArrayValue<T>(LogicArray<T> array, UInt32 value) where T : IInputOutput, new()
    {
        for (Int32 i = 0; i < array.Count; i++)
        {
            array[i].Value = (value & (1u << i)) != 0;
        }
    }

    public static UInt32 GetArrayValue<T>(LogicArray<T> array) where T : IInputOutput, new()
    {
        UInt32 value = 0;
        for (Int32 i = 0; i < array.Count; i++)
        {
            if (array[i].Value)
            {
                value |= 1u << i;
            }
        }
        return value;
    }

}
