using System;
using LibLogica.Blocks.Memory;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

internal class TestRam16x8 : LogicElementTestBase<Ram16x8>
{
    [Test]
    public void DataIn_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataIn), Is.Zero);
    }

    [Test]
    public void Address_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Address), Is.Zero);
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

    public static readonly Object[] WriteToAddressTestCases =
    [
        //             address, data
        new Object[] { 0b0000u, 0b00000001u },
        new Object[] { 0b0001u, 0b00000010u },
        new Object[] { 0b0010u, 0b00000100u },
        new Object[] { 0b0011u, 0b00001000u },
        new Object[] { 0b0100u, 0b00010000u },
        new Object[] { 0b0101u, 0b00100000u },
        new Object[] { 0b0110u, 0b01000000u },
        new Object[] { 0b0111u, 0b10000000u },
        new Object[] { 0b1000u, 0b11111110u },
        new Object[] { 0b1001u, 0b11111101u },
        new Object[] { 0b1010u, 0b11111011u },
        new Object[] { 0b1011u, 0b11110111u },
        new Object[] { 0b1100u, 0b11101111u },
        new Object[] { 0b1101u, 0b11011111u },
        new Object[] { 0b1110u, 0b10111111u },
        new Object[] { 0b1111u, 0b01111111u }
    ];

    [TestCaseSource(nameof(WriteToAddressTestCases))]
    public void Write_StoresData_AtAddress(UInt32 address, UInt32 data)
    {
        // Arrange
        LogicElementTestHelper.SetArrayValue(_element.DataIn, data);
        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        _element.Write.Value = true;
        _element.Enable.Value = true;

        // Act
        _element.Update();

        // Assert
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.EqualTo(data));
    }

    [Test]
    public void Write_DisablesDataOut_WhenWriteIsFalse()
    {
        // Arrange
        LogicElementTestHelper.SetArrayValue(_element.DataIn, 0b11111111u);
        LogicElementTestHelper.SetArrayValue(_element.Address, 0b0000u);
        _element.Write.Value = false;
        _element.Enable.Value = true;

        // Act
        _element.Update();

        // Assert
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.Zero);
    }

    [Test]
    public void Enable_DisablesDataOut_WhenFalse()
    {
        // Arrange
        LogicElementTestHelper.SetArrayValue(_element.DataIn, 0b11111111u);
        LogicElementTestHelper.SetArrayValue(_element.Address, 0b0000u);
        _element.Write.Value = true;
        _element.Enable.Value = false;

        // Act
        _element.Update();

        // Assert
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.Zero);
    }

    [Test]
    public void Enable_SetsDataOutToHighImpedance_WhenFalse()
    {
        // Arrange
        LogicElementTestHelper.SetArrayValue(_element.DataIn, 0b11111111u);
        LogicElementTestHelper.SetArrayValue(_element.Address, 0b0000u);
        _element.Write.Value = true;
        _element.Enable.Value = false;

        // Act
        _element.Update();

        // Assert
        Boolean allPinsHighImpedance = _element.DataOut.All(pin => ((Output)pin).IsHighImpedance);
        Assert.That(isHighImpedance, Is.True);
    }

    [Test]
    public void Enable_EnablesDataOut_WhenTrue()
    {
        // Arrange
        LogicElementTestHelper.SetArrayValue(_element.DataIn, 0b11111111u);
        LogicElementTestHelper.SetArrayValue(_element.Address, 0b0000u);
        _element.Write.Value = true;
        _element.Enable.Value = true;

        // Act
        _element.Update();

        // Assert
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.EqualTo(0b11111111u));
    }
}
