using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Blocks.Width8Bit;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;

/// <summary>
/// 1 x 8-bit RAM
/// </summary>
public class Ram1x8 : LogicElement
{
    private readonly BlockArray<Memory1Bit> _memory;
    private readonly TristateBuffer8Bit _tristateBuffer;

    // Inputs
    public LogicArray<Input> DataIn { get; }
    public Input Write { get; } = new();
    public Input Enable { get; } = new();

    // Outputs
    public LogicArray<Output> DataOut { get; }

    public Ram1x8()
    {
        DataIn = new LogicArray<Input>(8);
        DataOut = new LogicArray<Output>(8);

        _memory = new BlockArray<Memory1Bit>(8);
        _tristateBuffer = new TristateBuffer8Bit();

        for (Int32 i = 0; i < _memory.Count; i++)
        {
            _memory[i].Write.Connect(Write);
            _memory[i].DataIn.Connect(DataIn[i]);
            _tristateBuffer.Inputs[i].Connect(_memory[i].DataOut);
            DataOut[i].Connect(_tristateBuffer.Outputs[i]);
        }
        _tristateBuffer.Enable.Connect(Enable);
    }

    public override void Update()
    {
        ClearValuesCache(); // Always clear values cache for educational observability
        
        for (Int32 i = 0; i < _memory.Count; i++)
        {
            _memory[i].Update();
        }
        _tristateBuffer.Update();
    }

    public override IEnumerable<String> GetIds() => 
        DebugInfo()
            .AddArray(nameof(DataIn), DataIn)
            .AddLocal(nameof(Write), Write)
            .AddLocal(nameof(Enable), Enable)
            .AddArray(nameof(DataOut), DataOut)
            .AddChildren(_memory)
            .AddChild(_tristateBuffer)
            .Build().ids;

    public override IEnumerable<Boolean> GetValues() => 
        DebugInfo()
            .AddArray(nameof(DataIn), DataIn)
            .AddLocal(nameof(Write), Write)
            .AddLocal(nameof(Enable), Enable)
            .AddArray(nameof(DataOut), DataOut)
            .AddChildren(_memory)
            .AddChild(_tristateBuffer)
            .Build().values;
}
