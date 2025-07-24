using System;
using System.Collections.Generic;
using LibLogica.IO;

namespace LibLogica.Gates;

public class TristateBufferGate : LogicElement
{
    public Input A { get; } = new();
    public Input Enable { get; } = new();

    public Output O { get; } = new();

    public TristateBufferGate()
    {
        // Buffer gate starts in high impedance state
        O.IsHighImpedance = true;
    }

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability

        if (Enable.Value)
        {
            O.IsHighImpedance = false;
            O.Value = A.Value;
        }
        else
        {
            O.IsHighImpedance = true;
        }
    }

    public override IEnumerable<String> GetIds() =>
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(Enable), Enable), (nameof(O), O))
            .Build().ids;

    public override IEnumerable<Boolean> GetValues() =>
        DebugInfo()
            .AddLocals((nameof(A), A), (nameof(Enable), Enable), (nameof(O), O))
            .Build().values;
}
