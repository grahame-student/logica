using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;

public class AndGate : LogicElement, IBinaryGate
{
    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override void Update()
    {
        var newValue = A.Value && B.Value;
        if (O.Value != newValue)
        {
            O.Value = newValue;
            MarkStateChanged();
        }
        ClearDebugInfoCacheIfChanged();
    }

    protected override (IEnumerable<String> ids, IEnumerable<Boolean> values) GetDebugInfoInternal() =>
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O))
            .Build();

    public override IEnumerable<String> GetIds() => GetIdsCached();

    public override IEnumerable<Boolean> GetValues() => GetValuesCached();
}
