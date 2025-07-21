using System;
using LibLogica.Blocks.Memory;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;
internal class TestMemory1x8 : LogicElementTestBase<Memory1x8>
{
    [Test]
    public void DataIn_Initially_False()
    {
        Assert.That(_element.DataIn.Value, Is.False);
    }

    [Test]
    public void Write_Initially_AllFalse()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Write), Is.Zero);
    }

    [Test]
    public void DataOut_Initially_AllFalse()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.Zero);
    }

    public static readonly Object[] TestCaseData =
    [
        //            dataIn, write, dataOut
        new Object[] {true, 0b00000001u, 0b00000001u },
        new Object[] {true, 0b00000010u, 0b00000010u },
        new Object[] {true, 0b00000100u, 0b00000100u },
        new Object[] {true, 0b00001000u, 0b00001000u },
        new Object[] {true, 0b00010000u, 0b00010000u },
        new Object[] {true, 0b00100000u, 0b00100000u },
        new Object[] {true, 0b01000000u, 0b01000000u },
        new Object[] {true, 0b10000000u, 0b10000000u },
        new Object[] {false, 0b00000001u, 0b11111110u },
        new Object[] {false, 0b00000010u, 0b11111101u },
        new Object[] {false, 0b00000100u, 0b11111011u },
        new Object[] {false, 0b00001000u, 0b11110111u },
        new Object[] {false, 0b00010000u, 0b11101111u },
        new Object[] {false, 0b00100000u, 0b11011111u },
        new Object[] {false, 0b01000000u, 0b10111111u },
        new Object[] {false, 0b10000000u, 0b01111111u },
    ];

    [TestCaseSource(nameof(TestCaseData))]
    public void DataIn_PassesWriteValue_ToDataOut(Boolean dataIn, UInt32 write, UInt32 dataOut)
    {
        InitMemory(dataIn);

        _element.DataIn.Value = dataIn;
        LogicElementTestHelper.SetArrayValue(_element.Write, write);
        _element.Update();
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.EqualTo(dataOut));
    }

    private void InitMemory(Boolean dataIn)
    {
        if (dataIn) return;

        // If value to write is false, then initialize the memory with all ones
        _element.DataIn.Value = true;
        LogicElementTestHelper.SetArrayValue(_element.Write, 0b11111111u);
        _element.Update();
    }
}
