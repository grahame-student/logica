using LibLogica.Blocks.Base;

namespace TestLibLogica.Blocks.Base;

public class TestWideLatch
{
    /*
     * x D Type FlipFlops
     * x Clock lines common
     *
     *      D [x]
     *        V
     * +---------------+
     * |               |
     * |  x-bit Latch  + < Clk
     * |               |
     * +---------------+
     *        V
     *      Q [x]
     *
     *
     * QN outputs unused
     * D -> Q   when   clk == 1
     *
     */

    private WideLatch _block;

    [Test]
    public void Constructor_SetsInputD_ToPassedWidth()
    {
        _block = new WideLatch(8);
        foreach (String id in _block.GetIds())
        {
            Console.WriteLine(id);
        }

        Assert.That(_block.D.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsQ_ToPassedWidth()
    {
        _block = new WideLatch(8);

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
        _block = new WideLatch(8);
        _block.D[bit].Value = true;
        _block.Clock.Value = clk;

        _block.Update();

        Assert.That(_block.Q[bit].Value, Is.EqualTo(q));
    }


    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        for (Int32 i = 0; i < _block.GetIds().Count(); i++)
        {
            Console.WriteLine($"{_block.GetIds().ToArray()[i]} - {_block.GetValues().ToArray()[i]}");
        }

        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
