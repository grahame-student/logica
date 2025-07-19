using System;
using System.Linq;
using NUnit.Framework;

namespace TestLibLogica.Gates;

using LibLogica.Gates;
using LibLogica.IO;

public class TestNorGate3Input
{
    private NorGate3Input _gate;

    [SetUp]
    public void Setup()
    {
        _gate = new NorGate3Input();
    }

    [Test]
    public void InputAInitiallyFalse()
    {
        Assert.That(_gate.A.Value, Is.EqualTo(false));
    }

    [Test]
    public void InputBInitiallyFalse()
    {
        Assert.That(_gate.B.Value, Is.EqualTo(false));
    }

    [Test]
    public void InputCInitiallyFalse()
    {
        Assert.That(_gate.C.Value, Is.EqualTo(false));
    }

    public static Object[] UpdateTestCases =
    [ //               a,     b,     c,     expectedO
        new Object[] { false, false, false, true },
        new Object[] { false, false, true, false },
        new Object[] { false, true, false, false },
        new Object[] { false, true, true, false },
        new Object[] { true, false, false, false },
        new Object[] { true, false, true, false },
        new Object[] { true, true, false, false },
        new Object[] { true, true, true, false },
    ];

    [TestCaseSource(nameof(UpdateTestCases))]
    public void Update_SetsO_ToLogicalNorOfAAndBAndC(Boolean a, Boolean b, Boolean c, Boolean expectedO)
    {
        _gate.A.Value = a;
        _gate.B.Value = b;
        _gate.C.Value = c;

        _gate.Update();

        Assert.That(_gate.O.Value, Is.EqualTo(expectedO));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_gate.GetIds().Count(), Is.EqualTo(_gate.GetValues().Count()));
    }
}
