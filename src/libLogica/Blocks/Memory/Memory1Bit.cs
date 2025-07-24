using System;
using System.Collections.Generic;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;
public class Memory1Bit : LogicElement
{
    private readonly FlipFlopLevelTriggeredDType _flipFlop;

    // Inputs
    public Input DataIn { get; } = new();
    public Input Write { get; } = new();

    // Outputs
    public Output DataOut { get; } = new();

    public Memory1Bit()
    {
        _flipFlop = new FlipFlopLevelTriggeredDType();
        _flipFlop.D.Connect(DataIn);
        _flipFlop.Clock.Connect(Write);
        DataOut.Connect(_flipFlop.Q);
    }

    public override void Update()
    {
        _flipFlop.Update();
        // For composite gates, we conservatively mark state as changed since child state might have changed
        MarkStateChanged();
        ClearDebugInfoCacheIfChanged();
    }

    protected override (IEnumerable<String> ids, IEnumerable<Boolean> values) GetDebugInfoInternal() =>
        DebugInfo()
            .AddLocal(nameof(DataIn), DataIn)
            .AddLocal(nameof(Write), Write)
            .AddLocal(nameof(DataOut), DataOut)
            .AddChild(_flipFlop)
            .Build();

    public override IEnumerable<String> GetIds() => GetIdsCached();

    public override IEnumerable<Boolean> GetValues() => GetValuesCached();
}
