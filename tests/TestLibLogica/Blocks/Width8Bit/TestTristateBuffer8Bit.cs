using System;
using LibLogica.Blocks.Width8Bit;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Width8Bit;

internal class TestTristateBuffer8Bit : LogicElementTestBase<TristateBuffer8Bit>
{
    [Test]
    public void Inputs_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Inputs), Is.Zero);
    }

    [Test]
    public void Enable_Initially_False()
    {
        Assert.That(_element.Enable.Value, Is.False);
    }

    [Test]
    public void Outputs_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Outputs), Is.Zero);
    }

    public static readonly Object[] OutputTestCaseData =
    [
        //           bit, enable, inputs, outputs
        new Object[] { 0u, true, 0b00000001u, 0b00000001u },
        new Object[] { 1u, true, 0b00000010u, 0b00000010u },
        new Object[] { 2u, true, 0b00000100u, 0b00000100u },
        new Object[] { 3u, true, 0b00001000u, 0b00001000u },
        new Object[] { 4u, true, 0b00010000u, 0b00010000u },
        new Object[] { 5u, true, 0b00100000u, 0b00100000u },
        new Object[] { 6u, true, 0b01000000u, 0b01000000u },
        new Object[] { 7u, true, 0b10000000u, 0b10000000u },
        new Object[] { 0u, false, 0b00000001u, 0b00000000u },
        new Object[] { 1u, false, 0b00000010u, 0b00000000u },
        new Object[] { 2u, false, 0b00000100u, 0b00000000u },
        new Object[] { 3u, false, 0b00001000u, 0b00000000u },
        new Object[] { 4u, false, 0b00010000u, 0b00000000u },
        new Object[] { 5u, false, 0b00100000u, 0b00000000u },
        new Object[] { 6u, false, 0b01000000u, 0b00000000u },
        new Object[] { 7u, false, 0b10000000u, 0b00000000u }

    ];

    [TestCaseSource(nameof(OutputTestCaseData))]
    public void Output_SetToInput_WhenEnableTrue(UInt32 bit, Boolean enable, UInt32 inputs, UInt32 outputs)
    {
        _element.Enable.Value = enable;
        LogicElementTestHelper.SetArrayValue(_element.Inputs, inputs);
        _element.Update();
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Outputs), Is.EqualTo(outputs));
    }

    public static readonly Object[] EnableTestCaseData =
    [
        //            bit, enable, isHighImpedance
        new Object[] { 0, true, false },
        new Object[] { 1, true, false },
        new Object[] { 2, true, false },
        new Object[] { 3, true, false },
        new Object[] { 4, true, false },
        new Object[] { 5, true, false },
        new Object[] { 6, true, false },
        new Object[] { 7, true, false },
        new Object[] { 0, false, true },
        new Object[] { 1, false, true },
        new Object[] { 2, false, true },
        new Object[] { 3, false, true },
        new Object[] { 4, false, true },
        new Object[] { 5, false, true },
        new Object[] { 6, false, true },
        new Object[] { 7, false, true }
    ];

    [TestCaseSource(nameof(EnableTestCaseData))]
    public void Output_HighImpedance_WhenEnableFalse(Int32 bit, Boolean enable, Boolean isHighImpedance)
    {
        _element.Enable.Value = enable;

        _element.Update();

        // TODO: This a bit of a hack to access the IsHighImpedance property
        //       which is hidden by the IInputOutput interface.
        Boolean result = ((Output)_element.Outputs[bit]).IsHighImpedance;
        Assert.That(result, Is.EqualTo(isHighImpedance));
    }
}
