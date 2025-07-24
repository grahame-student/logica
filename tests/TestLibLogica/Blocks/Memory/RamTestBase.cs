using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

/// <summary>
/// Base class providing optimized testing strategies for RAM blocks using professional testing techniques.
/// Designed to provide comprehensive coverage without exhaustive testing, making it feasible for larger RAM assemblies.
/// </summary>
public abstract class RamTestBase<T> : LogicElementTestBase<T> where T : LogicElement, new()
{
    /// <summary>
    /// Generate boundary value test cases for a given address space.
    /// Tests the edges and critical boundaries where errors are most likely to occur.
    /// </summary>
    protected static IEnumerable<UInt32> GenerateBoundaryAddresses(UInt32 maxAddress)
    {
        if (maxAddress == 0)
        {
            yield return 0;
            yield break;
        }

        // Boundary values: min, min+1, max-1, max
        yield return 0;                    // Minimum address
        if (maxAddress > 0) yield return 1;                    // Minimum + 1
        if (maxAddress > 1) yield return maxAddress - 1;      // Maximum - 1
        yield return maxAddress;           // Maximum address
    }

    /// <summary>
    /// Generate equivalence partition test cases for address space.
    /// Divides the address space into partitions and tests representative values from each.
    /// </summary>
    protected static IEnumerable<UInt32> GeneratePartitionAddresses(UInt32 maxAddress, Int32 partitionCount = 8)
    {
        if (maxAddress == 0)
        {
            yield return 0;
            yield break;
        }

        for (Int32 i = 0; i < partitionCount; i++)
        {
            // Cast to long to prevent integer overflow during calculation
            // Note: This works correctly even when maxAddress = UInt32.MaxValue
            // since (UInt32.MaxValue + 1L) = 4294967296L which fits in long
            UInt32 address = (UInt32)(i * (maxAddress + 1L) / partitionCount);
            yield return Math.Min(address, maxAddress);
        }
    }

    /// <summary>
    /// Generate power-of-2 and power-of-2 adjacent addresses for testing address decoding logic.
    /// These addresses are prone to errors in binary address decoding implementations.
    /// </summary>
    protected static IEnumerable<UInt32> GeneratePowerOfTwoAddresses(UInt32 maxAddress)
    {
        for (Int32 power = 0; power < 32; power++)
        {
            UInt32 powerOfTwo = 1u << power;
            if (powerOfTwo > maxAddress) break;

            yield return powerOfTwo;

            // Test adjacent values if they're within range
            // Skip powerOfTwo - 1 when powerOfTwo = 1 to avoid yielding address 0 twice
            if (powerOfTwo > 1) yield return powerOfTwo - 1;
            if (powerOfTwo < maxAddress) yield return powerOfTwo + 1;
        }
    }

    /// <summary>
    /// Generate a comprehensive set of test addresses using multiple professional testing techniques.
    /// Combines boundary testing, equivalence partitioning, and error-prone value testing.
    /// </summary>
    protected static IEnumerable<UInt32> GenerateOptimizedAddresses(UInt32 maxAddress)
    {
        HashSet<UInt32> addresses = new HashSet<UInt32>();

        // Boundary value testing
        foreach (UInt32 addr in GenerateBoundaryAddresses(maxAddress))
        {
            addresses.Add(addr);
        }

        // Equivalence partitioning
        foreach (UInt32 addr in GeneratePartitionAddresses(maxAddress))
        {
            addresses.Add(addr);
        }

        // Power of 2 testing for binary address decoding
        foreach (UInt32 addr in GeneratePowerOfTwoAddresses(maxAddress))
        {
            addresses.Add(addr);
        }

        return addresses.OrderBy(a => a);
    }

    /// <summary>
    /// Generate test data patterns that exercise different bit combinations.
    /// Uses patterns that are likely to reveal errors in data storage and retrieval.
    /// </summary>
    protected static IEnumerable<UInt32> GenerateTestDataPatterns()
    {
        yield return 0b00000000u;  // All zeros
        yield return 0b11111111u;  // All ones
        yield return 0b10101010u;  // Alternating 10101010
        yield return 0b01010101u;  // Alternating 01010101
        yield return 0b11001100u;  // 11001100
        yield return 0b00110011u;  // 00110011
        yield return 0b11110000u;  // 11110000
        yield return 0b00001111u;  // 00001111
    }

