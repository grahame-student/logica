using LibLogica.Blocks.Width8Bit;

namespace TestLibLogica.Blocks.Width8Bit;

public class TestLatch8Bit
{
    /*
     * 8 D Type FlipFlops
     * 8 Clock lines common
     *
     *      D [8]
     *        V
     * +---------------+
     * |               |
     * |  8-bit Latch  + < Clk
     * |               |
     * +---------------+
     *        V
     *      Q [8]
     *
     *
     * QN outputs unused
     * D -> Q   when   clk == 1
     *
     */

    private Latch8Bit _block;

    [SetUp]
    public void Setup()
    {
        _block = new Latch8Bit();
    }

    [Test]
    public void Constructor_SetsInputD_To8BitsWide()
    {
        Assert.That(_block.D.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsQ_To8BitsWide()
    {
        Assert.That(_block.Q.Count, Is.EqualTo(8));
    }

    public static Object[] UpdateQTestCases =
    [
        new Object[] { 0, false, false },
        new Object[] { 0, true, true },
        new Object[] { 1, false, false },
        new Object[] { 1, true, true },
        new Object[] { 2, false, false },
        new Object[] { 2, true, true },
        new Object[] { 3, false, false },
        new Object[] { 3, true, true },
        new Object[] { 4, false, false },
        new Object[] { 4, true, true },
        new Object[] { 5, false, false },
        new Object[] { 5, true, true },
        new Object[] { 6, false, false },
        new Object[] { 6, true, true },
        new Object[] { 7, false, false },
        new Object[] { 7, true, true },
    ];

    [TestCaseSource((nameof(UpdateQTestCases)))]
    public void Update_SetsQToD_WhenClockTrue(Int32 bit, Boolean clk, Boolean q)
    {
        _block.D[bit].Value = true;
        _block.Clock.Value = clk;

        _block.Update();

        Assert.That(_block.Q[bit].Value, Is.EqualTo(q));
    }


    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
