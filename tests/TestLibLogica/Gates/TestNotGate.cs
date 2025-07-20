using System;
using System.Linq;
using NUnit.Framework;

namespace TestLibLogica.Gates;

using LibLogica.Gates;

public class TestNotGate : LogicElementTestBase<NotGate>
{
    private NotGate _gate;

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

    public static Object[] UpdateTestCases =
    [
        new Object[] { false, true },
        new Object[] { true, false },
    ];

    [TestCaseSource(nameof(UpdateTestCases))]
    public void Update_SetsO_ToLogicalNotOfA(Boolean a, Boolean expectedO)
    {
        _gate.A.Value = a;

        _gate.Update();

        Assert.That(_gate.O.Value, Is.EqualTo(expectedO));
    }
}
