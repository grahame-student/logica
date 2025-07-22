using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;
public class Memory8Bit : LogicElement
{
    private readonly BlockArray<Memory1Bit> _memoryBits;

    // Inputs
    public LogicArray<Input> DataIn { get; }
    public Input Write { get; } = new();

    // Outputs
    public LogicArray<Output> DataOut { get; }

    public Memory8Bit()
    {
        _memoryBits = new BlockArray<Memory1Bit>(8);
        DataIn = new LogicArray<Input>(8);
        DataOut = new LogicArray<Output>(8);
        for (Int32 i = 0; i < _memoryBits.Count; i++)
        {
            _memoryBits[i].DataIn.Connect(DataIn[i]);
            _memoryBits[i].Write.Connect(Write);
            DataOut[i].Connect(_memoryBits[i].DataOut);
        }
    }

    public override void Update()
    {
        for (Int32 i = 0; i < _memoryBits.Count; i++)
        {
            _memoryBits[i].Update();
        }
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo()
    {
        // Convert BlockArray to array for AddChildren
        var memoryChildren = new LogicElement[_memoryBits.Count];
        for (Int32 i = 0; i < _memoryBits.Count; i++)
        {
            memoryChildren[i] = _memoryBits[i];
        }

        return DebugInfo()
            .AddArray(nameof(DataIn), DataIn)
            .AddLocal(nameof(Write), Write)
            .AddArray(nameof(DataOut), DataOut)
            .AddChildren(memoryChildren)
            .Build();
    }

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
