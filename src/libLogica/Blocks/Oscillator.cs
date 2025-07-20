using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;

public class Oscillator : LogicElement
{
    private readonly NotGate _not = new();

    // Outputs
    public Output O { get; } = new();

    public Oscillator()
    {
        O.Connect(_not.O);
        _not.A.Connect(_not.O);
    }

    public override void Update() => _not.Update();

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddLocals((nameof(O), O))
            .AddChildren(_not)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
