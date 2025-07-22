using System;
using LibLogica.Blocks.Base;
using LibLogica.Blocks.Width16Bit;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Width16Bit;

internal class TestSelectSignal16Bit : LogicElementTestBase<SelectSignal16Bit>
{
    [Test]
    public void Inputs_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Inputs), Is.Zero);
    }

    [Test]
    public void Signal_Initially_False()
    {
        Assert.That(_element.Signal.Value, Is.False);
    }

    [Test]
    public void Outputs_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Outputs), Is.Zero);
    }

    public static readonly Object[] TestCases =
    [
        //               Input,          Signal, ExpectedOutput
        new Object[] { 0b0000000000000001u, true, 0b0000000000000001u },
        new Object[] { 0b0000000000000010u, true, 0b0000000000000010u },
        new Object[] { 0b0000000000000100u, true, 0b0000000000000100u },
        new Object[] { 0b0000000000001000u, true, 0b0000000000001000u },
        new Object[] { 0b0000000000010000u, true, 0b0000000000010000u },
        new Object[] { 0b0000000000100000u, true, 0b0000000000100000u },
        new Object[] { 0b0000000001000000u, true, 0b0000000001000000u },
        new Object[] { 0b0000000010000000u, true, 0b0000000010000000u },
        new Object[] { 0b0000000100000000u, true, 0b0000000100000000u },
        new Object[] { 0b0000001000000000u, true, 0b0000001000000000u },
        new Object[] { 0b0000010000000000u, true, 0b0000010000000000u },
        new Object[] { 0b0000100000000000u, true, 0b0000100000000000u },
        new Object[] { 0b0001000000000000u, true, 0b0001000000000000u },
        new Object[] { 0b0010000000000000u, true, 0b0010000000000000u },
        new Object[] { 0b0100000000000000u, true, 0b0100000000000000u },
        new Object[] { 0b1000000000000000u, true, 0b1000000000000000u },
        new Object[] { 0b0000000000000001u, false, 0b0000000000000000u },
        new Object[] { 0b0000000000000010u, false, 0b0000000000000000u },
        new Object[] { 0b0000000000000100u, false, 0b0000000000000000u },
        new Object[] { 0b0000000000001000u, false, 0b0000000000000000u },
        new Object[] { 0b0000000000010000u, false, 0b0000000000000000u },
        new Object[] { 0b0000000000100000u, false, 0b0000000000000000u },
        new Object[] { 0b0000000001000000u, false, 0b0000000000000000u },
        new Object[] { 0b0000000010000000u, false, 0b0000000000000000u },
        new Object[] { 0b0000000100000000u, false, 0b0000000000000000u },
        new Object[] { 0b0000001000000000u, false, 0b0000000000000000u },
        new Object[] { 0b0000010000000000u, false, 0b0000000000000000u },
        new Object[] { 0b0000100000000000u, false, 0b0000000000000000u },
        new Object[] { 0b0001000000000000u, false, 0b0000000000000000u },
        new Object[] { 0b0010000000000000u, false, 0b0000000000000000u },
        new Object[] { 0b0100000000000000u, false, 0b0000000000000000u },
        new Object[] { 0b1000000000000000u, false, 0b0000000000000000u }
    ];

    [TestCaseSource(nameof(TestCases))]
    public void SelectSignal16Bit_Test(UInt32 input, Boolean signal, UInt32 expectedOutput)
    {
        LogicElementTestHelper.SetArrayValue(_element.Inputs, input);
        _element.Signal.Value = signal;

        _element.Update();

        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Outputs), Is.EqualTo(expectedOutput));
    }
}
