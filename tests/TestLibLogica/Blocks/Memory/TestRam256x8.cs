using System;
using System.Linq;
using LibLogica.Blocks.Memory;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

internal class TestRam256x8 : RamTestBase<Ram256x8>
{
    private const UInt32 MAX_ADDRESS = 255;

    [Test]
    public void DataIn_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataIn), Is.Zero);
    }

    [Test]
    public void DataOut_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.Zero);
    }

    [Test]
    public void Write_Initially_False()
    {
        Assert.That(_element.Write.Value, Is.False);
    }

    [Test]
    public void Enable_Initially_False()
    {
        Assert.That(_element.Enable.Value, Is.False);
    }

    [Test]
    public void Address_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Address), Is.Zero);
    }

    public static readonly Object[] OptimizedWriteTestCaseData = GenerateWriteTestCases(MAX_ADDRESS).ToArray();

    public static readonly Object[] OptimizedReadTestCaseData = GenerateReadTestCases(MAX_ADDRESS).ToArray();

    [TestCaseSource(nameof(OptimizedWriteTestCaseData))]
    public void Address_WriteOptimized(UInt32 address, Boolean write, UInt32 dataIn, UInt32 dataOut)
    {
        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        _element.Write.Value = write;
        _element.Enable.Value = true;
        LogicElementTestHelper.SetArrayValue(_element.DataIn, dataIn);

        _element.Update();

        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.EqualTo(dataOut));
    }

    [TestCaseSource(nameof(OptimizedReadTestCaseData))]
    public void Address_ReadOptimized(UInt32 address, Boolean write, UInt32 dataIn, UInt32 dataOut)
    {
        InitializeMemoryForReading(MAX_ADDRESS, _element.Address, _element.Write, _element.DataIn, () => _element.Update());

        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        LogicElementTestHelper.SetArrayValue(_element.DataIn, dataIn);
        _element.Write.Value = false;
        _element.Enable.Value = true;

        _element.Update();

        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.EqualTo(dataOut));
    }

    [Test]
    public void WriteReadInvariant_RandomSampling()
    {
        // Property-based test: writing and then reading should return the same value
        var testCases = new[]
        {
            new { Address = 0u, Data = 0x42u },
            new { Address = 255u, Data = 0xAAu },
            new { Address = 128u, Data = 0x55u },
            new { Address = 64u, Data = 0xCCu },
            new { Address = 192u, Data = 0x33u }
        };

        foreach (var testCase in testCases)
        {
            _element.Enable.Value = true;
            VerifyWriteReadInvariant(testCase.Address, testCase.Data, MAX_ADDRESS,
                _element.Address, _element.Write, _element.DataIn, _element.DataOut,
                () => _element.Update());
        }
    }

    [Test]
    public void StressTest_RandomAddressDataCombinations()
    {
        _element.Enable.Value = true;
        PerformStressTest(MAX_ADDRESS, _element.Address, _element.Write,
            _element.DataIn, _element.DataOut, () => _element.Update(), cycles: 50);
    }

    [Test]
    public void Enable_DisablesOutput_WhenFalse()
    {
        // Write some data first
        LogicElementTestHelper.SetArrayValue(_element.Address, 0u);
        LogicElementTestHelper.SetArrayValue(_element.DataIn, 0xFFu);
        _element.Write.Value = true;
        _element.Enable.Value = true;
        _element.Update();

        // Now disable and try to read
        _element.Write.Value = false;
        _element.Enable.Value = false;
        _element.Update();

        // Output should be in high impedance state
        Boolean isHighImpedance = true;
        for (Int32 i = 0; i < _element.DataOut.Count; i++)
        {
            isHighImpedance &= ((LibLogica.IO.Output)_element.DataOut[i]).IsHighImpedance;
        }

        Assert.That(isHighImpedance, Is.True);
    }
}
