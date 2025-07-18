using LibLogica.Blocks.Base;

using System;
using NUnit.Framework;
using System.Linq;

namespace TestLibLogica.Blocks.Base;

public class TestWideLatchLevelTriggered
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

    private WideLatchLevelTriggered? _block;

    [Test]
    public void Constructor_SetsInputD_ToPassedWidth()
    {
        _block = new WideLatchLevelTriggered(8);

        Assert.That(_block.D.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsQ_ToPassedWidth()
    {
        _block = new WideLatchLevelTriggered(8);

        Assert.That(_block.Q.Count, Is.EqualTo(8));
    }

    public static Object[] _updateQTestCases =
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

    [TestCaseSource((nameof(_updateQTestCases)))]
    public void Update_SetsQToD_WhenClockTrue(Int32 bit, Boolean clk, Boolean q)
    {
        _block = new WideLatchLevelTriggered(8);
        _block.D[bit].Value = true;
        _block.Clock.Value = clk;

        _block.Update();

        Assert.That(_block.Q[bit].Value, Is.EqualTo(q));
    }


    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        _block = new WideLatchLevelTriggered(8);
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
