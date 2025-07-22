using System;
using LibLogica.Gates;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.IO;

internal class TestOutput
{
    private Output _output;

    [SetUp]
    public void Setup()
    {
        _output = new Output();
    }

    [Test]
    public void Value_RaisesSignalChangedEvent_WhenValueChanges()
    {
        Boolean wasRaised = false;
        _output.SignalChanged += (sender, args) => wasRaised = true;

        _output.Value = !_output.Value;

        Assert.That(wasRaised, Is.True);
    }

    [Test]
    public void Value_PassesNewValueInSignalChangedEvent_WhenValueChanges()
    {
        Boolean newValue = false;
        _output.SignalChanged += (sender, args) => newValue = args.Value;

        _output.Value = true;

        Assert.That(newValue, Is.True);
    }

    #region Tristate Buffer Multiple Sources Tests

    private class TristateBufferTestSetup
    {
        public TristateBufferGate Buffer1 { get; } = new();
        public TristateBufferGate Buffer2 { get; } = new();
        public Input Enable1 { get; } = new();
        public Input Enable2 { get; } = new();
        public Output Output { get; } = new();

        public TristateBufferTestSetup()
        {
            Buffer1.Enable.Connect(Enable1);
            Buffer2.Enable.Connect(Enable2);
            Buffer2.A.Value = true; // Set buffer2 input to true for testing

            Output.Connect(Buffer1.O);
            Output.Connect(Buffer2.O);
        }

        public void Update()
        {
            Buffer1.Update();
            Buffer2.Update();
        }
    }

    public static readonly Object[] TristateBufferTestCases =
    [
        new Object[] { true, false, false, "Buffer1 enabled, Buffer2 disabled" },
        new Object[] { false, true, false, "Buffer1 disabled, Buffer2 enabled" },
        new Object[] { false, false, true, "Both buffers disabled" },
    ];

    [TestCaseSource(nameof(TristateBufferTestCases))]
    public void Output_HighImpedanceState_WithMultipleTristateBuffers(Boolean enable1, Boolean enable2, Boolean expectedHighImpedance, String scenario)
    {
        var setup = new TristateBufferTestSetup();

        setup.Enable1.Value = enable1;
        setup.Enable2.Value = enable2;
        setup.Update();

        Assert.That(setup.Output.IsHighImpedance, Is.EqualTo(expectedHighImpedance), $"High impedance state incorrect for scenario: {scenario}");
    }

    [Test]
    public void Output_Value_WhenBuffer2EnabledWithTrueInput()
    {
        var setup = new TristateBufferTestSetup();

        setup.Enable1.Value = false;
        setup.Enable2.Value = true;
        setup.Update();

        Assert.That(setup.Output.Value, Is.True, "Output should be true when buffer2 is enabled (buffer2.A = true)");
    }

    [Test]
    public void Output_HighImpedanceState_RemainsStable_WhenDisabledBufferStateChanges()
    {
        var setup = new TristateBufferTestSetup();

        // Enable buffer1, disable buffer2
        setup.Enable1.Value = true;
        setup.Enable2.Value = false;
        setup.Update();

        // Change buffer2's enable state (but keep it disabled) - this should be a no-op
        setup.Enable2.Value = false;
        setup.Update();

        Assert.That(setup.Output.IsHighImpedance, Is.False, "Output should remain not high impedance when disabled buffer state changes");
    }

    public static readonly Object[] EnableOrderTestCases =
    [
        new Object[] { true, false, false, "Buffer1 enabled, buffer2 disabled" },
        new Object[] { true, true, false, "Both buffers enabled" },
        new Object[] { false, true, false, "Buffer1 disabled, buffer2 enabled" },
        new Object[] { false, false, true, "Both buffers disabled" }
    ];

    [TestCaseSource(nameof(EnableOrderTestCases))]
    public void Output_HighImpedanceState_HandlesEnableOrderChanges(Boolean enable1, Boolean enable2, Boolean expectedHighImpedance, String scenario)
    {
        var setup = new TristateBufferTestSetup();

        // Set the enable states in sequence to test order independence
        setup.Enable1.Value = enable1;
        setup.Enable2.Value = enable2;
        setup.Update();

        Assert.That(setup.Output.IsHighImpedance, Is.EqualTo(expectedHighImpedance), $"High impedance state incorrect for scenario: {scenario}");
    }

    #endregion
}
