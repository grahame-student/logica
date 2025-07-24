using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;

public class AndGate4Input : LogicElement
{
    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();
    public Input C { get; } = new();
    public Input D { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability
        
        var newValue = A.Value && B.Value && C.Value && D.Value;
        O.Value = newValue;
    }

    public override IEnumerable<String> GetIds() => 
        DebugInfo()
            .AddLocal(nameof(A), A)
            .AddLocal(nameof(B), B)
            .AddLocal(nameof(C), C)
            .AddLocal(nameof(D), D)
            .AddLocal(nameof(O), O)
            .Build().ids;

    public override IEnumerable<Boolean> GetValues() => 
        DebugInfo()
            .AddLocal(nameof(A), A)
            .AddLocal(nameof(B), B)
            .AddLocal(nameof(C), C)
            .AddLocal(nameof(D), D)
            .AddLocal(nameof(O), O)
            .Build().values;
}
