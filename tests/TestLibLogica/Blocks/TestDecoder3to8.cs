using System;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks;

internal class TestDecoder3to8 : LogicElementTestBase<Decoder3to8>
{
    [Test]
    public void Address_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Address), Is.Zero);
    }

    [Test]
    public void Write_Initially_Zero()
    {
        Assert.That(_element.Write.Value, Is.False);
    }

    [Test]
    public void Output_Initially_AllZero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Output), Is.Zero);
    }

    public static readonly Object[] TestCaseData =
    [
        //            address, write, activeOutput
        new Object[] { 0b000u, true, 0b00000001u },
        new Object[] { 0b001u, true, 0b00000010u },
        new Object[] { 0b010u, true, 0b00000100u },
        new Object[] { 0b011u, true, 0b00001000u },
        new Object[] { 0b100u, true, 0b00010000u },
        new Object[] { 0b101u, true, 0b00100000u },
        new Object[] { 0b110u, true, 0b01000000u },
        new Object[] { 0b111u, true, 0b10000000u },
        new Object[] { 0b000u, false, 0b00000000u },
        new Object[] { 0b001u, false, 0b00000000u },
        new Object[] { 0b010u, false, 0b00000000u },
        new Object[] { 0b011u, false, 0b00000000u },
        new Object[] { 0b100u, false, 0b00000000u },
        new Object[] { 0b101u, false, 0b00000000u },
        new Object[] { 0b110u, false, 0b00000000u },
        new Object[] { 0b111u, false, 0b00000000u },
    ];

    [TestCaseSource(nameof(TestCaseData))]
    public void Address_PassesWriteValue_ToActiveOutput(UInt32 address, Boolean write, UInt32 activeOutput)
    {
        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        _element.Write.Value = write;

        _element.Update();

        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Output), Is.EqualTo(activeOutput));
    }
}
