using System;

namespace LibLogica.Blocks.Base;

/// <summary>
/// Edge-triggered wide latch using D-type flip-flops
///
/// ASCII Diagram:
/// x D Type FlipFlops
/// x Clock lines common
///
///      D [x]
///        V
/// +---------------+
/// |               |
/// |  x-bit Latch  + < Clk
/// |               |
/// +---------------+
///        V
///      Q [x]
///
/// QN outputs unused
/// D -> Q   on the rising edge of clk
/// </summary>
public class LatchEdgeTriggeredWide : LatchWide<FlipFlopEdgeTriggeredDType>
{
    public LatchEdgeTriggeredWide(Int32 width) : base(width)
    {
    }
}
