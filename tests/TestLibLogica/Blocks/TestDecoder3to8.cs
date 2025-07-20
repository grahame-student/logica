using System;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks;

internal class TestDecoder3to8 : LogicElementTestBase<Decoder3to8>
{
    [Test]
    public void Address_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Address), Is.Zero);
    }

    [Test]
    public void Write_Initially_Zero()
    {
        Assert.That(_element.Write.Value, Is.False);
    }

    [Test]
    public void Output_Initially_AllZero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Output), Is.Zero);
    }
}
