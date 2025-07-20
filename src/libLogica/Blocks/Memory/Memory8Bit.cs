using System;
using System.Collections.Generic;
using System.Linq;
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

    public override IEnumerable<String> GetIds()
    {
        IEnumerable<String> result = GetLocalIds();
        for (Int32 i = _memoryBits.Count - 1; i >= 0; i--)
        {
            result = result.Concat(_memoryBits[i].GetIds().Select(x => IdPrefix() + x));
        }
        return result;
    }

    protected override IEnumerable<String> GetLocalIds()
    {
        return
        [
            $"{IdPrefix()}{nameof(DataIn)}",
            $"{IdPrefix()}{nameof(Write)}",
            $"{IdPrefix()}{nameof(DataOut)}"
        ];
    }

    public override IEnumerable<Boolean> GetValues()
    {
        IEnumerable<Boolean> result = GetLocalValues();
        for (Int32 i = _memoryBits.Count - 1; i >= 0; i--)
        {
            result = result.Concat(_memoryBits[i].GetValues());
        }
        return result;
    }

    protected override IEnumerable<Boolean> GetLocalValues()
    {
        IEnumerable<Boolean> result = new List<Boolean>();
        for (Int32 i = 0; i < DataIn.Count; i++)
        {
            result = result.Append(DataIn[i].Value);
        }
        result = result.Append(Write.Value);
        for (Int32 i = 0; i < DataOut.Count; i++)
        {
            result = result.Append(DataOut[i].Value);
        }
        return result;
    }
}
