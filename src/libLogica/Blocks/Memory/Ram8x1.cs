using System;
using System.Collections.Generic;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;

/// <summary>
/// 8 x 1-bit RAM
/// </summary>
public class Ram8x1 : LogicElement
{
    private readonly Decoder3to8 _decoder;
    private readonly Memory8x1 _memory;
    private readonly Selector8to1 _selector;

    // Inputs
    public LogicArray<Input> Address { get; }
    public Input Write { get; } = new();
    public Input DataIn { get; } = new();

    // Outputs
    public Output DataOut { get; } = new();

    public Ram8x1()
    {
        _decoder = new Decoder3to8();
        _memory = new Memory8x1();
        _selector = new Selector8to1();
        Address = new LogicArray<Input>(3);

        for (Int32 i = 0; i < 3; i++)
        {
            _decoder.Address[i].Connect(Address[i]);
            _selector.Address[i].Connect(Address[i]);
        }
        _decoder.Write.Connect(Write);
        _memory.DataIn.Connect(DataIn);
        _memory.Write.Connect(_decoder.Output);
        _selector.Input.Connect(_memory.DataOut);
        DataOut.Connect(_selector.Output);
    }

    public override void Update()
    {
        _decoder.Update();
        _memory.Update();
        _selector.Update();
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddLocal(nameof(DataIn), DataIn)
            .AddLocal(nameof(Write), Write)
            .AddArray(nameof(Address), Address)
            .AddLocal(nameof(DataOut), DataOut)
            .AddChildren(_decoder)
            .AddChildren(_memory)
            .AddChildren(_selector)
            .Build();

    public override IEnumerable<String> GetIds() => GetCachedDebugInfo(BuildDebugInfo).ids;

    public override IEnumerable<Boolean> GetValues() => GetCachedDebugInfo(BuildDebugInfo).values;
}
