using System;
using System.Collections.Generic;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;

public class HalfAdder : LogicElement
{
    private readonly XorGate _xorGate = new();
    private readonly AndGate _andGate = new();

    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();

    // Outputs
    public Output SumOut { get; } = new();
    public Output CarryOut { get; } = new();

    public HalfAdder()
    {
        _xorGate.A.Connect(A);
        _xorGate.B.Connect(B);
        _andGate.A.Connect(A);
        _andGate.B.Connect(B);
        SumOut.Connect(_xorGate.O);
        CarryOut.Connect(_andGate.O);
    }

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability
        
        _xorGate.Update();
        _andGate.Update();
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(B), B), (nameof(SumOut), SumOut), (nameof(CarryOut), CarryOut))
            .AddChildren(_xorGate, _andGate)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
