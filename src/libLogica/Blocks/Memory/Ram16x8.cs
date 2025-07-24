using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Blocks.Width16Bit;
using LibLogica.Blocks.Width8Bit;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;

/// <summary>
/// 16 x 8-bit RAM
/// </summary>
public class Ram16x8 : LogicElement
{
    private readonly Decoder4to16 _decoder = new();
    private readonly BlockArray<Ram1x8> _ramBlock;
    private readonly SelectSignal16Bit _selectSignal = new();
    private readonly TristateBuffer8Bit _tristateBuffer = new();

    // Inputs
    public LogicArray<Input> DataIn { get; }
    public LogicArray<Input> Address { get; }
    public Input Write { get; } = new();
    public Input Enable { get; } = new();

    // Outputs
    public LogicArray<Output> DataOut { get; }

    public Ram16x8()
    {
        _ramBlock = new BlockArray<Ram1x8>(16);
        DataIn = new LogicArray<Input>(8);
        Address = new LogicArray<Input>(4);
        DataOut = new LogicArray<Output>(8);

        _selectSignal.Signal.Connect(Write);
        for (Int32 i = 0; i < Address.Count; i++)
        {
            _decoder.Address[i].Connect(Address[i]);
        }

        for (Int32 j = 0; j < _ramBlock.Count; j++)
        {
            _selectSignal.Inputs[j].Connect(_decoder.Output[j]);
            _ramBlock[j].Write.Connect(_selectSignal.Outputs[j]);
            _ramBlock[j].Enable.Connect(_decoder.Output[j]);
            for (Int32 i = 0; i < DataIn.Count; i++)
            {
                _ramBlock[j].DataIn[i].Connect(DataIn[i]);
                _tristateBuffer.Inputs[i].Connect(_ramBlock[j].DataOut[i]);
                _tristateBuffer.Enable.Connect(Enable);
                DataOut[i].Connect(_tristateBuffer.Outputs[i]);
            }
        }
    }

    public override void Update()
    {
        ClearDebugInfoCache(); // Clear cache to ensure dynamic observability for educational use
        _decoder.Update();
        _selectSignal.Update();
        for (Int32 i = 0; i < _ramBlock.Count; i++)
        {
            _ramBlock[i].Update();
        }
        _tristateBuffer.Update();

        // TODO: Validate busses after update
        //       We should have at most one RAM block enabled at a time.
        //       No point logging failures until the update logic is complete
    }

    protected override (IEnumerable<String> ids, IEnumerable<Boolean> values) GetDebugInfoInternal() =>
        DebugInfo()
            .AddArray(nameof(DataIn), DataIn)
            .AddArray(nameof(Address), Address)
            .AddLocal(nameof(Write), Write)
            .AddLocal(nameof(Enable), Enable)
            .AddArray(nameof(DataOut), DataOut)
            .AddChild(_decoder)
            .AddChildren(_ramBlock)
            .Build();

    public override IEnumerable<String> GetIds() => GetIdsCached();

    public override IEnumerable<Boolean> GetValues() => GetValuesCached();
}
