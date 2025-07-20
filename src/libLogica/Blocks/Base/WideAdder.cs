using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Base;

public class WideAdder : LogicElement
{
    private readonly BlockArray<FullAdder> _adders;

    // Inputs
    public LogicArray<Input> A { get; }
    public LogicArray<Input> B { get; }
    public Input CarryIn { get; } = new();

    // Outputs
    public LogicArray<Output> SumOut { get; }
    public Output CarryOut { get; } = new();

    public WideAdder(Int32 width)
    {
        _adders = new BlockArray<FullAdder>(width);
        A = new LogicArray<Input>(width);
        B = new LogicArray<Input>(width);
        SumOut = new LogicArray<Output>(width);

        _adders[0].CarryIn.Connect(CarryIn);
        for (Int32 i = 0; i < _adders.Count; i++)
        {
            _adders[i].A.Connect(A[i]);
            _adders[i].B.Connect(B[i]);
            if (i < _adders.Count - 1)
            {
                _adders[i + 1].CarryIn.Connect(_adders[i].CarryOut);
            }
            else
            {
                CarryOut.Connect(_adders[i].CarryOut);
            }
            SumOut[i].Connect(_adders[i].SumOut);
        }
    }

    public override void Update()
    {
        for (Int32 i = 0; i < _adders.Count; i++)
        {
            _adders[i].Update();
        }
    }

    public override IEnumerable<String> GetIds()
    {
        var builder = DebugInfo()
            .AddArray(nameof(A), A)
            .AddArray(nameof(B), B)
            .AddLocal(nameof(CarryIn), CarryIn)
            .AddArray(nameof(SumOut), SumOut)
            .AddLocal(nameof(CarryOut), CarryOut);
            
        for (Int32 i = _adders.Count - 1; i >= 0; i--)
        {
            builder.AddChild(_adders[i]);
        }
        
        return builder.BuildIds();
    }

    protected override IEnumerable<String> GetLocalIds()
    {
        IEnumerable<String> result = new List<String>();
        for (Int32 i = A.Count - 1; i >= 0; i--)
        {
            result = result.Append($"{IdPrefix()}{nameof(A)}{i}");
        }
        for (Int32 i = B.Count - 1; i >= 0; i--)
        {
            result = result.Append($"{IdPrefix()}{nameof(B)}{i}");
        }
        result = result.Append($"{IdPrefix()}{nameof(CarryIn)}");
        for (Int32 i = SumOut.Count - 1; i >= 0; i--)
        {
            result = result.Append($"{IdPrefix()}{nameof(SumOut)}{i}");
        }
        result = result.Append($"{IdPrefix()}{nameof(CarryOut)}");

        return result;
    }

    public override IEnumerable<Boolean> GetValues()
    {
        var builder = DebugInfo()
            .AddArray(nameof(A), A)
            .AddArray(nameof(B), B)
            .AddLocal(nameof(CarryIn), CarryIn)
            .AddArray(nameof(SumOut), SumOut)
            .AddLocal(nameof(CarryOut), CarryOut);
            
        for (Int32 i = 0; i < _adders.Count; i++)
        {
            builder.AddChild(_adders[i]);
        }
        
        return builder.BuildValues();
    }

    protected override IEnumerable<Boolean> GetLocalValues()
    {
        IEnumerable<Boolean> result = new List<Boolean>();
        for (Int32 i = A.Count - 1; i >= 0; i--)
        {
            result = result.Append(A[i].Value);
        }
        for (Int32 i = B.Count - 1; i >= 0; i--)
        {
            result = result.Append(B[i].Value);
        }
        result = result.Append(CarryIn.Value);
        for (Int32 i = SumOut.Count - 1; i >= 0; i--)
        {
            result = result.Append(SumOut[i].Value);
        }
        result = result.Append(CarryOut.Value);

        return result;
    }
}
