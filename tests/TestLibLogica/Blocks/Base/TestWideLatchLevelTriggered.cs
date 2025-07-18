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
        TestUpdateSetsQToD(0, true, false, block =>
        {
            var clockProperty = typeof(WideLatchLevelTriggered).GetProperty("Clock");
            var clock = clockProperty!.GetValue(block) as Input;
            clock!.Value = false;
            block.Update();
        });
    }

    [Test]
    public void Update_SetsQToD_WhenClockTrue_Bit0_True()
    {
        TestUpdateSetsQToD(0, true, true, block =>
        {
            var clockProperty = typeof(WideLatchLevelTriggered).GetProperty("Clock");
            var clock = clockProperty!.GetValue(block) as Input;
            clock!.Value = true;
            block.Update();
        });
    }

    [Test]
    public void Update_SetsQToD_WhenClockTrue_Bit7_False()
    {
        TestUpdateSetsQToD(7, true, false, block =>
        {
            var clockProperty = typeof(WideLatchLevelTriggered).GetProperty("Clock");
            var clock = clockProperty!.GetValue(block) as Input;
            clock!.Value = false;
            block.Update();
        });
    }

    [Test]
    public void Update_SetsQToD_WhenClockTrue_Bit7_True()
    {
        TestUpdateSetsQToD(7, true, true, block =>
        {
            var clockProperty = typeof(WideLatchLevelTriggered).GetProperty("Clock");
            var clock = clockProperty!.GetValue(block) as Input;
            clock!.Value = true;
            block.Update();
        });
    }
}
