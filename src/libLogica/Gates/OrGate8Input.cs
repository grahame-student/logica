using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;
public class OrGate8Input : LogicElement
{
    // Inputs
    public Input A { get; } = new();
    public Input B { get; } = new();
    public Input C { get; } = new();
    public Input D { get; } = new();
    public Input E { get; } = new();
    public Input F { get; } = new();
    public Input G { get; } = new();
    public Input H { get; } = new();

    // Outputs
    public Output O { get; } = new();

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability

        O.Value = A.Value || B.Value || C.Value || D.Value || E.Value || F.Value || G.Value || H.Value;
    }

    public override IEnumerable<String> GetIds() =>
        DebugInfo()
            .AddLocals(
                (nameof(A), A),
                (nameof(B), B),
                (nameof(C), C),
                (nameof(D), D),
                (nameof(E), E),
                (nameof(F), F),
                (nameof(G), G),
                (nameof(H), H),
                (nameof(O), O))
            .Build().ids;

    public override IEnumerable<Boolean> GetValues() =>
        DebugInfo()
            .AddLocals(
                (nameof(A), A),
                (nameof(B), B),
                (nameof(C), C),
                (nameof(D), D),
                (nameof(E), E),
                (nameof(F), F),
                (nameof(G), G),
                (nameof(H), H),
                (nameof(O), O))
            .Build().values;

}
