using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;

public class NotGate : LogicElement
{
    // Inputs
    public Input A { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override void Update() => O.Value = !A.Value;

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo().AddLocals((nameof(A), A), (nameof(O), O)).Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
