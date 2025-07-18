using LibLogica.Blocks;

using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TestLibLogica.Blocks;

public class TestAccumulatingAdderEdgeTriggered
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
     *  |   |  8-bit Latch  + < Clk rising When Q to be passed to adder
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
     *      D -> Q   when   clk rising
     *
     */

    private AccumulatingAdderEdgeTriggered _block;

    [SetUp]
    public void Setup()
    {
        _block = new AccumulatingAdderEdgeTriggered();
    }

    // Consolidated test case array that replaces separate InitialAValues and InitialOValues arrays
    // to eliminate code duplication while maintaining parameterized test coverage for all bit positions
    public static readonly Object[] _bitIndices =
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

    [TestCaseSource(nameof(_bitIndices))]
    public void AInputsInitiallyFalse(Int32 bit)
    {
        Assert.That(_block.A[bit].Value, Is.False);
    }

    [Test]
    public void AddInitiallyFalse()
    {
        Assert.That(_block.Add.Value, Is.False);
    }

    [TestCaseSource(nameof(_bitIndices))]
    public void OOutputsInitiallyFalse(Int32 bit)
    {
        Assert.That(_block.O[bit].Value, Is.False);
    }

    [Test]
    public void UpdateSetsO_ToA_WhenAddTrue()
    {
        // bits 0 -> 7
        IList<Boolean> val170 =
        [
            false,
            true,
            false,
            true,
            false,
            true,
            false,
            true,
        ];
        for (Int32 i = 0; i < _block.A.Count; i++)
        {
            _block.A[i].Value = val170[i];
        }

        // rising edge of clock
        _block.Add.Value = false;
        _block.Update();
        _block.Add.Value = true;
        _block.Update();

        var result = new List<Boolean>();
        for (Int32 i = 0; i < _block.O.Count; i++)
        {
            result.Add(_block.O[i].Value);
        }

        Assert.That(result, Is.EqualTo(val170));
    }

    public static readonly Object[] clockEdges =
    [
    new Object[] { false, false },
        new Object[] { true, false },
        new Object[] { true, true },
    ];

    [TestCaseSource(nameof(clockEdges))]
    public void Update_DoesNotUpdateO_WhenAddNotRisingEdge(Boolean edge, Boolean nextEdge)
    {
        // bits 0 -> 7
        IList<Boolean> expected =
        [
            true, // lsb
            false,
            false,
            false,
            false,
            false,
            false,
            false, // msb
        ];
        _block.A[0].Value = true;
        // rising edge of clock
        _block.Add.Value = false;
        _block.Update();
        _block.Add.Value = true;
        _block.Update();

        // test sequence of clock edges
        _block.Add.Value = edge;
        _block.Update();
        _block.Add.Value = nextEdge;
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
