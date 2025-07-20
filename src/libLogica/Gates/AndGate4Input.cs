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

    protected new DebugInfoBuilder DebugInfo()
    {
        return base.DebugInfo()
            .AddLocal(nameof(A), A)
            .AddLocal(nameof(B), B)
            .AddLocal(nameof(C), C)
            .AddLocal(nameof(D), D)
            .AddLocal(nameof(O), O);
    }

    public override IEnumerable<System.String> GetIds() => DebugInfo().BuildIds();

    public override IEnumerable<System.Boolean> GetValues() => DebugInfo().BuildValues();

    public override void Update()
    {
        O.Value = A.Value && B.Value && C.Value && D.Value;
    }
}
