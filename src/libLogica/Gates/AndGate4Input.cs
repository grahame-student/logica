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

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddLocal(nameof(A), A)
            .AddLocal(nameof(B), B)
            .AddLocal(nameof(C), C)
            .AddLocal(nameof(D), D)
            .AddLocal(nameof(O), O)
            .Build();

    public override IEnumerable<System.String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<System.Boolean> GetValues() => BuildDebugInfo().values;

    public override void Update()
    {
        O.Value = A.Value && B.Value && C.Value && D.Value;
    }
}
