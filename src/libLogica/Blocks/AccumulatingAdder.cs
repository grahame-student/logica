using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Blocks.Width8Bit;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;

public abstract class AccumulatingAdder<TLatch> : LogicElement
    where TLatch : LogicElement, ILatch8Bit, new()
{
    private readonly Adder8Bit _adder8 = new();
    private readonly TLatch _latch8 = new();

    // Inputs
    public LogicArray<Input> A { get; } = new(8);
    public Input Add { get; } = new();

    // Outputs
    public LogicArray<Input> O { get; } = new(8);

    protected AccumulatingAdder()
    {
        _adder8.B.Connect(A);
        _latch8.D.Connect(_adder8.SumOut);
        _latch8.Clock.Connect(Add);
        A.Connect(_latch8.Q);
        O.Connect(_latch8.Q);
    }

    public override void Update()
    {
        _adder8.Update();
        _latch8.Update();
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddArray(nameof(A), A)
            .AddLocal(nameof(Add), Add)
            .AddArray(nameof(O), O)
            .AddChildren(_adder8, _latch8)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;

    protected override IEnumerable<String> GetLocalIds() => [];

    protected override IEnumerable<Boolean> GetLocalValues() => [];
}
