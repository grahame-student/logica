using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;

/// <summary>
/// 8 x 8-bit RAM
/// </summary>
public class Ram8x8 : LogicElement
{
    private readonly BlockArray<Ram8x1> _memory;

    // Inputs
    public LogicArray<Input> Address { get; }
    public LogicArray<Input> DataIn { get; }
    public Input Write { get; } = new();

    // Outputs
    public LogicArray<Output> DataOut { get; }

    public Ram8x8()
    {
        _memory = new BlockArray<Ram8x1>(8);
        Address = new LogicArray<Input>(3);
        DataIn = new LogicArray<Input>(8);
        DataOut = new LogicArray<Output>(8);

        for (Int32 i = 0; i < _memory.Count; i++)
        {
            _memory[i].DataIn.Connect(DataIn[i]);
            _memory[i].Write.Connect(Write);
            DataOut[i].Connect(_memory[i].DataOut);
            for (Int32 j = 0; j < Address.Count; j++)
            {
                _memory[i].Address[j].Connect(Address[j]);
            }
        }
    }

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability
        
        for (Int32 i = 0; i < _memory.Count; i++)
        {
            _memory[i].Update();
        }
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddLocal(nameof(Write), Write)
            .AddArray(nameof(Address), Address)
            .AddArray(nameof(DataIn), DataIn)
            .AddArray(nameof(DataOut), DataOut)
            .AddChildren(_memory)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
