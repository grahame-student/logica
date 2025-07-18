using System;
using LibLogica.Blocks;

namespace LibLogica.Blocks.Base;

/// <summary>
/// Level-triggered wide latch using D-type flip-flops
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
/// D -> Q   when   clk == 1
/// </summary>
public class WideLatchLevelTriggered : WideLatch<FlipFlopLevelTriggeredDType>
{
    public WideLatchLevelTriggered(Int32 width) : base(width)
    {
    }
}
