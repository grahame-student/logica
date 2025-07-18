using System;
using LibLogica.Blocks.Base;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Base;

internal class TestRippleCounter
{
    private RippleCounter _counter;

    [SetUp]
    public void SetUp()
    {
        _counter = new RippleCounter(8);
    }

    [Test]
    public void Constructor_SetsQ_ToPassedWidth()
    {
        Assert.That(_counter.Q.Count, Is.EqualTo(8));
    }

    [Test]
    public void QValues_False_Initially()
    {
        Assert.That(GetCounterValue(), Is.Zero);
    }

    public static readonly Object[] UpdateCounterTestCases =
    [
        new Object[] { 1, 1u },
        new Object[] { 2, 2u },
        new Object[] { 3, 3u },
        new Object[] { 255, 255u },
        new Object[] { 256, 0u },
        new Object[] { 257, 1u },
    ];

    [TestCaseSource(nameof(UpdateCounterTestCases))]
    public void RisingEdge_IncrementsCounter(Int32 numRising, UInt32 expected)
    {
        for (Int32 i = 0; i < numRising; i++)
        {
            _counter.Clk.Value = false;
            _counter.Update();
            _counter.Clk.Value = true;
            _counter.Update();
            TestContext.Out.WriteLine($"Counter value after {i + 1} rising edges: {GetCounterValue()}");
        }
        Assert.That(GetCounterValue(), Is.EqualTo(expected));
    }

    private UInt32 GetCounterValue()
    {
        UInt32 value = 0;
        for (Int32 i = 0; i < _counter.Q.Count; i++)
        {
            if (_counter.Q[i].Value)
            {
                value |= 1u << i;
            }
        }
        return value;
    }
}
