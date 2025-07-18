using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.IO;

namespace LibLogica.Gates;

public class NorGate : LogicElement, IBinaryGate
{
    private readonly OrGate _orGate = new();
    private readonly NotGate _notGate = new();

    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public NorGate()
    {
        _orGate.A.Connect(A);
        _orGate.B.Connect(B);
        _notGate.A.Connect(_orGate.O);
        O.Connect(_notGate.O);
    }

    public override void Update()
    {
        _orGate.Update();
        _notGate.Update();
    }

    public override IEnumerable<String> GetIds() => GetLocalIds()
        .Concat(_orGate.GetIds().Select(x => IdPrefix() + x))
        .Concat(_notGate.GetIds().Select(x => IdPrefix() + x));

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(A)}",
        $"{IdPrefix()}{nameof(B)}",
        $"{IdPrefix()}{nameof(O)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues()
        .Concat(_orGate.GetValues())
        .Concat(_notGate.GetValues());

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        A.Value,
        B.Value,
        O.Value
    ];
}
