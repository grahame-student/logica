using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Base;

public class WideLatchLevelTriggered : LogicElement
{
    private readonly BlockArray<FlipFlopLevelTriggeredDType> _latches;

    // Inputs
    public LogicArray<Input> D { get; }
    public Input Clock { get; } = new();

    // Outputs
    public LogicArray<Output> Q { get; }

    public WideLatchLevelTriggered(Int32 width)
    {
        _latches = new BlockArray<FlipFlopLevelTriggeredDType>(width);
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

    public override IEnumerable<String> GetIds()
    {
        IEnumerable<String> result = GetLocalIds();
        for (Int32 i = 0; i < _latches.Count; i++)
        {
            result = result.Concat(_latches[i].GetIds().Select(x => IdPrefix() + x));
        }
        return result;
    }

    protected override IEnumerable<String> GetLocalIds()
    {
        IEnumerable<String> result = new List<String>();
        for (Int32 i = D.Count - 1; i >= 0; i--)
        {
            result = result.Append($"{IdPrefix()}{nameof(D)}{i}");
        }
        result = result.Append($"{IdPrefix()}{nameof(Clock)}");
        for (Int32 i = Q.Count - 1; i >= 0; i--)
        {
            result = result.Append($"{IdPrefix()}{nameof(Q)}{i}");
        }

        return result;
    }

    public override IEnumerable<Boolean> GetValues()
    {
        IEnumerable<Boolean> result = GetLocalValues();
        for (Int32 i = 0; i < _latches.Count; i++)
        {
            result = result.Concat(_latches[i].GetValues());
        }
        return result;
    }

    protected override IEnumerable<Boolean> GetLocalValues()
    {
        IEnumerable<Boolean> result = new List<Boolean>();
        for (Int32 i = D.Count - 1; i >= 0; i--)
        {
            result = result.Append(D[i].Value);
        }
        result = result.Append(Clock.Value);
        for (Int32 i = Q.Count - 1; i >= 0; i--)
        {
            result = result.Append(Q[i].Value);
        }

        return result;
    }
}
