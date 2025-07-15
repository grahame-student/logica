using LibLogica.IO;

using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLogica.Gates;

public class NandGate : LogicElement
{
    private readonly AndGate _andGate = new();
    private readonly NotGate _notGate = new();

    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public NandGate()
    {
        _andGate.A.Connect(A);
        _andGate.B.Connect(B);
        _notGate.A.Connect(_andGate.O);
        O.Connect(_notGate.O);
    }

    public override void Update()
    {
        _andGate.Update();
        _notGate.Update();
    }

    public override IEnumerable<String> GetIds() => GetLocalIds()
        .Concat(_andGate.GetIds().Select(x => IdPrefix() + x))
        .Concat(_notGate.GetIds().Select(x => IdPrefix() + x));

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(A)}",
        $"{IdPrefix()}{nameof(B)}",
        $"{IdPrefix()}{nameof(O)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues()
        .Concat(_andGate.GetValues())
        .Concat(_notGate.GetValues());

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        A.Value,
        B.Value,
        O.Value
    ];
}
