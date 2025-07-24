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
        ClearValuesCache(); // Always clear values cache for educational observability
        
        _orGate.Update();
        _notGate.Update();
    }

    public override IEnumerable<String> GetIds() => 
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O))
            .AddChildren(_orGate, _notGate)
            .Build().ids;

    public override IEnumerable<Boolean> GetValues() => 
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O))
            .AddChildren(_orGate, _notGate)
            .Build().values;
}
