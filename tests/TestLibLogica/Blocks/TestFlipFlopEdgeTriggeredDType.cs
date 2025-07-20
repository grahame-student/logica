using System;
using System.Linq;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks;

public class TestFlipFlopEdgeTriggeredDType : LogicElementTestBase<FlipFlopEdgeTriggeredDType>
{
    private FlipFlopEdgeTriggeredDType _block;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _block = _element;
    }

    [Test]
    public void InputD_InitiallyFalse()
    {
        Assert.That(_block.D.Value, Is.False);
    }

    [Test]
    public void InputClock_InitiallyFalse()
    {
        Assert.That(_block.Clock.Value, Is.False);
    }

    [Test]
    public void OutputQ_InitiallyFalse()
    {
        Assert.That(_block.Q.Value, Is.False);
    }

    [Test]
    public void OutputNQ_InitiallyTrue()
    {
        Assert.That(_block.NQ.Value, Is.True);
    }

    public static Object[] UpdateQTestCases =
    [
        //             initQ, d,    isRising, expectedQ
        new Object[] { false, false, false, false },
        new Object[] { false, false, true, false },
        new Object[] { false, true, false, false },
        new Object[] { false, true, true, true },
        new Object[] { true, false, false, true },
        new Object[] { true, false, true, false },
        new Object[] { true, true, false, true },
        new Object[] { true, true, true, true },
    ];

    [TestCaseSource(nameof(UpdateQTestCases))]
    public void Update_SetsQ(Boolean initQ, Boolean d, Boolean isRisingEdge, Boolean expectedQ)
    {
        SetInitialQState(initQ);

        _block.D.Value = d;
        _block.Clock.Value = !isRisingEdge;
        _block.Update();

        _block.Clock.Value = isRisingEdge;
        _block.Update();

        Assert.That(_block.Q.Value, Is.EqualTo(expectedQ));
    }

    private void SetInitialQState(Boolean initQ)
    {
        if (!initQ) return;

        // If we want to set Q high, we need to simulate a rising edge on the clock
        _block.D.Value = true;
        _block.Clock.Value = false;
        _block.Update();
        _block.Clock.Value = true;
        // Rising edge to set Q high
        _block.Update();

        // Assert that Q is high to ensure that the test preconditions are met
        // Although this is technically a second assertion, it protects the integrity of the test
        Assert.That(_block.Q.Value, Is.True, "Q should be true");
    }

    public static Object[] UpdateNQTestCases =
    [
        //             initQ, d,     isRising, expectedNq
        new Object[] { false, false, false, true },
        new Object[] { false, false, true, true },
        new Object[] { false, true, false, true },
        new Object[] { false, true, true, false },

        new Object[] { true, false, false, false },
        new Object[] { true, false, true, false },
        new Object[] { true, true, false, false },
        new Object[] { true, true, true, false },
    ];

    [TestCaseSource(nameof(UpdateNQTestCases))]
    public void Update_SetsNQ(Boolean initQ, Boolean d, Boolean isRisingEdge, Boolean expectedNQ)
    {
        SetInitialQState(initQ);

        _block.D.Value = d;
        _block.Clock.Value = !isRisingEdge;
        _block.Update();

        _block.Clock.Value = isRisingEdge;
        _block.Update();

        Assert.That(_block.NQ.Value, Is.EqualTo(expectedNQ));
    }
}
