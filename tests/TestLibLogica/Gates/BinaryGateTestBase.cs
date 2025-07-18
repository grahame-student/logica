using System;
using NUnit.Framework;
using System.Linq;
using LibLogica.Gates;

namespace TestLibLogica.Gates;

public abstract class BinaryGateTestBase<TGate> where TGate : LogicElement, new()
{
    protected TGate _gate = null!;

    [SetUp]
    public virtual void Setup()
    {
        _gate = new TGate();
    }

    [Test]
    public void InputAInitiallyFalse()
    {
        var property = typeof(TGate).GetProperty("A");
        Assert.That(property, Is.Not.Null, "Gate should have an A input");
        var input = property!.GetValue(_gate) as dynamic;
        Assert.That(input!.Value, Is.EqualTo(false));
    }

    [Test]
    public void InputBInitiallyFalse()
    {
        var property = typeof(TGate).GetProperty("B");
        Assert.That(property, Is.Not.Null, "Gate should have a B input");
        var input = property!.GetValue(_gate) as dynamic;
        Assert.That(input!.Value, Is.EqualTo(false));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_gate.GetIds().Count(), Is.EqualTo(_gate.GetValues().Count()));
    }

    protected void TestLogicOperation(Boolean a, Boolean b, Boolean expectedO)
    {
        var aProperty = typeof(TGate).GetProperty("A");
        var bProperty = typeof(TGate).GetProperty("B");
        var oProperty = typeof(TGate).GetProperty("O");

        var aInput = aProperty!.GetValue(_gate) as dynamic;
        var bInput = bProperty!.GetValue(_gate) as dynamic;
        var output = oProperty!.GetValue(_gate) as dynamic;

        aInput!.Value = a;
        bInput!.Value = b;

        _gate.Update();

        Assert.That(output!.Value, Is.EqualTo(expectedO));
    }
}
