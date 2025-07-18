using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks;

public class TestAccumulatingAdderLevelTriggered
{
    /*
     *         A[8] = 0 <- initial value
     *         A[8] := Q[8]
     *  +-------+
     *  |       |        B [8] = Value to add
     *  |       V        V
     *  |   +---------------+
     *  |   |               |
     *  |   |  8-bit Adder  + < CI = 0
     *  |   |               |
     *  |   +---------------+
     *  |          V
     *  |        D [8]
     *  |          V
     *  |   +---------------+
     *  |   |               |
     *  |   |  8-bit Latch  + < Clk = 1 When Q to be passed to adder
     *  |   |               |
     *  |   +---------------+
     *  |          |
     *  +----------+ Q [8]
     *             |
     *             V
     *           Result
     *
     *
     *      QN outputs unused
     *      D -> Q   when   clk == 1
     *
     */

    private AccumulatingAdderLevelTriggered _block;

    [SetUp]
    public void Setup()
    {
        _block = new AccumulatingAdderLevelTriggered();
    }

    // Consolidated test case array that replaces separate InitialAValues and InitialOValues arrays
    // to eliminate code duplication while maintaining parameterized test coverage for all bit positions
    public static Object[] BitIndices =
    [
        new Object[] { 0, },
        new Object[] { 1, },
        new Object[] { 2, },
        new Object[] { 3, },
        new Object[] { 4, },
        new Object[] { 5, },
        new Object[] { 6, },
        new Object[] { 7, },
    ];

    [TestCaseSource(nameof(BitIndices))]
    public void AInputsInitiallyFalse(Int32 bit)
    {
        Assert.That(_block.A[bit].Value, Is.EqualTo(false));
    }

    [Test]
    public void AddInitiallyFalse()
    {
        Assert.That(_block.Add.Value, Is.EqualTo(false));
    }

    [TestCaseSource(nameof(BitIndices))]
    public void OOutputsInitiallyFalse(Int32 bit)
    {
        Assert.That(_block.O[bit].Value, Is.EqualTo(false));
    }

    [Test]
    public void UpdateSetsO_ToA_WhenAddTrue()
    {
        // bits 0 -> 7
        IList<Boolean> val170 = new List<Boolean>()
        {
            false,
            true,
            false,
            true,
            false,
            true,
            false,
            true,
        };
        for (Int32 i = 0; i < _block.A.Count; i++)
        {
            _block.A[i].Value = val170[i];
        }

        _block.Add.Value = true;
        _block.Update();

        var result = new List<Boolean>();
        for (Int32 i = 0; i < _block.O.Count; i++)
        {
            result.Add(_block.O[i].Value);
        }

        Assert.That(result, Is.EqualTo(val170));
    }

    [Test]
    public void Update_DoesNotUpdateO_WhenAddFalse()
    {
        // bits 0 -> 7
        IList<Boolean> expected = new List<Boolean>()
        {
            true,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
        };
        _block.A[0].Value = true;
        _block.Add.Value = true;
        _block.Update();

        _block.Add.Value = false;
        _block.Update();

        var result = new List<Boolean>();
        for (Int32 i = 0; i < _block.O.Count; i++)
        {
            result.Add(_block.O[i].Value);
        }

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
    }
}
