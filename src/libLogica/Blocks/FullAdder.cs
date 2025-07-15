using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.IO;

namespace LibLogica.Blocks;

using LibLogica.Gates;


public class FullAdder : LogicElement
{
    private readonly HalfAdder _ha1 = new();
    private readonly HalfAdder _ha2 = new();
    private readonly OrGate _orGate = new();

    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();
    public Input CarryIn { get; } = new();

    // Outputs
    public Output SumOut { get; } = new();
    public Output CarryOut { get; } = new();

    public FullAdder()
    {
        _ha1.A.Connect(A);
        _ha1.B.Connect(B);
        _ha2.A.Connect(CarryIn);
        _ha2.B.Connect(_ha1.SumOut);
        _orGate.A.Connect(_ha2.CarryOut);
        _orGate.B.Connect(_ha1.CarryOut);
        SumOut.Connect(_ha2.SumOut);
        CarryOut.Connect(_orGate.O);
    }

    public override void Update()
    {
        _ha1.Update();
        _ha2.Update();
        _orGate.Update();
    }

    public override IEnumerable<String> GetIds() => GetLocalIds()
        .Concat(_ha1.GetIds().Select(x => IdPrefix() + x))
        .Concat(_ha2.GetIds().Select(x => IdPrefix() + x))
        .Concat(_orGate.GetIds().Select(x => IdPrefix() + x));

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(A)}",
        $"{IdPrefix()}{nameof(B)}",
        $"{IdPrefix()}{nameof(CarryIn)}",
        $"{IdPrefix()}{nameof(SumOut)}",
        $"{IdPrefix()}{nameof(CarryOut)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues()
        .Concat(_ha1.GetValues())
        .Concat(_ha2.GetValues())
        .Concat(_orGate.GetValues());

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        A.Value,
        B.Value,
        CarryIn.Value,
        SumOut.Value,
        CarryOut.Value,
    ];
}
