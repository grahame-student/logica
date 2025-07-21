using System;
using System.Linq;
using LibLogica.Blocks.Base;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Base;

public class TestWideLatchEdgeTriggered : WideLatchTestBase<LatchEdgeTriggeredWide>
{
    protected override LatchEdgeTriggeredWide CreateWideLatch(Int32 width)
    {
        return new LatchEdgeTriggeredWide(width);
    }

    public static Object[] UpdateQTestCases =
    [
        new Object[] { 0, false, false },
        new Object[] { 0, true, true },
        new Object[] { 7, false, false },
        new Object[] { 7, true, true },
    ];

    [TestCaseSource(nameof(UpdateQTestCases))]
    public void Update_SetsQToD_WhenClockRising(Int32 bit, Boolean d, Boolean expectedQ)
    {
        TestUpdateSetsQToD(bit, d, expectedQ, PerformRisingEdgeClockOperation);
    }
}
