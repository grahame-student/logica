using LibLogica.IO;

using System;
using System.Collections.Generic;

namespace LibLogica.Gates;

public class AndGate : LogicElement, IBinaryGate
{
    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override void Update() => O.Value = A.Value && B.Value;

    public override IEnumerable<String> GetIds() => GetLocalIds();

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(A)}",
        $"{IdPrefix()}{nameof(B)}",
        $"{IdPrefix()}{nameof(O)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues();

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        A.Value,
        B.Value,
        O.Value
    ];
}
