using System;
using LibLogica.Blocks;

namespace LibLogica.Blocks.Base;

public class WideLatchEdgeTriggered : WideLatch<FlipFlopEdgeTriggeredDType>
{
    public WideLatchEdgeTriggered(Int32 width) : base(width)
    {
    }
}
