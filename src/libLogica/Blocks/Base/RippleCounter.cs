using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Base;

public class RippleCounter : LogicElement
{
    private readonly BlockArray<FlipFlopEdgeTriggeredDType> _flipflops;

    // Inputs
    public Input Clk { get; } = new();

    // Outputs
    public LogicArray<Output> Q { get; }

    public RippleCounter(Int32 width)
    {
        _flipflops = new BlockArray<FlipFlopEdgeTriggeredDType>(width);
        Q = new LogicArray<Output>(width);

        _flipflops[0].Clock.Connect(Clk);
        for (Int32 i = 0; i < _flipflops.Count; i++)
        {
            Q[i].Connect(_flipflops[i].Q);
            _flipflops[i].D.Connect(_flipflops[i].NQ);
            if (i > 0)
            {
                // Connect the clock of each flip-flop to the NQ output of the previous flip-flop
                _flipflops[i].Clock.Connect(_flipflops[i - 1].NQ);
            }
        }
    }

    public override void Update()
    {
        // Optimized update using change detection to achieve O(n) complexity.
        // Instead of always doing n×n updates, we continue updating until
        // the circuit stabilizes (no more changes occur).
        Boolean hasChanges;
        Int32 pass = 0;
        const Int32 maxPasses = 32; // Safety limit to prevent infinite loops

        do
        {
            hasChanges = false;

            // Capture the current state before updating
            Boolean[] previousStates = new Boolean[_flipflops.Count];
            for (Int32 i = 0; i < _flipflops.Count; i++)
            {
                previousStates[i] = _flipflops[i].Q.Value;
            }

            // Update all flip-flops once
            for (Int32 i = 0; i < _flipflops.Count; i++)
            {
                _flipflops[i].Update();
            }

            // Check if any outputs changed
            for (Int32 i = 0; i < _flipflops.Count; i++)
            {
                if (_flipflops[i].Q.Value != previousStates[i])
                {
                    hasChanges = true;
                    break;
                }
            }

            pass++;
        } while (hasChanges && pass < maxPasses);
    }

    public override IEnumerable<String> GetIds()
    {
        IEnumerable<String> result = GetLocalIds();
        for (Int32 i = _flipflops.Count - 1; i >= 0; i--)
        {
            result = result.Concat(_flipflops[i].GetIds().Select(x => IdPrefix() + x));
        }
        return result;
    }

    protected override IEnumerable<String> GetLocalIds()
    {
        IEnumerable<String> result = [];
        result = result.Append($"{IdPrefix()}{nameof(Clk)}");
        for (Int32 i = Q.Count - 1; i >= 0; i--)
        {
            result = result.Append($"{IdPrefix()}{nameof(Q)}{i}");
        }

        return result;
    }

    public override IEnumerable<Boolean> GetValues()
    {
        IEnumerable<Boolean> result = GetLocalValues();
        for (Int32 i = 0; i < _flipflops.Count; i++)
        {
            result = result.Concat(_flipflops[i].GetValues());
        }
        return result;
    }

    protected override IEnumerable<Boolean> GetLocalValues()
    {
        IEnumerable<Boolean> result = [];
        result = result.Append(Clk.Value);
        for (Int32 i = Q.Count - 1; i >= 0; i--)
        {
            result = result.Append(Q[i].Value);
        }

        return result;
    }
}
