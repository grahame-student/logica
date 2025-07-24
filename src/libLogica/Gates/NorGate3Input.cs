using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;
public class NorGate3Input : LogicElement
{
    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();
    public Input C { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability
        
        O.Value = !(A.Value || B.Value || C.Value);
    }

    public override IEnumerable<String> GetIds() => 
        DebugInfo().AddLocals((nameof(A), A), (nameof(B), B), (nameof(C), C), (nameof(O), O)).Build().ids;

    public override IEnumerable<Boolean> GetValues() => 
        DebugInfo().AddLocals((nameof(A), A), (nameof(B), B), (nameof(C), C), (nameof(O), O)).Build().values;
}
