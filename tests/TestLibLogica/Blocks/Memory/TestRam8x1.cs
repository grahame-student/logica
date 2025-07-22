using System;
using LibLogica.Blocks.Memory;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

internal class TestRam8x1 : LogicElementTestBase<Ram8x1>
{
    [Test]
    public void Address_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Address), Is.Zero);
    }

    [Test]
    public void DataIn_Initially_False()
    {
        Assert.That(_element.DataIn.Value, Is.False);
    }

    [Test]
    public void Write_Initially_False()
    {
        Assert.That(_element.Write.Value, Is.False);
    }

    [Test]
    public void DataOut_Initially_False()
    {
        Assert.That(_element.DataOut.Value, Is.False);
    }

    public static readonly Object[] WriteTestCaseData =
    [
        //            address, write, dataIn, dataOut
        new Object[] {0b000u, true, 0b00000001u, true }, // Write to address 0
        new Object[] {0b001u, true, 0b00000010u, true },
        new Object[] {0b010u, true, 0b00000100u, true },
        new Object[] {0b011u, true, 0b00001000u, true },
        new Object[] {0b100u, true, 0b00010000u, true },
        new Object[] {0b101u, true, 0b00100000u, true },
        new Object[] {0b110u, true, 0b01000000u, true },
        new Object[] {0b111u, true, 0b10000000u, true }, // Write to address 7
        new Object[] {0b000u, true, 0b00000000u, false }, // Write zero to address 0
        new Object[] {0b001u, true, 0b00000000u, false },
        new Object[] {0b010u, true, 0b00000000u, false },
        new Object[] {0b011u, true, 0b00000000u, false },
        new Object[] {0b100u, true, 0b00000000u, false },
        new Object[] {0b101u, true, 0b00000000u, false },
        new Object[] {0b110u, true, 0b00000000u, false },
        new Object[] {0b111u, true, 0b00000000u, false }, // Write zero to address 7
    ];

    public static readonly Object[] ReadTestCaseData =
    [
        new Object[] {0b000u, false, 0b00000001u, true }, // Read from address 0
        new Object[] {0b001u, false, 0b00000010u, true },
        new Object[] {0b010u, false, 0b00000100u, true },
        new Object[] {0b011u, false, 0b00001000u, true },
        new Object[] {0b100u, false, 0b00010000u, true },
        new Object[] {0b101u, false, 0b00100000u, true },
        new Object[] {0b110u, false, 0b01000000u, true },
        new Object[] {0b111u, false, 0b10000000u, true }, // Read from address 7
        new Object[] {0b000u, false, 0b00000000u, false }, // Read zero from address 0
        new Object[] {0b001u, false, 0b00000000u, false },
        new Object[] {0b010u, false, 0b00000000u, false },
        new Object[] {0b011u, false, 0b00000000u, false },
        new Object[] {0b100u, false, 0b00000000u, false },
        new Object[] {0b101u, false, 0b00000000u, false },
        new Object[] {0b110u, false, 0b00000000u, false },
        new Object[] {0b111u, false, 0b00000000u, false }, // Read zero from address 7
    ];

    [TestCaseSource(nameof(WriteTestCaseData))]
    public void Write_UpdatesDataOut_ForAddress(UInt32 address, Boolean write, UInt32 dataIn, Boolean dataOut)
    {
        Boolean init = dataIn == 0;
        InitMemory(address, init);

        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        _element.Write.Value = write;
        _element.DataIn.Value = dataIn != 0;

        _element.Update();

        Assert.That(_element.DataOut.Value, Is.EqualTo(dataOut));
    }

    [TestCaseSource(nameof(ReadTestCaseData))]
    public void Address_Read(UInt32 address, Boolean write, UInt32 dataIn, Boolean dataOut)
    {
        Boolean init = dataIn != 0;
        InitMemory(address, init);

        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        _element.DataIn.Value = false;
        _element.Write.Value = false;

        _element.Update();

        Assert.That(_element.DataOut.Value, Is.EqualTo(dataOut));
    }

    /// <summary>Set memory precondition</summary>
    /// If we're testing writing a 1 to memory, then memory should be initialized to 0.
    /// If we're testing writing a 0 to memory, then memory should be initialized to 1.
    private void InitMemory(UInt32 address, Boolean initValue)
    {
        // No need to initialize if we're not writing a 1
        if (!initValue) return;

        LogicElementTestHelper.SetArrayValue(_element.Address, address);
        _element.DataIn.Value = true;
        _element.Write.Value = true;
        _element.Update(); // Perform the write operation
    }
}
