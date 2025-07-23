using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Blocks.Width16Bit;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;
public class Ram256x8 : LogicElement
{
    private readonly Decoder4to16 _decoder = new();
    private readonly BlockArray<Ram16x8> _ramBlocks;
    private readonly SelectSignal16Bit _selectWrite = new();
    private readonly SelectSignal16Bit _selectEnable = new();

    // Inputs
    public LogicArray<Input> Address { get; }
    public LogicArray<Input> DataIn { get; }
    public Input Write { get; } = new();
    public Input Enable { get; } = new();

    // Outputs
    public LogicArray<Output> DataOut { get; }

    public Ram256x8()
    {
        _decoder = new Decoder4to16();
        _ramBlocks = new BlockArray<Ram16x8>(16);
        Address = new LogicArray<Input>(8);
        DataIn = new LogicArray<Input>(8);
        DataOut = new LogicArray<Output>(8);

        // High 4 bits of address select the RAM block
        _decoder.Address[0].Connect(Address[4]);
        _decoder.Address[1].Connect(Address[5]);
        _decoder.Address[2].Connect(Address[6]);
        _decoder.Address[3].Connect(Address[7]);

        for (Int32 j = 0; j < _ramBlocks.Count; j++)
        {
            _selectEnable.Signal.Connect(Enable);
            _selectEnable.Inputs[j].Connect(_decoder.Output[j]);

            _selectWrite.Signal.Connect(Write);
            _selectWrite.Inputs[j].Connect(_decoder.Output[j]);

            _ramBlocks[j].Write.Connect(_selectWrite.Outputs[j]);
            _ramBlocks[j].Enable.Connect(_selectEnable.Outputs[j]);

            // Connect the low 4 bits of Address to each RAM block's Address
            for (Int32 i = 0; i < 4; i++)
            {
                _ramBlocks[j].Address[i].Connect(Address[i]);
            }

            // Connect DataIn to each RAM block's DataIn
            // and DataOut to the main DataOut
            for (Int32 i = 0; i < DataIn.Count; i++)
            {
                _ramBlocks[j].DataIn[i].Connect(DataIn[i]);
                DataOut[i].Connect(_ramBlocks[j].DataOut[i]);
            }
        }
    }

    public override void Update()
    {
        ClearDebugInfoCache(); // Clear cache to ensure dynamic observability
        _decoder.Update();
        _selectEnable.Update();
        _selectWrite.Update();
        for (Int32 i = 0; i < _ramBlocks.Count; i++)
        {
            _ramBlocks[i].Update();
        }
    }

    protected override (IEnumerable<String> ids, IEnumerable<Boolean> values) GetDebugInfoInternal() =>
        DebugInfo()
            .AddArray(nameof(Address), Address)
            .AddArray(nameof(DataIn), DataIn)
            .AddArray(nameof(DataOut), DataOut)
            .AddLocal(nameof(Write), Write)
            .AddLocal(nameof(Enable), Enable)
            .AddChild(_decoder)
            .AddChild(_selectWrite)
            .AddChild(_selectEnable)
            .AddChildren(_ramBlocks)
            .Build();

    public override IEnumerable<String> GetIds() => GetIdsCached();

    public override IEnumerable<Boolean> GetValues() => GetValuesCached();
}
