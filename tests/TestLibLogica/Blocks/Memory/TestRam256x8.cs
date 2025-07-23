using System;
using LibLogica.Blocks.Memory;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;

internal class TestRam256x8 : LogicElementTestBase<Ram256x8>
{
    [Test]
    public void DataIn_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataIn), Is.Zero);
    }

    [Test]
    public void DataOut_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.DataOut), Is.Zero);
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
    public void Address_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Address), Is.Zero);
    }
}
