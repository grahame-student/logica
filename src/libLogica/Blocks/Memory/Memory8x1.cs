using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;
public class Memory8x1 : LogicElement
{
    private readonly BlockArray<Memory1Bit> _memory;

    // Inputs
    public Input DataIn { get; } = new();
    public LogicArray<Input> Write { get; }

    // Outputs
    public LogicArray<Output> DataOut { get; }

    public Memory8x1()
    {
        _memory = new BlockArray<Memory1Bit>(8);
        Write = new LogicArray<Input>(8);
        DataOut = new LogicArray<Output>(8);
        for (Int32 i = 0; i < 8; i++)
        {
            _memory[i].DataIn.Connect(DataIn);
            _memory[i].Write.Connect(Write[i]);
            DataOut[i].Connect(_memory[i].DataOut);
        }
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddArray(nameof(Write), Write)
            .AddLocal(nameof(DataIn), DataIn)
            .AddArray(nameof(DataOut), DataOut)
            .AddChildren(_memory)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability

        for (Int32 i = 0; i < _memory.Count; i++)
        {
            _memory[i].Update();
        }
    }
}
