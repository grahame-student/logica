using LibLogica.Blocks;

namespace TestLibLogica.Blocks;

public class TestFlipFlopLevelTriggeredDType
{
    private FlipFlopLevelTriggeredDType _block;

    [SetUp]
    public void Setup()
    {
        _block = new FlipFlopLevelTriggeredDType();
    }

    [Test]
    public void InputD_InitiallyFalse()
    {
        Assert.That(_block.D.Value, Is.EqualTo(false));
    }

    [Test]
    public void InputClock_InitiallyFalse()
    {
        Assert.That(_block.Clock.Value, Is.EqualTo(false));
    }

    public static Object[] UpdateQTestCases =
    [
        //             d,     clk,  q
        new Object[] { false, true, false },
        new Object[] { true,  true, true },
    ];

    [TestCaseSource((nameof(UpdateQTestCases)))]
    public void Update_SetsQ(Boolean d, Boolean clk, Boolean expectedQ)
    {
        _block.D.Value     = d;
        _block.Clock.Value = clk;

        _block.Update();

        Assert.That(_block.Q.Value, Is.EqualTo(expectedQ));
    }

    public static Object[] UpdateNQTestCases =
    [
        //             d,     clk,  nq
        new Object[] { false, true, true },
        new Object[] { true,  true, false },
    ];

    [TestCaseSource((nameof(UpdateNQTestCases)))]
    public void Update_SetsNQ(Boolean d, Boolean clk, Boolean expectedNQ)
    {
        _block.D.Value     = d;
        _block.Clock.Value = clk;

        _block.Update();

        Assert.That(_block.NQ.Value, Is.EqualTo(expectedNQ));
    }

    [Test]
    public void Update_DoesNotUpdateInitialQ_WhenClockFalse()
    {
        Boolean qStart  = _block.Q.Value;
        _block.D.Value     = true;
        _block.Clock.Value = false;

        _block.Update();

        Assert.That(_block.Q.Value, Is.EqualTo(qStart));
    }

    [Test]
    public void Update_DoesNotUpdateQ_WhenInputsFalse()
    {
        _block.D.Value     = true;
        _block.Clock.Value = false;
        _block.Update();
        Boolean qStart = _block.Q.Value;

        _block.D.Value     = false;
        _block.Clock.Value = false;
        _block.Update();

        Assert.That(_block.Q.Value, Is.EqualTo(qStart));
    }

    [Test]
    public void Update_DoesNotUpdateInitialNQ_WhenClockFalse()
    {
        Boolean nqStart = _block.NQ.Value;
        _block.D.Value     = false;
        _block.Clock.Value = false;

        _block.Update();

        Assert.That(_block.NQ.Value, Is.EqualTo(nqStart));
    }

    [Test]
    public void Update_DoesNotUpdateNQ_WhenInputsFalse()
    {
        _block.D.Value     = true;
        _block.Clock.Value = false;
        _block.Update();
        Boolean nqStart = _block.NQ.Value;

        _block.D.Value     = false;
        _block.Clock.Value = false;
        _block.Update();

        Assert.That(_block.NQ.Value, Is.EqualTo(nqStart));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
