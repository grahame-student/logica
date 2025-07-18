using System;
using System.Linq;
using NUnit.Framework;
using LibLogica.Gates;
using LibLogica.IO;

namespace TestLibLogica.Blocks.Base;

public abstract class WideLatchTestBase<TWideLatch> where TWideLatch : LogicElement
{
    protected TWideLatch? _block;

    [Test]
    public void Constructor_SetsInputD_ToPassedWidth()
    {
        _block = CreateWideLatch(8);
        var dProperty = typeof(TWideLatch).GetProperty("D");
        if (dProperty?.GetValue(_block) is LogicArray<Input> dArray)
        {
            Assert.That(dArray.Count, Is.EqualTo(8));
        }
    }

    [Test]
    public void Constructor_SetsQ_ToPassedWidth()
    {
        _block = CreateWideLatch(8);
        var qProperty = typeof(TWideLatch).GetProperty("Q");
        if (qProperty?.GetValue(_block) is LogicArray<Output> qArray)
        {
            Assert.That(qArray.Count, Is.EqualTo(8));
        }
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

        var dProperty = typeof(TWideLatch).GetProperty("D");
        if (dProperty?.GetValue(_block) is LogicArray<Input> dArray)
        {
            dArray[bit].Value = d;

            clockOperation(_block);

            var qProperty = typeof(TWideLatch).GetProperty("Q");
            if (qProperty?.GetValue(_block) is LogicArray<Output> qArray)
            {
                Assert.That(qArray[bit].Value, Is.EqualTo(expectedQ));
            }
        }
    }
}
