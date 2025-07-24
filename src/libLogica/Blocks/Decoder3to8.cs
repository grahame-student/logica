using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Blocks.Base;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;
public class Decoder3to8 : LogicElement
{
    private readonly BlockArray<NotGate> _notGates = new(3);
    private readonly BlockArray<AndGate4Input> _andGates = new(8);

    // Inputs
    public LogicArray<Input> Address { get; }
    public Input Write { get; } = new();

    // Outputs
    public LogicArray<Output> Output { get; }

    public Decoder3to8()
    {
        Address = new LogicArray<Input>(3);
        Output = new LogicArray<Output>(8);
        // Connect the not gates to the address inputs
        for (Int32 i = 0; i < 3; i++)
        {
            _notGates[i].A.Connect(Address[i]);
        }
        // Connect the outputs to the AND gates' Outputs
        // Connect the Write input to the AND gates
        for (Int32 i = 0; i < 8; i++)
        {
            Output[i].Connect(_andGates[i].O);
            _andGates[i].D.Connect(Write);
        }
        // Connect the AND gates to the NOT gates and Address inputs
        _andGates[0].A.Connect(_notGates[2].O);
        _andGates[0].B.Connect(_notGates[1].O);
        _andGates[0].C.Connect(_notGates[0].O);
        _andGates[1].A.Connect(_notGates[2].O);
        _andGates[1].B.Connect(_notGates[1].O);
        _andGates[1].C.Connect(Address[0]);
        _andGates[2].A.Connect(_notGates[2].O);
        _andGates[2].B.Connect(Address[1]);
        _andGates[2].C.Connect(_notGates[0].O);
        _andGates[3].A.Connect(_notGates[2].O);
        _andGates[3].B.Connect(Address[1]);
        _andGates[3].C.Connect(Address[0]);
        _andGates[4].A.Connect(Address[2]);
        _andGates[4].B.Connect(_notGates[1].O);
        _andGates[4].C.Connect(_notGates[0].O);
        _andGates[5].A.Connect(Address[2]);
        _andGates[5].B.Connect(_notGates[1].O);
        _andGates[5].C.Connect(Address[0]);
        _andGates[6].A.Connect(Address[2]);
        _andGates[6].B.Connect(Address[1]);
        _andGates[6].C.Connect(_notGates[0].O);
        _andGates[7].A.Connect(Address[2]);
        _andGates[7].B.Connect(Address[1]);
        _andGates[7].C.Connect(Address[0]);
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo()
    {
        return DebugInfo()
            .AddArray(nameof(Address), Address)
            .AddLocal(nameof(Write), Write)
            .AddArray(nameof(Output), Output)
            .AddChildren(_notGates)
            .AddChildren(_andGates)
            .Build();
    }

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability// Update the NOT gates
        for (Int32 i = 0; i < _notGates.Count; i++)
        {
            _notGates[i].Update();
        }
        // Update the AND gates
        for (Int32 i = 0; i < _andGates.Count; i++)
        {
            _andGates[i].Update();
        }
    }
}
