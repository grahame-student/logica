using System;
using System.Collections.Generic;
using LibLogica.Blocks.Base;
using LibLogica.Blocks.Width8Bit;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Memory;

/// <summary>
/// 256 x 8-bit RAM with streaming debug info optimization
/// This approach uses IEnumerable yielding to avoid materializing large collections upfront
/// </summary>
public class Ram256x8Streaming : LogicElement
{
    private readonly BlockArray<Ram16x8> _memory;

    // Inputs
    public LogicArray<Input> Address { get; }
    public LogicArray<Input> DataIn { get; }
    public Input Write { get; } = new();

    // Outputs
    public LogicArray<Output> DataOut { get; }

    public Ram256x8Streaming()
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

    /// <summary>
    /// Streaming version that yields debug IDs on-demand without materializing the entire collection
    /// </summary>
    public override IEnumerable<String> GetIds()
    {
        // Yield local IDs first
        yield return $"{IdPrefix()}Write";
        
        // Yield address array
        for (Int32 i = Address.Count - 1; i >= 0; i--)
        {
            yield return $"{IdPrefix()}Address{i}";
        }
        
        // Yield data in array
        for (Int32 i = DataIn.Count - 1; i >= 0; i--)
        {
            yield return $"{IdPrefix()}DataIn{i}";
        }
        
        // Yield data out array
        for (Int32 i = DataOut.Count - 1; i >= 0; i--)
        {
            yield return $"{IdPrefix()}DataOut{i}";
        }
        
        // Yield children IDs
        for (Int32 i = _memory.Count - 1; i >= 0; i--)
        {
            foreach (var childId in _memory[i].GetIds())
            {
                yield return $"{IdPrefix()}{childId}";
            }
        }
    }

    /// <summary>
    /// Streaming version that yields debug values on-demand without materializing the entire collection
    /// </summary>
    public override IEnumerable<Boolean> GetValues()
    {
        // Yield local values first
        yield return Write.Value;
        
        // Yield address array values
        for (Int32 i = Address.Count - 1; i >= 0; i--)
        {
            yield return Address[i].Value;
        }
        
        // Yield data in array values
        for (Int32 i = DataIn.Count - 1; i >= 0; i--)
        {
            yield return DataIn[i].Value;
        }
        
        // Yield data out array values
        for (Int32 i = DataOut.Count - 1; i >= 0; i--)
        {
            yield return DataOut[i].Value;
        }
        
        // Yield children values
        for (Int32 i = _memory.Count - 1; i >= 0; i--)
        {
            foreach (var childValue in _memory[i].GetValues())
            {
                yield return childValue;
            }
        }
    }
}