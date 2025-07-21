using System;
using LibLogica.Blocks.Memory;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

internal class TestRam8x8 : LogicElementTestBase<Ram8x8>
{
    [Test]
    public void Address_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Address), Is.Zero);
    }

    [Test]
    public void DataIn_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataIn), Is.Zero);
    }

    [Test]
    public void Write_Initially_False()
    {
        Assert.That(_element.Write.Value, Is.False);
    }

    [Test]
    public void DataOut_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.Zero);
    }

    public static readonly Object[] WriteTestCaseData =
    [
        //            address, write, dataIn, dataOut
        new Object[] {0b000u, true, 0b10101010u, 0b10101010u }, // Write to address 0
        new Object[] {0b001u, true, 0b01010101u, 0b01010101u },
        new Object[] {0b010u, true, 0b00110011u, 0b00110011u },
        new Object[] {0b011u, true, 0b11001100u, 0b11001100u },
        new Object[] {0b100u, true, 0b00001111u, 0b00001111u },
        new Object[] {0b101u, true, 0b11110000u, 0b11110000u },
        new Object[] {0b110u, true, 0b11100011u, 0b11100011u },
        new Object[] {0b111u, true, 0b00011100u, 0b00011100u }, // Write to address 7
    ];

    public static readonly Object[] ReadTestCaseData =
    [
        new Object[] {0b000u, false, 0b00000000u, 0b10101010u }, // Read from address 0
        new Object[] {0b001u, false, 0b00000000u, 0b01010101u },
        new Object[] {0b010u, false, 0b00000000u, 0b00110011u },
        new Object[] {0b011u, false, 0b00000000u, 0b11001100u },
        new Object[] {0b100u, false, 0b00000000u, 0b00001111u },
        new Object[] {0b101u, false, 0b00000000u, 0b11110000u },
        new Object[] {0b110u, false, 0b00000000u, 0b11100011u },
        new Object[] {0b111u, false, 0b00000000u, 0b00011100u }, // Read from address 7
    ];

    [TestCaseSource(nameof(WriteTestCaseData))]
    public void Address_Write(UInt32 address, Boolean write, UInt32 dataIn, UInt32 dataOut)
    {
        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        _element.Write.Value = write;
        LogicElementTestHelper.SetArrayValue(_element.DataIn, dataIn);

        _element.Update();

        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.EqualTo(dataOut));
    }

    [TestCaseSource(nameof(ReadTestCaseData))]
    public void Address_Read(UInt32 address, Boolean write, UInt32 dataIn, UInt32 dataOut)
    {
        InitMemory(address, dataOut);

        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        LogicElementTestHelper.SetArrayValue(_element.DataIn, dataIn);
        _element.Write.Value = false;

        _element.Update();

        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.EqualTo(dataOut));
    }

    /// <summary>Set memory precondition</summary>
    /// Write some known values to the memory before running read test.
    private void InitMemory(UInt32 address, UInt32 dataIn)
    {
        // Write known poison values to all addresses, to help debugging if test fails
        for (UInt32 i = 0; i < 8; i++)
        {
            LogicElementTestHelper.SetArrayValue(_element.Address, i);
            _element.Write.Value = true;
            LogicElementTestHelper.SetArrayValue(_element.DataIn, i);
            _element.Update();
        }
        // Set the dataIn to the value we want to read in the test
        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        LogicElementTestHelper.SetArrayValue(_element.DataIn, dataIn);
        _element.Update();

    }
}
