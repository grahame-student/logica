using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Base;

public abstract class WideLatch<TFlipFlop> : LogicElement, IWideLatch
    where TFlipFlop : LogicElement, IDTypeFlipFlop, new()
{
    private readonly BlockArray<TFlipFlop> _latches;

    // Inputs
    public LogicArray<Input> D { get; }
    public Input Clock { get; } = new();

    // Outputs
    public LogicArray<Output> Q { get; }

    protected WideLatch(Int32 width)
    {
        _latches = new BlockArray<TFlipFlop>(width);
        D = new LogicArray<Input>(width);
        Q = new LogicArray<Output>(width);

        for (Int32 i = 0; i < width; i++)
        {
            _latches[i].D.Connect(D[i]);
            _latches[i].Clock.Connect(Clock);
            Q[i].Connect(_latches[i].Q);
        }
    }

    public override void Update()
    {
        for (Int32 i = 0; i < _latches.Count; i++)
        {
            _latches[i].Update();
        }
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddArray(nameof(D), D)
            .AddLocal(nameof(Clock), Clock)
            .AddArray(nameof(Q), Q)
            .AddChildren(_latches)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
