using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Base;

public class RippleCounter : LogicElement
{
    private readonly BlockArray<FlipFlopEdgeTriggeredDTypeSimple> _flipflops;

    // Inputs
    public Input Clk { get; } = new();

    // Outputs
    public LogicArray<Output> Q { get; }

    public RippleCounter(Int32 width)
    {
        _flipflops = new BlockArray<FlipFlopEdgeTriggeredDTypeSimple>(width);
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
        // Optimized adaptive multi-pass update
        // 
        // PROBLEM: Originally, this used a fixed number of passes (8), but the requirement
        // was to optimize the O(n²) complexity that would occur if passes = n.
        // 
        // SOLUTION: Instead of always doing a fixed number of passes, we do only
        // as many passes as needed until the system converges to a stable state.
        // This significantly reduces the number of updates needed on average.
        // 
        // ANALYSIS: In a ripple counter, when the external clock triggers:
        // - Most cycles: only FF0 toggles (1 pass needed)
        // - Every 2nd cycle: FF0 and FF1 toggle (2 passes needed)  
        // - Every 4th cycle: FF0, FF1, FF2 toggle (3 passes needed)
        // - Every 2^k cycle: FF0 through FFk toggle (k+1 passes needed)
        // 
        // The average number of passes is much less than n, making this approach
        // much more efficient than always doing n passes.
        
        Int32 maxPasses = _flipflops.Count; // Theoretical maximum: each flip-flop can affect the next
        
        for (Int32 pass = 0; pass < maxPasses; pass++)
        {
            Boolean anyChanged = false;
            
            // Update all flip-flops in this pass
            for (Int32 i = 0; i < _flipflops.Count; i++)
            {
                Boolean prevQ = _flipflops[i].Q.Value;
                Boolean prevNQ = _flipflops[i].NQ.Value;
                
                _flipflops[i].Update();
                
                // Check if this flip-flop changed state
                if (_flipflops[i].Q.Value != prevQ || _flipflops[i].NQ.Value != prevNQ)
                {
                    anyChanged = true;
                }
            }
            
            // If no flip-flops changed in this pass, we've reached steady state
            if (!anyChanged)
            {
                // Converged! No more passes needed.
                // In most cases, this happens much sooner than maxPasses.
                break;
            }
        }
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
