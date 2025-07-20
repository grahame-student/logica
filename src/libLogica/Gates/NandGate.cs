using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.IO;

namespace LibLogica.Gates;

public class NandGate : LogicElement, IBinaryGate
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

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O))
            .AddChildren(_andGate, _notGate)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
