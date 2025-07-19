using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;

public class FlipFlopEdgeTriggeredDTypeSimple : LogicElement, IDTypeFlipFlop
{
    private readonly NotGate _not1 = new();
    private readonly NorGate3Input _nor1 = new();
    private readonly NorGate3Input _nor2 = new();
    private readonly NorGate3Input _nor3 = new();
    private readonly NorGate3Input _nor4 = new();
    private readonly NorGate3Input _nor5 = new();
    private readonly NorGate3Input _nor6 = new();

    // Inputs
    public Input D { get; } = new();
    public Input Clock { get; } = new();
    public Input Clear { get; } = new();
    public Input Preset { get; } = new();

    // Outputs
    public Output Q { get; } = new();
    public Output NQ { get; } = new();

    public FlipFlopEdgeTriggeredDTypeSimple()
    {
        _not1.A.Connect(Clock);
        _nor1.A.Connect(Clear);
        _nor1.B.Connect(_nor4.O);
        _nor1.C.Connect(_nor2.O);
        _nor2.A.Connect(_nor1.O);
        _nor2.B.Connect(Preset);
        _nor2.C.Connect(_not1.O);
        _nor3.A.Connect(_nor2.O);
        _nor3.B.Connect(_not1.O);
        _nor3.C.Connect(_nor4.O);
        _nor4.A.Connect(_nor3.O);
        _nor4.B.Connect(D);
        _nor4.C.Connect(Preset);
        _nor5.A.Connect(Clear);
        _nor5.B.Connect(_nor2.O);
        _nor5.C.Connect(_nor6.O);
        _nor6.A.Connect(_nor5.O);
        _nor6.B.Connect(_nor3.O);
        _nor6.C.Connect(Preset);

        Q.Connect(_nor5.O);
        NQ.Connect(_nor6.O);

        // Initialize the flip-flop to a known state
        InitializeState();
    }

    public override void Update()
    {
        PerformUpdate();
    }

    private void InitializeState()
    {
        PerformUpdate();
    }

    private void PerformUpdate()
    {
        _not1.Update();
        _nor1.Update();
        _nor2.Update();
        _nor3.Update();
        _nor4.Update();
        _nor6.Update(); // Order is important to ensure correct logic flow
        _nor5.Update(); // Unit test cases depend on this order
    }

    public override IEnumerable<String> GetIds() => GetLocalIds()
        .Concat(_not1.GetIds().Select(x => IdPrefix() + x))
        .Concat(_nor1.GetIds().Select(x => IdPrefix() + x))
        .Concat(_nor2.GetIds().Select(x => IdPrefix() + x))
        .Concat(_nor3.GetIds().Select(x => IdPrefix() + x))
        .Concat(_nor4.GetIds().Select(x => IdPrefix() + x))
        .Concat(_nor5.GetIds().Select(x => IdPrefix() + x))
        .Concat(_nor6.GetIds().Select(x => IdPrefix() + x));

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(D)}",
        $"{IdPrefix()}{nameof(Clock)}",
        $"{IdPrefix()}{nameof(Clear)}",
        $"{IdPrefix()}{nameof(Preset)}",
        $"{IdPrefix()}{nameof(Q)}",
        $"{IdPrefix()}{nameof(NQ)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues()
        .Concat(_not1.GetValues())
        .Concat(_nor1.GetValues())
        .Concat(_nor2.GetValues())
        .Concat(_nor3.GetValues())
        .Concat(_nor4.GetValues())
        .Concat(_nor5.GetValues())
        .Concat(_nor6.GetValues());

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        D.Value,
        Clock.Value,
        Clear.Value,
        Preset.Value,
        Q.Value,
        NQ.Value
    ];
}
