using System;
using LibLogica.Blocks;

namespace LibLogica.Blocks.Base;

public class WideLatchLevelTriggered : WideLatch<FlipFlopLevelTriggeredDType>
{
    public WideLatchLevelTriggered(Int32 width) : base(width)
    {
    }
}
