using System;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Gates;

internal class TestOrGate8Input : LogicElementTestBase<OrGate8Input>
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
    public void E_Initially_False()
    {
        Assert.That(_element.E.Value, Is.False);
    }

    [Test]
    public void F_Initially_False()
    {
        Assert.That(_element.F.Value, Is.False);
    }

    [Test]
    public void G_Initially_False()
    {
        Assert.That(_element.G.Value, Is.False);
    }

    [Test]
    public void H_Initially_False()
    {
        Assert.That(_element.H.Value, Is.False);
    }

    [Test]
    public void O_Initially_False()
    {
        Assert.That(_element.O.Value, Is.False);
    }

    public static readonly Object[] TestCaseData =
    [
        // Testing full truth table is overkill, open to ideas for best practices here.
        // Inputs: A, B, C, D, E, F, G, H | Expected Output
        new Object[] { false, false, false, false, false, false, false, false, false },
        new Object[] { false, false, false, false, false, false, false, true, true },
        new Object[] { false, false, false, false, false, false, true, false, true },
        new Object[] { false, false, false, false, false, true, false, false, true },
        new Object[] { false, false, false, false, true, false, false, false, true },
        new Object[] { false, false, false, true, false, false, false, false, true },
        new Object[] { false, false, true, false, false, false, false, false, true },
        new Object[] { false, true, false, false, false, false, false, false, true },
        new Object[] { true, false, false, false, false, false, false, false, true },
        new Object[] { true, true, true, true, true, true, true, true, true },
    ];

    [TestCaseSource(nameof(TestCaseData))]
    public void OrGate8Input_UpdatesCorrectly(Boolean a,
                                              Boolean b,
                                              Boolean c,
                                              Boolean d,
                                              Boolean e,
                                              Boolean f,
                                              Boolean g,
                                              Boolean h,
                                              Boolean expected)
    {
        _element.A.Value = a;
        _element.B.Value = b;
        _element.C.Value = c;
        _element.D.Value = d;
        _element.E.Value = e;
        _element.F.Value = f;
        _element.G.Value = g;
        _element.H.Value = h;

        _element.Update();

        Assert.That(_element.O.Value, Is.EqualTo(expected));
    }
}
