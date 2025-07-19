using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;
public class NorGate3Input : LogicElement
{
    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();
    public Input C { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override IEnumerable<String> GetIds() => GetLocalIds();

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(A)}",
        $"{IdPrefix()}{nameof(B)}",
        $"{IdPrefix()}{nameof(C)}",
        $"{IdPrefix()}{nameof(O)}",
    ];

    public override void Update() => O.Value = !(A.Value || B.Value || C.Value);

    public override IEnumerable<Boolean> GetValues() => GetLocalValues();

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        A.Value,
        B.Value,
        C.Value,
        O.Value
    ];
}
