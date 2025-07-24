using System;
using System.Collections.Generic;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Base;

public class TristateBufferWide : LogicElement
{
    private readonly BlockArray<TristateBufferGate> _buffers;

    // Inputs
    public LogicArray<Input> Inputs { get; }
    public Input Enable { get; } = new();

    // Outputs
    public LogicArray<Output> Outputs { get; }

    public TristateBufferWide() : this(8) { }

    public TristateBufferWide(Int32 width)
    {
        _buffers = new BlockArray<TristateBufferGate>(width);
        Inputs = new LogicArray<Input>(width);
        Outputs = new LogicArray<Output>(width);
        for (Int32 i = 0; i < _buffers.Count; i++)
        {
            _buffers[i].A.Connect(Inputs[i]);
            _buffers[i].Enable.Connect(Enable);
            Outputs[i].Connect(_buffers[i].O);
        }
    }

    public override void Update()
    {
        for (Int32 i = 0; i < _buffers.Count; i++)
        {
            _buffers[i].Update();
        }
        // For composite gates, we conservatively mark state as changed since child state might have changed
        MarkStateChanged();
        ClearDebugInfoCacheIfChanged();
    }

    protected override (IEnumerable<String> ids, IEnumerable<Boolean> values) GetDebugInfoInternal() =>
        DebugInfo()
            .AddArray(nameof(Inputs), Inputs)
            .AddLocal(nameof(Enable), Enable)
            .AddArray(nameof(Outputs), Outputs)
            .Build();

    public override IEnumerable<String> GetIds() => GetIdsCached();

    public override IEnumerable<Boolean> GetValues() => GetValuesCached();
}
