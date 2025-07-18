using LibLogica.IO;

using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLogica.Gates;

public class XorGate : LogicElement, IBinaryGate
{
    private readonly OrGate _orGate = new();
    private readonly NandGate _nandGate = new();
    private readonly AndGate _andGate = new();

    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public XorGate()
    {
        _orGate.A.Connect(A);
        _orGate.B.Connect(B);

        _nandGate.A.Connect(A);
        _nandGate.B.Connect(B);

        _andGate.A.Connect(_orGate.O);
        _andGate.B.Connect(_nandGate.O);

        O.Connect(_andGate.O);
    }

    public override void Update()
    {
        _orGate.Update();
        _nandGate.Update();
        _andGate.Update();
    }

    public override IEnumerable<String> GetIds() => GetLocalIds()
        .Concat(_orGate.GetIds().Select(x => IdPrefix() + x))
        .Concat(_nandGate.GetIds().Select(x => IdPrefix() + x))
        .Concat(_andGate.GetIds().Select(x => IdPrefix() + x));

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(A)}",
        $"{IdPrefix()}{nameof(B)}",
        $"{IdPrefix()}{nameof(O)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues()
        .Concat(_orGate.GetValues())
        .Concat(_nandGate.GetValues())
        .Concat(_andGate.GetValues());

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        A.Value,
        B.Value,
        O.Value
    ];
}
