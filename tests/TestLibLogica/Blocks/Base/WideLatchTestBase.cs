using System;
using System.Linq;
using NUnit.Framework;
using LibLogica.Gates;
using LibLogica.IO;

namespace TestLibLogica.Blocks.Base;

public abstract class WideLatchTestBase<TWideLatch> where TWideLatch : LogicElement, LibLogica.Blocks.Base.IWideLatch
{
    protected TWideLatch? _block;

    [Test]
    public void Constructor_SetsInputD_ToPassedWidth()
    {
        _block = CreateWideLatch(8);
        Assert.That(_block.D.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsQ_ToPassedWidth()
    {
        _block = CreateWideLatch(8);
        Assert.That(_block.Q.Count, Is.EqualTo(8));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        _block = CreateWideLatch(8);
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }

    protected abstract TWideLatch CreateWideLatch(int width);

    protected void TestUpdateSetsQToD(Int32 bit, Boolean d, Boolean expectedQ, Action<TWideLatch> clockOperation)
    {
        _block = CreateWideLatch(8);
        _block.D[bit].Value = d;

        clockOperation(_block);

        Assert.That(_block.Q[bit].Value, Is.EqualTo(expectedQ));
    }

    /// <summary>
    /// Helper method for edge-triggered latches - performs a rising edge clock operation.
    /// </summary>
    protected void PerformRisingEdgeClockOperation(TWideLatch block)
    {
        block.Clock.Value = false;
        block.Update();
        block.Clock.Value = true;
        block.Update();
    }

    /// <summary>
    /// Helper method for level-triggered latches - sets clock high and updates.
    /// </summary>
    protected void PerformHighLevelClockOperation(TWideLatch block)
    {
        block.Clock.Value = true;
        block.Update();
    }

    /// <summary>
    /// Helper method for level-triggered latches - sets clock low and updates.
    /// </summary>
    protected void PerformLowLevelClockOperation(TWideLatch block)
    {
        block.Clock.Value = false;
        block.Update();
    }
}