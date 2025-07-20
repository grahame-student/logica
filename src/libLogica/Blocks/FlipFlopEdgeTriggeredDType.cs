using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;

public class FlipFlopEdgeTriggeredDType : LogicElement, IDTypeFlipFlop
{
    private readonly NotGate _not1 = new();
    private readonly NotGate _not2 = new();
    private readonly AndGate _and1 = new();
    private readonly AndGate _and2 = new();
    private readonly FlipFlopRS _rs1 = new();
    private readonly AndGate _and3 = new();
    private readonly AndGate _and4 = new();
    private readonly FlipFlopRS _rs2 = new();

    // Inputs
    public Input D { get; } = new();
    public Input Clock { get; } = new();

    // Outputs
    public Output Q { get; } = new();
    public Output NQ { get; } = new();

    public FlipFlopEdgeTriggeredDType()
    {
        _not1.A.Connect(Clock);
        _not2.A.Connect(D);
        _and1.A.Connect(D);
        _and1.B.Connect(_not1.O);
        _and2.A.Connect(_not1.O);
        _and2.B.Connect(_not2.O);
        _rs1.R.Connect(_and1.O);
        _rs1.S.Connect(_and2.O);
        _and3.A.Connect(_rs1.Q);
        _and3.B.Connect(Clock);
        _and4.A.Connect(Clock);
        _and4.B.Connect(_rs1.NQ);
        _rs2.R.Connect(_and3.O);
        _rs2.S.Connect(_and4.O);
        Q.Connect(_rs2.Q);
        NQ.Connect(_rs2.NQ);

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
        _not2.Update();
        _and1.Update();
        _and2.Update();
        _rs1.Update();
        _and3.Update();
        _and4.Update();
        _rs2.Update();
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddLocals((nameof(D), D), (nameof(Clock), Clock), (nameof(Q), Q), (nameof(NQ), NQ))
            .AddChildren(_not1, _not2, _and1, _and2, _rs1, _and3, _and4, _rs2)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
