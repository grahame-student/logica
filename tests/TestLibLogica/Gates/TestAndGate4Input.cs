using System;
using System.Collections;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Gates;

internal class TestAndGate4Input : LogicElementTestBase<AndGate4Input>
{
    [Test]
    public void A_Initially_False()
    {
        Assert.That(_element.A.Value, Is.False);
    }

    [Test]
    public void B_Initially_False()
    {
        Assert.That(_element.B.Value, Is.False);
    }

    [Test]
    public void C_Initially_False()
    {
        Assert.That(_element.C.Value, Is.False);
    }

    [Test]
    public void D_Initially_False()
    {
        Assert.That(_element.D.Value, Is.False);
    }

    [Test]
    public void O_Initially_False()
    {
        Assert.That(_element.O.Value, Is.False);
    }

    public static readonly Object[] AndGate4InputTruethTable =
        [
            new Object[] { false, false, false, false, false },
            new Object[] { false, false, false, true, false },
            new Object[] { false, false, true, false, false },
            new Object[] { false, false, true, true, false },
            new Object[] { false, true, false, false, false },
            new Object[] { false, true, false, true, false },
            new Object[] { false, true, true, false, false },
            new Object[] { false, true, true, true, false },
            new Object[] { true, false, false, false, false },
            new Object[] { true, false, false, true, false },
            new Object[] { true, false, true, false, false },
            new Object[] { true, false, true, true, false },
            new Object[] { true, true, false, false, false },
            new Object[] { true, true, false, true, false },
            new Object[] { true, true, true, false, false },
            new Object[] { true, true, true, true, true }
        ];

    [TestCaseSource(nameof(AndGate4InputTruethTable))]
    public void AndGate4Input_TruthTable(Boolean a, Boolean b, Boolean c, Boolean d, Boolean o)
    {
        _element.A.Value = a;
        _element.B.Value = b;
        _element.C.Value = c;
        _element.D.Value = d;
        _element.Update();
        Assert.That(_element.O.Value, Is.EqualTo(o));
    }
}
