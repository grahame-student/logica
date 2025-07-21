using System;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks;

internal class TestDecoder4to16 : LogicElementTestBase<Decoder4to16>
{
    [Test]
    public void Address_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Address), Is.Zero);
    }

    [Test]
    public void Output_Initially_AllZero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Output), Is.Zero);
    }

    public static readonly Object[] TestCaseData =
    [
        new Object[] { 0b0000u, 0b0000_0000_0000_0001u },
        new Object[] { 0b0001u, 0b0000_0000_0000_0010u },
        new Object[] { 0b0010u, 0b0000_0000_0000_0100u },
        new Object[] { 0b0011u, 0b0000_0000_0000_1000u },
        new Object[] { 0b0100u, 0b0000_0000_0001_0000u },
        new Object[] { 0b0101u, 0b0000_0000_0010_0000u },
        new Object[] { 0b0110u, 0b0000_0000_0100_0000u },
        new Object[] { 0b0111u, 0b0000_0000_1000_0000u },
        new Object[] { 0b1000u, 0b0000_0001_0000_0000u },
        new Object[] { 0b1001u, 0b0000_0010_0000_0000u },
        new Object[] { 0b1010u, 0b0000_0100_0000_0000u },
        new Object[] { 0b1011u, 0b0000_1000_0000_0000u },
        new Object[] { 0b1100u, 0b0001_0000_0000_0000u },
        new Object[] { 0b1101u, 0b0010_0000_0000_0000u },
        new Object[] { 0b1110u, 0b0100_0000_0000_0000u },
        new Object[] { 0b1111u, 0b1000_0000_0000_0000u }
    ];

    [TestCaseSource(nameof(TestCaseData))]
    public void Address_PassesWriteValue_ToActiveOutput(UInt32 address, UInt32 activeOutput)
    {
        LogicElementTestHelper.SetArrayValue(_element.Address, address);

        _element.Update();

        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Output), Is.EqualTo(activeOutput));
    }
}
