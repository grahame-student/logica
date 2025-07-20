using System;
using System.Linq;
using LibLogica.Blocks.Base;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Base;

public class TestWideAdder
{
    // Only nullable because of the way NUnit works with fields.
    private WideAdder? _block;

    [Test]
    public void Constructor_SetsInputA_ToPassedWidth()
    {
        _block = new WideAdder(8);
        foreach (String id in _block.GetIds())
        {
            Console.WriteLine(id);
        }

        Assert.That(_block.A.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsInputB_ToPassedWidth()
    {
        _block = new WideAdder(8);

        Assert.That(_block.B.Count, Is.EqualTo(8));
    }

    [Test]
    public void Constructor_SetsSumOut_ToPassedWidth()
    {
        _block = new WideAdder(8);

        Assert.That(_block.SumOut.Count, Is.EqualTo(8));
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
        _block = new WideAdder(8);
        _block.A[ax].Value = true;
        _block.B[bx].Value = true;

        _block.Update();

        Assert.That(_block.SumOut[sx].Value, Is.True);
    }

    [Test]
    public void Update_SetsCarryOut_ToCarryOfSumOfAAndB()
    {
        _block = new WideAdder(8);
        _block.A[7].Value = true;
        _block.B[7].Value = true;

        _block.Update();

        Assert.That(_block.CarryOut.Value, Is.True);
    }

    [Test]
    public void Update_AddsCarryIn_ToBit0()
    {
        _block = new WideAdder(8);
        _block.CarryIn.Value = true;

        _block.Update();

        Assert.That(_block.SumOut[0].Value, Is.True);

    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        _block = new WideAdder(8);
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }

    [Test]
    public void GetIdsAndGetValues_CorrespondByPosition()
    {
        _block = new WideAdder(8);
        var ids = _block.GetIds().ToList();
        var values = _block.GetValues().ToList();

        Assert.That(ids.Count, Is.EqualTo(values.Count));

        // Test that IDs and values correspond by position
        var correspondences = ids.Zip(values, (id, value) => new { Id = id, Value = value }).ToList();
        Assert.That(correspondences.Count, Is.EqualTo(ids.Count),
            "IDs and values should correspond by position");
    }

    [Test]
    public void GetIds_AllIdsFollowCorrectFormat()
    {
        _block = new WideAdder(8);
        var ids = _block.GetIds().ToList();

        foreach (var id in ids)
        {
            Assert.That(id.StartsWith("WideAdder_"), Is.True,
                $"ID '{id}' should start with the class name prefix 'WideAdder_'");
        }
    }
}
