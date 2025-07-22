using System;
using System.Linq;
using LibLogica.Blocks.Base;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Base;

public class TestWideLatchLevelTriggered : WideLatchTestBase<LatchLevelTriggeredWide>
{
    protected override LatchLevelTriggeredWide CreateWideLatch(Int32 width)
    {
        return new LatchLevelTriggeredWide(width);
    }

    public static Object[] UpdateQWhenClockHighTestCases =
    [
        new Object[] { 0, false, false },
        new Object[] { 0, true, true },
        new Object[] { 7, false, false },
        new Object[] { 7, true, true },
    ];

    [TestCaseSource(nameof(UpdateQWhenClockHighTestCases))]
    public void Update_SetsQToD_WhenClockHigh(Int32 bit, Boolean d, Boolean expectedQ)
    {
        TestUpdateSetsQToD(bit, d, expectedQ, PerformHighLevelClockOperation);
    }

    public static Object[] UpdateQWhenClockLowTestCases =
    [
        new Object[] { 0, false, false },
        new Object[] { 0, true, false },
        new Object[] { 7, false, false },
        new Object[] { 7, true, false },
    ];

    [TestCaseSource(nameof(UpdateQWhenClockLowTestCases))]
    public void Update_DoesNotSetQToD_WhenClockLow(Int32 bit, Boolean d, Boolean expectedQ)
    {
        TestUpdateSetsQToD(bit, d, expectedQ, PerformLowLevelClockOperation);
    }
}
