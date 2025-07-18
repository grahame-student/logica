using LibLogica.Blocks.Base;
using System;
using NUnit.Framework;
using System.Linq;
using LibLogica.IO;

namespace TestLibLogica.Blocks.Base;

public class TestWideLatchLevelTriggered : WideLatchTestBase<WideLatchLevelTriggered>
{
    protected override WideLatchLevelTriggered CreateWideLatch(int width)
    {
        return new WideLatchLevelTriggered(width);
    }

    [Test]
    public void Update_SetsQToD_WhenClockTrue_Bit0_False()
    {
        TestUpdateSetsQToD(0, true, false, PerformLowLevelClockOperation);
    }

    [Test]
    public void Update_SetsQToD_WhenClockTrue_Bit0_True()
    {
        TestUpdateSetsQToD(0, true, true, PerformHighLevelClockOperation);
    }

    [Test]
    public void Update_SetsQToD_WhenClockTrue_Bit7_False()
    {
        TestUpdateSetsQToD(7, true, false, PerformLowLevelClockOperation);
    }

    [Test]
    public void Update_SetsQToD_WhenClockTrue_Bit7_True()
    {
        TestUpdateSetsQToD(7, true, true, PerformHighLevelClockOperation);
    }
}
