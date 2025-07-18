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
        TestUpdateSetsQToD(0, false, false, block =>
        {
            var clockProperty = typeof(WideLatchEdgeTriggered).GetProperty("Clock");
            var clock = clockProperty!.GetValue(block) as Input;
            clock!.Value = false;
            block.Update();
            clock.Value = true;
            block.Update();
        });
    }

    [Test]
    public void Update_SetsQToD_WhenClockRising_Bit0_True()
    {
        TestUpdateSetsQToD(0, true, true, block =>
        {
            var clockProperty = typeof(WideLatchEdgeTriggered).GetProperty("Clock");
            var clock = clockProperty!.GetValue(block) as Input;
            clock!.Value = false;
            block.Update();
            clock.Value = true;
            block.Update();
        });
    }

    [Test]
    public void Update_SetsQToD_WhenClockRising_Bit7_False()
    {
        TestUpdateSetsQToD(7, false, false, block =>
        {
            var clockProperty = typeof(WideLatchEdgeTriggered).GetProperty("Clock");
            var clock = clockProperty!.GetValue(block) as Input;
            clock!.Value = false;
            block.Update();
            clock.Value = true;
            block.Update();
        });
    }

    [Test]
    public void Update_SetsQToD_WhenClockRising_Bit7_True()
    {
        TestUpdateSetsQToD(7, true, true, block =>
        {
            var clockProperty = typeof(WideLatchEdgeTriggered).GetProperty("Clock");
            var clock = clockProperty!.GetValue(block) as Input;
            clock!.Value = false;
            block.Update();
            clock.Value = true;
            block.Update();
        });
    }
}