    /// <summary>
    /// Generate combined test cases for write operations using optimized addresses and data patterns.
    /// Returns tuples of (address, dataValue) for parameterized testing.
    /// </summary>
    protected static IEnumerable<Object[]> GenerateWriteTestCases(UInt32 maxAddress)
    {
        List<UInt32> addresses = GenerateOptimizedAddresses(maxAddress).ToList();
        List<UInt32> dataPatterns = GenerateTestDataPatterns().ToList();

        // Test each optimized address with a representative data pattern
        for (Int32 i = 0; i < addresses.Count; i++)
        {
            UInt32 address = addresses[i];
            UInt32 dataPattern = dataPatterns[i % dataPatterns.Count];
            yield return new Object[] { address, true, dataPattern, dataPattern };
        }
    }

    /// <summary>
    /// Generate combined test cases for read operations using optimized addresses and data patterns.
    /// Returns tuples of (address, expectedData) for parameterized testing.
    /// </summary>
    protected static IEnumerable<Object[]> GenerateReadTestCases(UInt32 maxAddress)
    {
        List<UInt32> addresses = GenerateOptimizedAddresses(maxAddress).ToList();
        List<UInt32> dataPatterns = GenerateTestDataPatterns().ToList();

        // Test reading from each optimized address with expected data pattern
        for (Int32 i = 0; i < addresses.Count; i++)
        {
            UInt32 address = addresses[i];
            UInt32 dataPattern = dataPatterns[i % dataPatterns.Count];
            yield return new Object[] { address, false, 0b00000000u, dataPattern };
        }
    }

    /// <summary>
    /// Helper method to initialize memory with known values for read testing.
    /// Writes test patterns to all addresses that will be tested.
    /// </summary>
    protected void InitializeMemoryForReading(UInt32 maxAddress, LogicArray<Input> address, Input write, LogicArray<Input> dataIn, Action updateAction)
    {
        List<UInt32> addresses = GenerateOptimizedAddresses(maxAddress).ToList();
        List<UInt32> dataPatterns = GenerateTestDataPatterns().ToList();

        for (Int32 i = 0; i < addresses.Count; i++)
        {
            UInt32 addr = addresses[i];
            UInt32 pattern = dataPatterns[i % dataPatterns.Count];

            LogicElementTestHelper.SetArrayValue(address, addr);
            LogicElementTestHelper.SetArrayValue(dataIn, pattern);
            write.Value = true;
            updateAction();
        }
    }

    /// <summary>
    /// Property-based test helper: Verify that writing and then reading from the same address returns the same data.
    /// This is a fundamental invariant that should always hold for any RAM implementation.
    /// </summary>
    protected void VerifyWriteReadInvariant(UInt32 testAddress, UInt32 testData, UInt32 maxAddress,
        LogicArray<Input> address, Input write, LogicArray<Input> dataIn, LogicArray<Output> dataOut, Action updateAction)
    {
        if (testAddress > maxAddress) return;

        // Write the test data
        LogicElementTestHelper.SetArrayValue(address, testAddress);
        LogicElementTestHelper.SetArrayValue(dataIn, testData);
        write.Value = true;
        updateAction();

        // Read it back
        write.Value = false;
        updateAction();

        UInt32 readData = LogicElementTestHelper.GetArrayValue(dataOut);
        Assert.That(readData, Is.EqualTo(testData & 0xFF),
            $"Write-read invariant failed: wrote {testData:X2} to address {testAddress}, read {readData:X2}");
    }

    /// <summary>
    /// Stress test helper: Perform rapid write-read cycles to test timing and state consistency.
    /// </summary>
    protected void PerformStressTest(UInt32 maxAddress, LogicArray<Input> address, Input write,
        LogicArray<Input> dataIn, LogicArray<Output> dataOut, Action updateAction, Int32 cycles = 100)
    {
        Random random = new Random(42); // Fixed seed for reproducible tests
        for (Int32 i = 0; i < cycles; i++)
        {
            UInt32 testAddress = (UInt32)random.Next(0, (Int32)maxAddress + 1);
            UInt32 testData = (UInt32)random.Next(0, 256);
            VerifyWriteReadInvariant(testAddress, testData, maxAddress, address, write, dataIn, dataOut, updateAction);
        }
    }
}
