using System;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks;

internal class TestSelector8to1 : LogicElementTestBase<Selector8to1>
{
    [Test]
    public void Address_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Address), Is.Zero);
    }

    [Test]
    public void Input_Initially_AllZero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Input), Is.Zero);
    }

    [Test]
    public void Output_Initially_Zero()
    {
        Assert.That(_element.Output.Value, Is.False);
    }

    public static readonly Object[] TestCaseData =
    [
        //            address, input, output
        new Object[] { 0b000u, 0b00000001u, true },
        new Object[] { 0b001u, 0b00000010u, true },
        new Object[] { 0b010u, 0b00000100u, true },
        new Object[] { 0b011u, 0b00001000u, true },
        new Object[] { 0b100u, 0b00010000u, true },
        new Object[] { 0b101u, 0b00100000u, true },
        new Object[] { 0b110u, 0b01000000u, true },
        new Object[] { 0b111u, 0b10000000u, true },
        new Object[] { 0b000u, 0b00000000u, false },
        new Object[] { 0b001u, 0b00000000u, false },
        new Object[] { 0b010u, 0b00000000u, false },
        new Object[] { 0b011u, 0b00000000u, false },
        new Object[] { 0b100u, 0b00000000u, false },
        new Object[] { 0b101u, 0b00000000u, false },
        new Object[] { 0b110u, 0b00000000u, false },
        new Object[] { 0b111u, 0b00000000u, false },
    ];

    [TestCaseSource(nameof(TestCaseData))]
    public void Address_PassesInputValue_ToOutput(UInt32 address, UInt32 input, Boolean output)
    {
        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        LogicElementTestHelper.SetArrayValue(_element.Input, input);

        _element.Update();

        Assert.That(_element.Output.Value, Is.EqualTo(output));
    }
}
