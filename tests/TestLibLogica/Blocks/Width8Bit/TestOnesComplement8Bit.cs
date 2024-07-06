using LibLogica.Blocks.Width8Bit;

namespace TestLibLogica.Blocks.Width8Bit;

public class TestOnesComplement8Bit
{
    private OnesComplement8Bit _block;

    [SetUp]
    public void Setup()
    {
        _block = new OnesComplement8Bit();
    }

    [Test]
    public void Constructor_SetsInputA_ToPassedWidth()
    {

        Assert.That(_block.A.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsO_ToPassedWidth()
    {
        Assert.That(_block.O.Count, Is.EqualTo(8));
    }

    public static Object[] InvertTestCases =
    [
        //             ax, invert, expected
        new Object[] { 0, false, true },
        new Object[] { 0, true, false },
        new Object[] { 1, false, true },
        new Object[] { 1, true, false },
        new Object[] { 2, false, true },
        new Object[] { 2, true, false },
        new Object[] { 3, false, true },
        new Object[] { 3, true, false },
        new Object[] { 4, false, true },
        new Object[] { 4, true, false },
        new Object[] { 5, false, true },
        new Object[] { 5, true, false },
        new Object[] { 6, false, true },
        new Object[] { 6, true, false },
        new Object[] { 7, false, true },
        new Object[] { 7, true, false },
    ];

    [TestCaseSource((nameof(InvertTestCases)))]
    public void Update_SetsO_ToNotAWhenInvertSet(Int32 bit, Boolean invert, Boolean output)
    {
        _block.A[bit].Value = true;
        _block.Invert.Value = invert;

        _block.Update();

        Assert.That(_block.O[bit].Value, Is.EqualTo(output));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
