using LibLogica.Blocks.Width8Bit;

namespace TestLibLogica.Blocks.Width8Bit;

public class TestAdder8Bit
{
    private Adder8Bit _block;

    [SetUp]
    public void Setup()
    {
        _block = new Adder8Bit();
    }

    [Test]
    public void Constructor_SetsInputA_To8BitsWide()
    {
        Assert.That(_block.A.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsInputB_To8BitsWide()
    {
        Assert.That(_block.B.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsSumOut_To8BitsWide()
    {
        Assert.That(_block.SumOut.Count, Is.EqualTo(8));
    }

    public static Object[] UpdateSumOutTestCases =
    [
        new Object[] { 0, 0, 1 },
        new Object[] { 1, 1, 2 },
        new Object[] { 2, 2, 3 },
        new Object[] { 3, 3, 4 },
        new Object[] { 4, 4, 5 },
        new Object[] { 5, 5, 6 },
        new Object[] { 6, 6, 7 },
    ];

    [TestCaseSource((nameof(UpdateSumOutTestCases)))]
    public void Update_SetsSumOut_ToSumOfAAndB(Int32 ax, Int32 bx, Int32 sx)
    {
        _block.A[ax].Value = true;
        _block.B[bx].Value = true;

        _block.Update();

        Assert.That(_block.SumOut[sx].Value, Is.EqualTo(true));
    }

    [Test]
    public void Update_SetsCarryOut_ToCarryOfSumOfAAndB()
    {
        _block.A[7].Value = true;
        _block.B[7].Value = true;

        _block.Update();

        Assert.That(_block.CarryOut.Value, Is.EqualTo(true));
    }

    [Test]
    public void Update_AddsCarryIn_ToBit0()
    {
        _block.CarryIn.Value = true;

        _block.Update();

        Assert.That(_block.SumOut[0].Value, Is.EqualTo(true));

    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
