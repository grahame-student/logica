using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Blocks.Width8Bit;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;

/// <summary>
/// 16 x 8-bit RAM
/// Constructed from 16 instances of Ram1x8 blocks
/// </summary>
public class Ram16x8 : LogicElement
{
    private readonly BlockArray<Ram1x8> _memory;

    // Inputs
    public LogicArray<Input> Address { get; }
    public LogicArray<Input> DataIn { get; }
    public Input Write { get; } = new();

    // Outputs
    public LogicArray<Output> DataOut { get; }

    public Ram16x8()
    {
        _memory = new BlockArray<Ram1x8>(16);
        Address = new LogicArray<Input>(4); // 4 bits to address 16 blocks
        DataIn = new LogicArray<Input>(8);
        DataOut = new LogicArray<Output>(8);

        for (Int32 i = 0; i < _memory.Count; i++)
        {
            for (Int32 j = 0; j < DataIn.Count; j++)
            {
                _memory[i].DataIn[j].Connect(DataIn[j]);
            }
            _memory[i].Write.Connect(Write);
            _memory[i].Enable.Connect(Address[0]); // Simplified addressing for now
            
            for (Int32 j = 0; j < DataOut.Count; j++)
            {
                DataOut[j].Connect(_memory[i].DataOut[j]);
            }
        }
    }

    public override void Update()
    {
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

    public override IEnumerable<String> GetIds() => GetCachedDebugInfo(BuildDebugInfo).ids;

    public override IEnumerable<Boolean> GetValues() => GetCachedDebugInfo(BuildDebugInfo).values;
}