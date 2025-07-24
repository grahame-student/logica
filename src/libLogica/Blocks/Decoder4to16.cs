using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;

public class Decoder4to16 : LogicElement
{
    private readonly BlockArray<NotGate> _notGates = new(4);
    private readonly BlockArray<AndGate4Input> _andGates = new(16);

    public LogicArray<Input> Address { get; } = new(4);
    public LogicArray<Output> Output { get; } = new(16);

    public Decoder4to16()
    {
        for (Int32 i = 0; i < 4; i++)
        {
            _notGates[i].A.Connect(Address[i]);
        }
        for (Int32 i = 0; i < 16; i++)
        {
            Output[i].Connect(_andGates[i].O);
        }
        _andGates[0].A.Connect(_notGates[3].O);
        _andGates[0].B.Connect(_notGates[2].O);
        _andGates[0].C.Connect(_notGates[1].O);
        _andGates[0].D.Connect(_notGates[0].O);
        _andGates[1].A.Connect(_notGates[3].O);
        _andGates[1].B.Connect(_notGates[2].O);
        _andGates[1].C.Connect(_notGates[1].O);
        _andGates[1].D.Connect(Address[0]);
        _andGates[2].A.Connect(_notGates[3].O);
        _andGates[2].B.Connect(_notGates[2].O);
        _andGates[2].C.Connect(Address[1]);
        _andGates[2].D.Connect(_notGates[0].O);
        _andGates[3].A.Connect(_notGates[3].O);
        _andGates[3].B.Connect(_notGates[2].O);
        _andGates[3].C.Connect(Address[1]);
        _andGates[3].D.Connect(Address[0]);
        _andGates[4].A.Connect(_notGates[3].O);
        _andGates[4].B.Connect(Address[2]);
        _andGates[4].C.Connect(_notGates[1].O);
        _andGates[4].D.Connect(_notGates[0].O);
        _andGates[5].A.Connect(_notGates[3].O);
        _andGates[5].B.Connect(Address[2]);
        _andGates[5].C.Connect(_notGates[1].O);
        _andGates[5].D.Connect(Address[0]);
        _andGates[6].A.Connect(_notGates[3].O);
        _andGates[6].B.Connect(Address[2]);
        _andGates[6].C.Connect(Address[1]);
        _andGates[6].D.Connect(_notGates[0].O);
        _andGates[7].A.Connect(_notGates[3].O);
        _andGates[7].B.Connect(Address[2]);
        _andGates[7].C.Connect(Address[1]);
        _andGates[7].D.Connect(Address[0]);
        _andGates[8].A.Connect(Address[3]);
        _andGates[8].B.Connect(_notGates[2].O);
        _andGates[8].C.Connect(_notGates[1].O);
        _andGates[8].D.Connect(_notGates[0].O);
        _andGates[9].A.Connect(Address[3]);
        _andGates[9].B.Connect(_notGates[2].O);
        _andGates[9].C.Connect(_notGates[1].O);
        _andGates[9].D.Connect(Address[0]);
        _andGates[10].A.Connect(Address[3]);
        _andGates[10].B.Connect(_notGates[2].O);
        _andGates[10].C.Connect(Address[1]);
        _andGates[10].D.Connect(_notGates[0].O);
        _andGates[11].A.Connect(Address[3]);
        _andGates[11].B.Connect(_notGates[2].O);
        _andGates[11].C.Connect(Address[1]);
        _andGates[11].D.Connect(Address[0]);
        _andGates[12].A.Connect(Address[3]);
        _andGates[12].B.Connect(Address[2]);
        _andGates[12].C.Connect(_notGates[1].O);
        _andGates[12].D.Connect(_notGates[0].O);
        _andGates[13].A.Connect(Address[3]);
        _andGates[13].B.Connect(Address[2]);
        _andGates[13].C.Connect(_notGates[1].O);
        _andGates[13].D.Connect(Address[0]);
        _andGates[14].A.Connect(Address[3]);
        _andGates[14].B.Connect(Address[2]);
        _andGates[14].C.Connect(Address[1]);
        _andGates[14].D.Connect(_notGates[0].O);
        _andGates[15].A.Connect(Address[3]);
        _andGates[15].B.Connect(Address[2]);
        _andGates[15].C.Connect(Address[1]);
        _andGates[15].D.Connect(Address[0]);
    }

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability
        
        for (Int32 i = 0; i < _notGates.Count; i++)
        {
            _notGates[i].Update();
        }
        for (Int32 i = 0; i < _andGates.Count; i++)
        {
            _andGates[i].Update();
        }
    }

    public override IEnumerable<String> GetIds() => 
        DebugInfo()
            .AddArray(nameof(Address), Address)
            .AddArray(nameof(Output), Output)
            .AddChildren(_notGates)
            .AddChildren(_andGates)
            .Build().ids;

    public override IEnumerable<Boolean> GetValues() => 
        DebugInfo()
            .AddArray(nameof(Address), Address)
            .AddArray(nameof(Output), Output)
            .AddChildren(_notGates)
            .AddChildren(_andGates)
            .Build().values;
}
