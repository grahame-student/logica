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

    public override IEnumerable<String> GetIds() => GetLocalIds()
        .Concat(_not.GetIds().Select(x => IdPrefix() + x));

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(O)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues()
        .Concat(_not.GetValues());

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        O.Value
    ];
}
