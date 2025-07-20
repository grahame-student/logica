using System;
using System.Linq;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks;

public class TestOscillator : LogicElementTestBase<Oscillator>
{
    private Oscillator _block;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _block = _element;
    }

    [Test]
    public void OutputO_InitiallyFalse()
    {
        Assert.That(_block.O.Value, Is.False);
    }

    [Test]
    public void Update_InvertsO()
    {
        Boolean start = _block.O.Value;

        _block.Update();

        Assert.That(_block.O.Value, Is.Not.EqualTo(start));
    }
}
