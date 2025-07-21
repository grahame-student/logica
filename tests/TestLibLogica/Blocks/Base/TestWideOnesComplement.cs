using System;
using System.Linq;
using LibLogica.Blocks.Base;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Base;

public class TestWideOnesComplement : LogicElementTestBase<OnesComplementWide>
{
    [Test]
    public void Constructor_SetsInputA_ToPassedWidth()
    {
        Assert.That(_element.A.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsSumOut_ToPassedWidth()
    {
        Assert.That(_element.O.Count, Is.EqualTo(8));
    }

    public static Object[] InvertTestCases =
    [
        //             ax, invert, expected
        new Object[] { 0, false, true },
        new Object[] { 0, true, false },
        new Object[] { 1, false, true },
        new Object[] { 1, true, false },
        new Object[] { 2, false, true },
        new Object[] { 2, true, false },
        new Object[] { 3, false, true },
        new Object[] { 3, true, false },
        new Object[] { 4, false, true },
        new Object[] { 4, true, false },
        new Object[] { 5, false, true },
        new Object[] { 5, true, false },
        new Object[] { 6, false, true },
        new Object[] { 6, true, false },
        new Object[] { 7, false, true },
        new Object[] { 7, true, false },
    ];

    [TestCaseSource(nameof(InvertTestCases))]
    public void Update_SetsO_ToNotAWhenInvertSet(Int32 bit, Boolean invert, Boolean output)
    {
        _element.A[bit].Value = true;
        _element.Invert.Value = invert;

        _element.Update();

        Assert.That(_element.O[bit].Value, Is.EqualTo(output));
    }
}
