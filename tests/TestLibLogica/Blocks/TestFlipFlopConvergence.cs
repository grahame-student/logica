using System;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks;

internal class TestFlipFlopConvergence
{
    [Test]
    public void SingleFlipFlop_ConvergenceAnalysis()
    {
        var flipflop = new FlipFlopEdgeTriggeredDTypeSimple();
        
        // Set up toggle configuration (D connected to NQ)
        flipflop.D.Connect(flipflop.NQ);
        
        TestContext.Out.WriteLine("Initial state:");
        TestContext.Out.WriteLine($"Q={flipflop.Q.Value}, NQ={flipflop.NQ.Value}, Clock={flipflop.Clock.Value}, D={flipflop.D.Value}");
        
        // Test multiple passes to see convergence
        TestContext.Out.WriteLine("\nTesting multiple update passes:");
        for (int pass = 1; pass <= 10; pass++)
        {
            flipflop.Update();
            TestContext.Out.WriteLine($"Pass {pass}: Q={flipflop.Q.Value}, NQ={flipflop.NQ.Value}, D={flipflop.D.Value}");
            
            // Check if we've reached a stable state
            if (pass > 1 && flipflop.Q.Value != flipflop.NQ.Value)
            {
                TestContext.Out.WriteLine($"Stable state reached at pass {pass}");
                break;
            }
        }
        
        // Now test clock edge
        TestContext.Out.WriteLine("\nTesting clock edge from low to high:");
        flipflop.Clock.Value = false;
        flipflop.Update();
        TestContext.Out.WriteLine($"Clock low: Q={flipflop.Q.Value}, NQ={flipflop.NQ.Value}, D={flipflop.D.Value}");
        
        flipflop.Clock.Value = true;
        for (int pass = 1; pass <= 10; pass++)
        {
            flipflop.Update();
            TestContext.Out.WriteLine($"Clock high, pass {pass}: Q={flipflop.Q.Value}, NQ={flipflop.NQ.Value}, D={flipflop.D.Value}");
        }
    }

    [Test]
    public void TwoConnectedFlipFlops_ConvergenceAnalysis()
    {
        var ff1 = new FlipFlopEdgeTriggeredDTypeSimple();
        var ff2 = new FlipFlopEdgeTriggeredDTypeSimple();
        
        // Set up like in ripple counter
        ff1.D.Connect(ff1.NQ);  // Toggle configuration
        ff2.D.Connect(ff2.NQ);  // Toggle configuration
        ff2.Clock.Connect(ff1.NQ);  // ff2 clock driven by ff1 output
        
        TestContext.Out.WriteLine("Two flip-flops connected as in ripple counter:");
        TestContext.Out.WriteLine("Initial state:");
        PrintFlipFlopStates(ff1, ff2);
        
        // Test clock edge transition from low to high
        ff1.Clock.Value = false;
        ff1.Update();
        ff2.Update();
        TestContext.Out.WriteLine("\nAfter setting ff1.Clock = false and updating:");
        PrintFlipFlopStates(ff1, ff2);
        
        ff1.Clock.Value = true;
        
        TestContext.Out.WriteLine("\nAfter setting ff1.Clock = true, testing convergence:");
        for (int pass = 1; pass <= 10; pass++)
        {
            ff1.Update();
            ff2.Update();
            TestContext.Out.WriteLine($"Pass {pass}:");
            PrintFlipFlopStates(ff1, ff2);
            
            // Check for convergence (no more changes)
            var prevQ1 = pass == 1 ? false : ff1.Q.Value;  // Rough check
            if (pass > 2)  // Allow some passes for convergence
            {
                // We'll see what happens
            }
        }
    }
    
    private static void PrintFlipFlopStates(FlipFlopEdgeTriggeredDTypeSimple ff1, FlipFlopEdgeTriggeredDTypeSimple ff2)
    {
        TestContext.Out.WriteLine($"  FF1: Clock={ff1.Clock.Value}, D={ff1.D.Value}, Q={ff1.Q.Value}, NQ={ff1.NQ.Value}");
        TestContext.Out.WriteLine($"  FF2: Clock={ff2.Clock.Value}, D={ff2.D.Value}, Q={ff2.Q.Value}, NQ={ff2.NQ.Value}");
    }
}