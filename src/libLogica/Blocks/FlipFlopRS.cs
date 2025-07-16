using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.IO;

namespace LibLogica.Blocks;

using Gates;

public class FlipFlopRS : LogicElement
{
    private readonly NorGate _nor1 = new NorGate();
    private readonly NorGate _nor2 = new NorGate();

    // Inputs
    public Input R { get; } = new();
    public Input S { get; } = new();

    // Outputs
    public Output Q { get; } = new();
    public Output NQ { get; } = new();

    public FlipFlopRS()
    {
        _nor1.A.Connect(R);
        _nor1.B.Connect(_nor2.O);
        _nor2.A.Connect(_nor1.O);
        _nor2.B.Connect(S);
        Q.Connect(_nor1.O);
        NQ.Connect(_nor2.O);

        // Put flip-flop into known starting state
        // R S Q NQ
        // 0 0 0 1
        R.Value = true;
        _InitializeState();
        R.Value = false;
        _InitializeState();
    }

    public override void Update()
    {
        _InitializeState();
    }

    private void _InitializeState()
    {
        // Order is important, unit tests will fail if swapped
        _nor2.Update();
        _nor1.Update();
    }

    public override IEnumerable<String> GetIds() => GetLocalIds()
        .Concat(_nor1.GetIds().Select(x => IdPrefix() + x))
        .Concat(_nor2.GetIds().Select(x => IdPrefix() + x));

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(R)}",
        $"{IdPrefix()}{nameof(S)}",
        $"{IdPrefix()}{nameof(Q)}",
        $"{IdPrefix()}{nameof(NQ)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues()
        .Concat(_nor1.GetValues())
        .Concat(_nor2.GetValues());

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        R.Value,
        S.Value,
        Q.Value,
        NQ.Value,
    ];
}
