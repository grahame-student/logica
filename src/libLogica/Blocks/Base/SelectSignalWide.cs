using System;
using System.Collections.Generic;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Base;

public class SelectSignalWide : LogicElement
{
    private readonly BlockArray<AndGate> _andGate;

    // Inputs
    public LogicArray<Input> Inputs { get; }
    public Input Signal { get; } = new();

    // Outputs
    public LogicArray<Output> Outputs { get; }

    public SelectSignalWide(Int32 width)
    {
        _andGate = new BlockArray<AndGate>(width);
        Inputs = new LogicArray<Input>(width);
        Outputs = new LogicArray<Output>(width);
        for (Int32 i = 0; i < _andGate.Count; i++)
        {
            _andGate[i].A.Connect(Inputs[i]);
            _andGate[i].B.Connect(Signal);
            Outputs[i].Connect(_andGate[i].O);
        }
    }

    public override void Update()
    {
        for (Int32 i = 0; i < _andGate.Count; i++)
        {
            _andGate[i].Update();
        }
        // For composite gates, we conservatively mark state as changed since child state might have changed
        MarkStateChanged();
        ClearDebugInfoCacheIfChanged();
    }

    protected override (IEnumerable<String> ids, IEnumerable<Boolean> values) GetDebugInfoInternal() =>
        DebugInfo()
            .AddArray(nameof(Inputs), Inputs)
            .AddLocal(nameof(Signal), Signal)
            .AddArray(nameof(Outputs), Outputs)
            .Build();

    public override IEnumerable<String> GetIds() => GetIdsCached();

    public override IEnumerable<Boolean> GetValues() => GetValuesCached();
}
