using System;
using System.Linq;
using NUnit.Framework;

namespace TestLibLogica.Gates;

using LibLogica.Gates;

public class TestNotGate
{
    private NotGate _gate;

    [SetUp]
    public void Setup()
    {
        _gate = new NotGate();
    }

    [Test]
    public void InputAInitiallyFalse()
    {
        Assert.That(_gate.A.Value, Is.EqualTo(false));
    }

    public static Object[] UpdateTestCases =
    [
        new Object[] { false, true },
        new Object[] { true, false },
    ];

    [TestCaseSource((nameof(UpdateTestCases)))]
    public void Update_SetsO_ToLogicalNotOfA(Boolean a, Boolean expectedO)
    {
        _gate.A.Value = a;

        _gate.Update();

        Assert.That(_gate.O.Value, Is.EqualTo(expectedO));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_gate.GetIds().Count(), Is.EqualTo(_gate.GetValues().Count()));
    }
}
