using System;
using System.Linq;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks;

public class TestHalfAdder
{
    private HalfAdder _block;

    [SetUp]
    public void Setup()
    {
        _block = new HalfAdder();
    }

    [Test]
    public void InputAInitiallyFalse()
    {
        Assert.That(_block.A.Value, Is.False);
    }

    [Test]
    public void InputBInitiallyFalse()
    {
        Assert.That(_block.B.Value, Is.False);
    }

    public static Object[] UpdateSumOutTestCases =
    [
        new Object[] { false, false, false },
        new Object[] { false, true, true },
        new Object[] { true, false, true },
        new Object[] { true, true, false }
    ];

    [TestCaseSource(nameof(UpdateSumOutTestCases))]
    public void Update_SetsSumOut_ToSumOfAAndB(Boolean a, Boolean b, Boolean expectedO)
    {
        _block.A.Value = a;
        _block.B.Value = b;

        _block.Update();

        Assert.That(_block.SumOut.Value, Is.EqualTo(expectedO));
    }

    public static Object[] UpdateCarryOutTestCases =
    [
        new Object[] { false, false, false },
        new Object[] { false, true, false },
        new Object[] { true, false, false },
        new Object[] { true, true, true }
    ];

    [TestCaseSource(nameof(UpdateCarryOutTestCases))]
    public void Update_SetsCarryOut_ToSumOfAAndB(Boolean a, Boolean b, Boolean expectedO)
    {
        _block.A.Value = a;
        _block.B.Value = b;

        _block.Update();

        Assert.That(_block.CarryOut.Value, Is.EqualTo(expectedO));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
