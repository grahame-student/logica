using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Blocks.Width8Bit;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;

/// <summary>
/// 256 x 8-bit RAM
/// Constructed from 16 instances of Ram16x8 blocks
/// </summary>
public class Ram256x8 : LogicElement
{
    private readonly BlockArray<Ram16x8> _memory;

    // Inputs
    public LogicArray<Input> Address { get; }
    public LogicArray<Input> DataIn { get; }
    public Input Write { get; } = new();

    // Outputs
    public LogicArray<Output> DataOut { get; }

    public Ram256x8()
    {
        _memory = new BlockArray<Ram16x8>(16);
        Address = new LogicArray<Input>(8); // 8 bits to address 256 blocks
        DataIn = new LogicArray<Input>(8);
        DataOut = new LogicArray<Output>(8);

        for (Int32 i = 0; i < _memory.Count; i++)
        {
            for (Int32 j = 0; j < DataIn.Count; j++)
            {
                _memory[i].DataIn[j].Connect(DataIn[j]);
            }
            _memory[i].Write.Connect(Write);
            
            for (Int32 j = 0; j < 4; j++) // Connect lower 4 address bits
            {
                _memory[i].Address[j].Connect(Address[j]);
            }
            
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