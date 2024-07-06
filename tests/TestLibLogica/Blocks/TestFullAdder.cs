namespace TestLibLogica.Blocks;

using LibLogica.Blocks;

public class TestFullAdder
{
    private FullAdder _block;

    [SetUp]
    public void Setup()
    {
        _block = new FullAdder();
    }

    [Test]
    public void InputAInitiallyFalse()
    {
        Assert.That(_block.A.Value, Is.EqualTo(false));
    }

    [Test]
    public void InputBInitiallyFalse()
    {
        Assert.That(_block.B.Value, Is.EqualTo(false));
    }

    public static Object[] UpdateSumOutTestCases =
    [
        new Object[] { false, false, false, false },
        new Object[] { false, false, true, true },
        new Object[] { false, true, false, true },
        new Object[] { false, true, true, false },
        new Object[] { true, false, false, true },
        new Object[] { true, false, true, false },
        new Object[] { true, true, false, false },
        new Object[] { true, true, true, true },
    ];

    [TestCaseSource((nameof(UpdateSumOutTestCases)))]
    public void Update_SetsSumOut_ToSumOfAAndB(Boolean a, Boolean b, Boolean carryIn, Boolean expectedO)
    {
        _block.A.Value = a;
        _block.B.Value = b;
        _block.CarryIn.Value = carryIn;

        _block.Update();

        Assert.That(_block.SumOut.Value, Is.EqualTo(expectedO));
    }

    public static Object[] UpdateCarryOutTestCases =
    [
        new Object[] { false, false, false, false },
        new Object[] { false, false, true, false },
        new Object[] { false, true, false, false },
        new Object[] { false, true, true, true },
        new Object[] { true, false, false, false },
        new Object[] { true, false, true, true },
        new Object[] { true, true, false, true },
        new Object[] { true, true, true, true },
    ];

    [TestCaseSource((nameof(UpdateCarryOutTestCases)))]
    public void Update_SetsCarryOut_ToCarryOfSumOfAAndB(Boolean a, Boolean b, Boolean carryIn, Boolean expectedO)
    {
        _block.A.Value = a;
        _block.B.Value = b;
        _block.CarryIn.Value = carryIn;

        _block.Update();

        Assert.That(_block.CarryOut.Value, Is.EqualTo(expectedO));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
