using System;
using LibLogica.Blocks.Memory;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

internal class TestRam1x8 : LogicElementTestBase<Ram1x8>
{
    [Test]
    public void DataIn_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataIn), Is.Zero);
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

    [Test]
    public void DataOut_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.Zero);
    }

    [Test]
    public void Update_StoresDataIn_WhenWriteSet()
    {
        LogicElementTestHelper.SetArrayValue(_element.DataIn, 255u);
        _element.Write.Value = true;
        _element.Enable.Value = true;

        _element.Update();

        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.EqualTo(255));
    }

    [Test]
    public void Update_DoesNotStoreDataIn_WhenWriteClear()
    {
        LogicElementTestHelper.SetArrayValue(_element.DataIn, 255u);
        _element.Write.Value = false;
        _element.Enable.Value = true;

        _element.Update();

        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.Zero);
    }

    [Test]
    public void DataOut_IsHighImpedance_WhenEnableFalse()
    {
        _element.Enable.Value = false;

        _element.Update();

        Boolean isHighImpedance = true;
        for (Int32 i = 0; i < _element.DataOut.Count; i++)
        {
            isHighImpedance &= ((Output)_element.DataOut[i]).IsHighImpedance;
        }

        Assert.That(isHighImpedance, Is.True);
    }
}
