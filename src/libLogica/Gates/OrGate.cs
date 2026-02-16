using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;

public class OrGate : LogicElement, IBinaryGate
{
    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability

        var newValue = A.Value || B.Value;
        O.Value = newValue;
    }

    public override IEnumerable<String> GetIds() =>
        DebugInfo().AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O)).Build().ids;

    public override IEnumerable<Boolean> GetValues() =>
        DebugInfo().AddLocals((nameof(A), A), (nameof(B), B), (nameof(O), O)).Build().values;
}
