using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;

public class AndGate4Input : LogicElement
{
    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();
    public Input C { get; } = new();
    public Input D { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override void Update()
    {
        var newValue = A.Value && B.Value && C.Value && D.Value;
        if (O.Value != newValue)
        {
            O.Value = newValue;
            MarkStateChanged();
        }
        ClearDebugInfoCacheIfChanged();
    }

    protected override (IEnumerable<String> ids, IEnumerable<Boolean> values) GetDebugInfoInternal() =>
        DebugInfo()
            .AddLocal(nameof(A), A)
            .AddLocal(nameof(B), B)
            .AddLocal(nameof(C), C)
            .AddLocal(nameof(D), D)
            .AddLocal(nameof(O), O)
            .Build();

    public override IEnumerable<System.String> GetIds() => GetIdsCached();

    public override IEnumerable<System.Boolean> GetValues() => GetValuesCached();
}
