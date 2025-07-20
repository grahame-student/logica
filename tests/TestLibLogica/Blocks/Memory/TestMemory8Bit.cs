using System;
using System.Diagnostics.Metrics;
using LibLogica.Blocks.Memory;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

internal class TestMemory8Bit : LogicElementTestBase<Memory8Bit>
{
    [Test]
    public void DataIn_IntialValue_ShouldBeZero()
    {
        Assert.That(GetArrayValue(_element.DataIn), Is.Zero);
    }

    [Test]
    public void DataOut_InitialValue_ShouldBeZero()
    {
        Assert.That(GetArrayValue(_element.DataOut), Is.Zero);
    }

    [Test]
    public void WriteEnable_InitialValue_ShouldBeFalse()
    {
        Assert.That(_element.Write.Value, Is.False);
    }

    public static readonly Object[] WriteTestCases =
    [
        //             initial value, data_in, write, expected value
        new Object[] { 0b00000000u, 0b11111111u, false, 0b00000000u },
        new Object[] { 0b00000000u, 0b11111111u, true, 0b11111111u },
        new Object[] { 0b11111111u, 0b00000000u, false, 0b11111111u },
        new Object[] { 0b11111111u, 0b00000000u, true, 0b00000000u },
        new Object[] { 0b10101010u, 0b01010101u, false, 0b10101010u },
        new Object[] { 0b10101010u, 0b01010101u, true, 0b01010101u },
        new Object[] { 0b11110000u, 0b00001111u, false, 0b11110000u },
        new Object[] { 0b11110000u, 0b00001111u, true, 0b00001111u },
        new Object[] { 0b00001111u, 0b11110000u, false, 0b00001111u },
        new Object[] { 0b00001111u, 0b11110000u, true, 0b11110000u },
        new Object[] { 0b10101010u, 0b10101010u, false, 0b10101010u },
        new Object[] { 0b10101010u, 0b10101010u, true, 0b10101010u },
        new Object[] { 0b11111111u, 0b11111111u, false, 0b11111111u },
        new Object[] { 0b11111111u, 0b11111111u, true, 0b11111111u },
        new Object[] { 0b00000000u, 0b00000000u, false, 0b00000000u },
        new Object[] { 0b00000000u, 0b00000000u, true, 0b00000000u },
    ];

    [TestCaseSource(nameof(WriteTestCases))]
    public void DataOut_SetToDataIn_WhenWriteIsTrue(UInt32 init, UInt32 dataIn, Boolean write, UInt32 dataOut)
    {
        SetArrayValue(_element.DataIn, init);
        _element.Write.Value = true;
        _element.Update();

        SetArrayValue(_element.DataIn, dataIn);
        _element.Write.Value = write;
        _element.Update();

        Assert.That(GetArrayValue(_element.DataOut), Is.EqualTo(dataOut));
    }

    private void SetArrayValue<T>(LogicArray<T> array, UInt32 value) where T : IInputOutput, new()
    {
        for (Int32 i = 0; i < array.Count; i++)
        {
            array[i].Value = (value & (1u << i)) != 0;
        }
    }

    private UInt32 GetArrayValue<T>(LogicArray<T> array) where T : IInputOutput, new()
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
