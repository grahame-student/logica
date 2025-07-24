using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;

public class Selector8to1 : LogicElement
{
    private readonly BlockArray<NotGate> _notgates;
    private readonly BlockArray<AndGate4Input> _andgates;

    private readonly OrGate8Input _orGate = new();

    // Inputs
    public LogicArray<Input> Address { get; }
    public LogicArray<Input> Input { get; }

    // Outputs
    public Output Output { get; } = new();

    public Selector8to1()
    {
        Address = new LogicArray<Input>(3);
        Input = new LogicArray<Input>(8);
        _notgates = new BlockArray<NotGate>(3);
        _andgates = new BlockArray<AndGate4Input>(8);

        for (Int32 i = 0; i < 3; i++)
        {
            _notgates[i].A.Connect(Address[i]);
        }
        for (Int32 i = 0; i < 8; i++)
        {
            _andgates[i].D.Connect(Input[i]);
        }
        _andgates[0].A.Connect(_notgates[2].O);
        _andgates[0].B.Connect(_notgates[1].O);
        _andgates[0].C.Connect(_notgates[0].O);
        _andgates[1].A.Connect(_notgates[2].O);
        _andgates[1].B.Connect(_notgates[1].O);
        _andgates[1].C.Connect(Address[0]);
        _andgates[2].A.Connect(_notgates[2].O);
        _andgates[2].B.Connect(Address[1]);
        _andgates[2].C.Connect(_notgates[0].O);
        _andgates[3].A.Connect(_notgates[2].O);
        _andgates[3].B.Connect(Address[1]);
        _andgates[3].C.Connect(Address[0]);
        _andgates[4].A.Connect(Address[2]);
        _andgates[4].B.Connect(_notgates[1].O);
        _andgates[4].C.Connect(_notgates[0].O);
        _andgates[5].A.Connect(Address[2]);
        _andgates[5].B.Connect(_notgates[1].O);
        _andgates[5].C.Connect(Address[0]);
        _andgates[6].A.Connect(Address[2]);
        _andgates[6].B.Connect(Address[1]);
        _andgates[6].C.Connect(_notgates[0].O);
        _andgates[7].A.Connect(Address[2]);
        _andgates[7].B.Connect(Address[1]);
        _andgates[7].C.Connect(Address[0]);

        _orGate.A.Connect(_andgates[7].O);
        _orGate.B.Connect(_andgates[6].O);
        _orGate.C.Connect(_andgates[5].O);
        _orGate.D.Connect(_andgates[4].O);
        _orGate.E.Connect(_andgates[3].O);
        _orGate.F.Connect(_andgates[2].O);
        _orGate.G.Connect(_andgates[1].O);
        _orGate.H.Connect(_andgates[0].O);
        Output.Connect(_orGate.O);
    }

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability

        for (Int32 i = 0; i < _notgates.Count; i++)
        {
            _notgates[i].Update();
        }
        for (Int32 i = 0; i < _andgates.Count; i++)
        {
            _andgates[i].Update();
        }
        _orGate.Update();
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddArray(nameof(Address), Address)
            .AddArray(nameof(Input), Input)
            .AddLocal(nameof(Output), Output)
            .AddChildren(_notgates)
            .AddChildren(_andgates)
            .AddChild(_orGate)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
