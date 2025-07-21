using NUnit.Framework;
using LibLogica.Gates;
using LibLogica.Blocks.Width8Bit;
using System;

[TestFixture]
public class DebugTristateTests
{
    [Test]
    public void Debug_TristateGate_EnableFalse()
    {
        var gate = new TristateBufferGate();
        
        // Initially high impedance
        Console.WriteLine($"Initial: IsHighImpedance={gate.O.IsHighImpedance}");
        
        // Enable it
        gate.Enable.Value = true;
        gate.Update();
        Console.WriteLine($"After enable true: IsHighImpedance={gate.O.IsHighImpedance}");
        
        // Disable it
        gate.Enable.Value = false;
        gate.Update();
        Console.WriteLine($"After enable false: IsHighImpedance={gate.O.IsHighImpedance}");
        
        Assert.That(gate.O.IsHighImpedance, Is.True);
    }
    
    [Test]
    public void Debug_TristateBuffer8Bit_EnableFalse()
    {
        var buffer = new TristateBuffer8Bit();
        
        // Set enable false and update
        buffer.Enable.Value = false;
        buffer.Update();
        
        Console.WriteLine($"8-bit buffer: Outputs[0].IsHighImpedance={((LibLogica.IO.Output)buffer.Outputs[0]).IsHighImpedance}");
        
        Assert.That(((LibLogica.IO.Output)buffer.Outputs[0]).IsHighImpedance, Is.True);
    }
}