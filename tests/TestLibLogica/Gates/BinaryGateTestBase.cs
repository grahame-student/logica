using System;
using System.Linq;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Gates;

public abstract class BinaryGateTestBase<TGate> : LogicElementTestBase<TGate>
    where TGate : LogicElement, IBinaryGate, new()
{
    protected TGate _gate;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _gate = _element;
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

    protected void TestLogicOperation(Boolean a, Boolean b, Boolean expectedO)
    {
        _gate.A.Value = a;
        _gate.B.Value = b;

        _gate.Update();

        Assert.That(_gate.O.Value, Is.EqualTo(expectedO));
    }
}
