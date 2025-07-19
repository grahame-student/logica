using System;
using System.Linq;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Gates;

public abstract class BinaryGateTestBase<TGate> where TGate : LogicElement, IBinaryGate, new()
{
    protected TGate _gate;

    [SetUp]
    public virtual void Setup()
    {
        _gate = new TGate();
    }

    [Test]
    public void InputAInitiallyFalse()
    {
        Assert.That(_gate.A.Value, Is.False);
    }

    [Test]
    public void InputBInitiallyFalse()
    {
        Assert.That(_gate.B.Value, Is.False);
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_gate.GetIds().Count(), Is.EqualTo(_gate.GetValues().Count()));
    }

    protected void TestLogicOperation(Boolean a, Boolean b, Boolean expectedO)
    {
        _gate.A.Value = a;
        _gate.B.Value = b;

        _gate.Update();

        Assert.That(_gate.O.Value, Is.EqualTo(expectedO));
    }
}
