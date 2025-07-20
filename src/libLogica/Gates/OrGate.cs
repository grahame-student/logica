using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;

public class OrGate : LogicElement, IBinaryGate
{
    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override void Update() => O.Value = A.Value || B.Value;

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo().AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O)).Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(A)}",
        $"{IdPrefix()}{nameof(B)}",
        $"{IdPrefix()}{nameof(O)}",
    ];

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        A.Value,
        B.Value,
        O.Value
    ];
}
