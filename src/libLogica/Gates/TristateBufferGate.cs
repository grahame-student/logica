using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;

public class TristateBufferGate : LogicElement
{
    public Input A { get; } = new();
    public Input Enable { get; } = new();

    public Output O { get; } = new();

    public TristateBufferGate()
    {
        // Buffer gate starts in high impedance state
        O.IsHighImpedance = true;
    }

    public override void Update()
    {
        var oldValue = O.Value;
        var oldImpedance = O.IsHighImpedance;
        
        if (Enable.Value)
        {
            O.IsHighImpedance = false;
            O.Value = A.Value;
        }
        else
        {
            O.IsHighImpedance = true;
        }
        
        if (O.Value != oldValue || O.IsHighImpedance != oldImpedance)
        {
            MarkStateChanged();
        }
        ClearDebugInfoCacheIfChanged();
    }

    protected override (IEnumerable<String> ids, IEnumerable<Boolean> values) GetDebugInfoInternal() =>
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(Enable), Enable), (nameof(O), O))
            .Build();

    public override IEnumerable<String> GetIds() => GetIdsCached();

    public override IEnumerable<Boolean> GetValues() => GetValuesCached();
}
