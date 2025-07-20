using System;
using System.Linq;
using NUnit.Framework;

namespace TestLibLogica.Gates;

using LibLogica.Gates;

public class TestAndGate : LogicElementTestBase<AndGate>
{
    private AndGate _gate;

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

    public static Object[] UpdateTestCases =
    [
        new Object[] { false, false, false },
        new Object[] { false, true, false },
        new Object[] { true, false, false },
        new Object[] { true, true, true }
    ];

    [TestCaseSource(nameof(UpdateTestCases))]
    public void Update_SetsO_ToLogicalAndOfAAndB(Boolean a, Boolean b, Boolean expectedO)
    {
        _gate.A.Value = a;
        _gate.B.Value = b;

        _gate.Update();

        Assert.That(_gate.O.Value, Is.EqualTo(expectedO));
    }
}
