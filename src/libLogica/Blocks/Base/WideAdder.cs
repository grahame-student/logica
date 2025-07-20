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

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddArray(nameof(A), A)
            .AddArray(nameof(B), B)
            .AddLocal(nameof(CarryIn), CarryIn)
            .AddArray(nameof(SumOut), SumOut)
            .AddLocal(nameof(CarryOut), CarryOut)
            .AddChildren(_adders)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
