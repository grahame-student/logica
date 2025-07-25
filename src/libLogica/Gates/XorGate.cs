using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.IO;

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
        ClearValuesCache(); // Always clear values cache for educational observability

        _orGate.Update();
        _nandGate.Update();
        _andGate.Update();
    }

    public override IEnumerable<String> GetIds() =>
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O))
            .AddChildren(_orGate, _nandGate, _andGate)
            .Build().ids;

    public override IEnumerable<Boolean> GetValues() =>
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O))
            .AddChildren(_orGate, _nandGate, _andGate)
            .Build().values;
}
