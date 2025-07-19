using System;
using System.Linq;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Gates;

internal class TestBufferGate
{
    //
    // A --|>-- O
    //     ^
    //     Enable
    //
    // | A | Enable | O    |
    // +---+--------+------+
    // | 0 | 1      | 0    |
    // | 1 | 1      | 1    |
    // | x | 0      | null |

    private BufferGate _gate;

    [SetUp]
    public void Setup()
    {
        _gate = new BufferGate();
    }

    [Test]
    public void A_Initially_False()
    {
        Assert.That(_gate.A.Value, Is.False);
    }

    [Test]
    public void Enable_Initially_False()
    {
        Assert.That(_gate.Enable.Value, Is.False);
    }

    [Test]
    public void O_Initially_Null()
    {
        Assert.That(_gate.O.Value, Is.False);
    }

    [Test]
    public void O_Initially_HighImpedance()
    {
        Assert.That(_gate.O.IsHighImpedance, Is.True);
    }

    [Test]
    public void O_IsNotHighImpedance_WhenEnableTrue()
    {
        _gate.Enable.Value = true;
        _gate.Update();

        Assert.That(_gate.O.IsHighImpedance, Is.False);
    }

    public static readonly Object[] UpdateTestCases =
    [
        new Object[] { false, false },
        new Object[] { true, true },
    ];

    [TestCaseSource(nameof(UpdateTestCases))]
    public void Update_SetsO_ToAWhenEnabled(Boolean a, Boolean expectedO)
    {
        _gate.A.Value = a;
        _gate.Enable.Value = true;

        _gate.Update();

        Assert.That(_gate.O.Value, Is.EqualTo(expectedO));
        Assert.That(_gate.O.IsHighImpedance, Is.False);
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_gate.GetIds().Count(), Is.EqualTo(_gate.GetValues().Count()));
    }
}
