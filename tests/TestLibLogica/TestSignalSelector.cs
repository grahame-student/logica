using System;
using LibLogica.Gates;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica;

/// <summary>
/// Test class to reproduce the tristate buffer issue described in the GitHub issue.
/// This demonstrates the problem where combining multiple tristate outputs can lead to
/// unexpected High Impedance states depending on the order of Enable being set/reset.
/// </summary>
internal class TestSignalSelector
{
    private class SignalSelector
    {
        private readonly TristateBufferGate _tsBuffer1 = new();
        private readonly TristateBufferGate _tsBuffer2 = new();

        public Input Enable1 { get; } = new();
        public Input Enable2 { get; } = new();

        public Output Op { get; } = new();

        public SignalSelector()
        {
            _tsBuffer1.Enable.Connect(Enable1);
            _tsBuffer2.Enable.Connect(Enable2);

            Op.Connect(_tsBuffer1.O);
            Op.Connect(_tsBuffer2.O);

            _tsBuffer2.A.Value = true;
        }

        public void Update()
        {
            _tsBuffer1.Update();
            _tsBuffer2.Update();
        }
    }

    private SignalSelector _selector;

    [SetUp]
    public void SetUp()
    {
        _selector = new SignalSelector();
    }

    [Test]
    public void Output_ShouldNotBeHighImpedance_WhenBuffer1Enabled()
    {
        // Arrange: Enable buffer1, keep buffer2 disabled
        _selector.Enable1.Value = true;
        _selector.Enable2.Value = false;
        
        // Act
        _selector.Update();
        
        // Assert: Output should not be high impedance since buffer1 is active
        Assert.That(_selector.Op.IsHighImpedance, Is.False, "Output should not be high impedance when buffer1 is enabled");
    }

    [Test]
    public void Output_ShouldNotBeHighImpedance_WhenBuffer2Enabled()
    {
        // Arrange: Enable buffer2, keep buffer1 disabled
        _selector.Enable1.Value = false;
        _selector.Enable2.Value = true;
        
        // Act
        _selector.Update();
        
        // Assert: Output should not be high impedance since buffer2 is active
        Assert.That(_selector.Op.IsHighImpedance, Is.False, "Output should not be high impedance when buffer2 is enabled");
        Assert.That(_selector.Op.Value, Is.True, "Output should be true when buffer2 is enabled (buffer2.A = true)");
    }

    [Test]
    public void Output_ShouldBeHighImpedance_WhenBothBuffersDisabled()
    {
        // Arrange: Disable both buffers
        _selector.Enable1.Value = false;
        _selector.Enable2.Value = false;
        
        // Act
        _selector.Update();
        
        // Assert: Output should be high impedance since no buffers are active
        Assert.That(_selector.Op.IsHighImpedance, Is.True, "Output should be high impedance when both buffers are disabled");
    }

    [Test]
    public void Output_ShouldRemainNotHighImpedance_WhenDisabledBufferStateChanges()
    {
        // Arrange: Enable buffer1, disable buffer2
        _selector.Enable1.Value = true;
        _selector.Enable2.Value = false;
        _selector.Update();
        
        // Verify initial state
        Assert.That(_selector.Op.IsHighImpedance, Is.False, "Initial state should not be high impedance");
        
        // Act: Change buffer2's enable state (but keep it disabled)
        _selector.Enable2.Value = false; // This should be a no-op, but might trigger the bug
        _selector.Update();
        
        // Assert: Output should still not be high impedance since buffer1 is still active
        Assert.That(_selector.Op.IsHighImpedance, Is.False, "Output should remain not high impedance when disabled buffer state changes");
    }

    [Test]
    public void Output_ShouldHandleEnableOrderChanges()
    {
        // Test the scenario described in the issue where order of enable changes matters
        
        // Scenario 1: Enable buffer1 first, then buffer2
        _selector.Enable1.Value = true;
        _selector.Update();
        Assert.That(_selector.Op.IsHighImpedance, Is.False, "Should not be high impedance with buffer1 enabled");
        
        _selector.Enable2.Value = true;
        _selector.Update();
        // Both enabled - this might be a bus conflict in real hardware, but for now we test current behavior
        
        // Scenario 2: Disable buffer1, keep buffer2 enabled
        _selector.Enable1.Value = false;
        _selector.Update();
        Assert.That(_selector.Op.IsHighImpedance, Is.False, "Should not be high impedance with buffer2 still enabled");
        
        // Scenario 3: Disable buffer2 as well
        _selector.Enable2.Value = false;
        _selector.Update();
        Assert.That(_selector.Op.IsHighImpedance, Is.True, "Should be high impedance when both buffers disabled");
    }
}