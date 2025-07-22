using System;
using System.Linq;
using LibLogica.Blocks.Base;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Base;

public class TestAdderWide : LogicElementTestBase<AdderWide>
{
    [Test]
    public void Constructor_SetsInputA_ToPassedWidth()
    {
        _element = new AdderWide(8);
        foreach (String id in _element.GetIds())
        {
            Console.WriteLine(id);
        }

        Assert.That(_element.A.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsInputB_ToPassedWidth()
    {
        _element = new AdderWide(8);

        Assert.That(_element.B.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsSumOut_ToPassedWidth()
    {
        _element = new AdderWide(8);

        Assert.That(_element.SumOut.Count, Is.EqualTo(8));
    }

    public static Object[] UpdateSumOutTestCases =
    [
        new Object[] { 0, 0, 1 },
        new Object[] { 1, 1, 2 },
        new Object[] { 2, 2, 3 },
        new Object[] { 3, 3, 4 },
        new Object[] { 4, 4, 5 },
        new Object[] { 5, 5, 6 },
        new Object[] { 6, 6, 7 },
    ];

    [TestCaseSource(nameof(UpdateSumOutTestCases))]
    public void Update_SetsSumOut_ToSumOfAAndB(Int32 ax, Int32 bx, Int32 sx)
    {
        _element = new AdderWide(8);
        _element.A[ax].Value = true;
        _element.B[bx].Value = true;

        _element.Update();

        Assert.That(_element.SumOut[sx].Value, Is.True);
    }

    [Test]
    public void Update_SetsCarryOut_ToCarryOfSumOfAAndB()
    {
        _element = new AdderWide(8);
        _element.A[7].Value = true;
        _element.B[7].Value = true;

        _element.Update();

        Assert.That(_element.CarryOut.Value, Is.True);
    }

    [Test]
    public void Update_AddsCarryIn_ToBit0()
    {
        _element = new AdderWide(8);
        _element.CarryIn.Value = true;

        _element.Update();

        Assert.That(_element.SumOut[0].Value, Is.True);

    }
}
