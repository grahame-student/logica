using LibLogica.Blocks;

using System;
using NUnit.Framework;
using System.Linq;

namespace TestLibLogica.Blocks;

public class TestOscillator
{
    private Oscillator _block;

    [SetUp]
    public void Setup()
    {
        _block = new Oscillator();
    }

    [Test]
    public void OutputO_InitiallyFalse()
    {
        Assert.That(_block.O.Value, Is.EqualTo(false));
    }

    [Test]
    public void Update_InvertsO()
    {
        Boolean start = _block.O.Value;

        _block.Update();

        Assert.That(_block.O.Value, Is.Not.EqualTo(start));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
