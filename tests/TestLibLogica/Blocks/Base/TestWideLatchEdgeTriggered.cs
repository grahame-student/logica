using System;
using System.Linq;
using LibLogica.Blocks.Base;
using NUnit.Framework;
using LibLogica.IO;

namespace TestLibLogica.Blocks.Base;

public class TestWideLatchEdgeTriggered : WideLatchTestBase<WideLatchEdgeTriggered>
{
    protected override WideLatchEdgeTriggered CreateWideLatch(int width)
    {
        return new WideLatchEdgeTriggered(width);
    }

    [Test]
    public void Update_SetsQToD_WhenClockRising_Bit0_False()
    {
        TestUpdateSetsQToD(0, false, false, PerformRisingEdgeClockOperation);
    }

    [Test]
    public void Update_SetsQToD_WhenClockRising_Bit0_True()
    {
        TestUpdateSetsQToD(0, true, true, PerformRisingEdgeClockOperation);
    }

    [Test]
    public void Update_SetsQToD_WhenClockRising_Bit7_False()
    {
        TestUpdateSetsQToD(7, false, false, PerformRisingEdgeClockOperation);
    }

    [Test]
    public void Update_SetsQToD_WhenClockRising_Bit7_True()
    {
        TestUpdateSetsQToD(7, true, true, PerformRisingEdgeClockOperation);
    }
}
