using LibLogica.Blocks;

using System;
using NUnit.Framework;
using System.Linq;

namespace TestLibLogica.Blocks;

public class TestFlipFlopRS
{
    private FlipFlopRS _block;

    [SetUp]
    public void Setup()
    {
        _block = new FlipFlopRS();
    }

    [Test]
    public void InputRInitiallyFalse()
    {
        Assert.That(_block.R.Value, Is.EqualTo(false));
    }

    [Test]
    public void InputSInitiallyFalse()
    {
        Assert.That(_block.S.Value, Is.EqualTo(false));
    }

    [Test]
    public void OutputQInitiallyFalse()
    {
        Assert.That(_block.Q.Value, Is.EqualTo(false));
    }

    [Test]
    public void OutputNQInitiallyTrue()
    {
        Assert.That(_block.NQ.Value, Is.EqualTo(true));
    }

    public static Object[] UpdateQTestCases =
    [
        //             r,     s,     q
        new Object[] { false, true, true },
        new Object[] { true, false, false },
        new Object[] { true, true, false },
    ];

    [TestCaseSource((nameof(UpdateQTestCases)))]
    public void Update_SetsQ(Boolean r, Boolean s, Boolean expectedQ)
    {
        _block.R.Value = r;
        _block.S.Value = s;

        _block.Update();

        Assert.That(_block.Q.Value, Is.EqualTo(expectedQ));
    }

    public static Object[] UpdateNQTestCases =
    [
        //             r,     s,     nq
        new Object[] { false, true, false },
        new Object[] { true, false, true },
        new Object[] { true, true, false },
    ];

    [TestCaseSource((nameof(UpdateNQTestCases)))]
    public void Update_SetsNQ(Boolean r, Boolean s, Boolean expectedNQ)
    {
        _block.R.Value = r;
        _block.S.Value = s;

        _block.Update();

        Assert.That(_block.NQ.Value, Is.EqualTo(expectedNQ));
    }

    [Test]
    public void Update_DoesNotUpdateInitialQ_WhenInputsFalse()
    {
        Boolean qStart = _block.Q.Value;
        _block.R.Value = false;
        _block.S.Value = false;

        _block.Update();

        Assert.That(_block.Q.Value, Is.EqualTo(qStart));
    }

    [Test]
    public void Update_DoesNotUpdateQ_WhenInputsFalse()
    {
        _block.S.Value = true;
        _block.R.Value = false;
        _block.Update();
        Boolean qStart = _block.Q.Value;

        _block.R.Value = false;
        _block.S.Value = false;
        _block.Update();

        Assert.That(_block.Q.Value, Is.EqualTo(qStart));
    }

    [Test]
    public void Update_DoesNotUpdateInitialNQ_WhenInputsFalse()
    {
        Boolean nqStart = _block.NQ.Value;
        _block.R.Value = false;
        _block.S.Value = false;

        _block.Update();

        Assert.That(_block.NQ.Value, Is.EqualTo(nqStart));
    }

    [Test]
    public void Update_DoesNotUpdateNQ_WhenInputsFalse()
    {
        _block.R.Value = true;
        _block.S.Value = false;
        _block.Update();
        Boolean nqStart = _block.NQ.Value;

        _block.R.Value = false;
        _block.S.Value = false;
        _block.Update();

        Assert.That(_block.NQ.Value, Is.EqualTo(nqStart));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
