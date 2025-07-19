using System;
using LibLogica.Blocks.Base;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Base;

internal class TestRippleCounterDetailed
{
    [Test]
    public void DetailedRippleCounterAnalysis()
    {
        var counter = new RippleCounter(4); // Smaller for easier analysis
        
        TestContext.Out.WriteLine("=== Initial State ===");
        PrintCounterState(counter);
        
        TestContext.Out.WriteLine("\n=== Clock Low ===");
        counter.Clk.Value = false;
        counter.Update();
        PrintCounterState(counter);
        
        TestContext.Out.WriteLine("\n=== Clock High - Multi-pass vs Single-pass ===");
        
        // Test with multi-pass (current implementation)
        var multiPassCounter = new RippleCounter(4);
        multiPassCounter.Clk.Value = false;
        multiPassCounter.Update();
        TestContext.Out.WriteLine("\nMulti-pass counter before clock high:");
        PrintCounterState(multiPassCounter);
        
        multiPassCounter.Clk.Value = true;
        multiPassCounter.Update();
        TestContext.Out.WriteLine("Multi-pass counter after clock high:");
        PrintCounterState(multiPassCounter);
        
        // Test with single-pass
        var singlePassCounter = new RippleCounter(4);
        singlePassCounter.Clk.Value = false;
        singlePassCounter.UpdateSinglePass();
        TestContext.Out.WriteLine("\nSingle-pass counter before clock high:");
        PrintCounterState(singlePassCounter);
        
        singlePassCounter.Clk.Value = true;
        singlePassCounter.UpdateSinglePass();
        TestContext.Out.WriteLine("Single-pass counter after clock high:");
        PrintCounterState(singlePassCounter);
        
        // Compare values
        TestContext.Out.WriteLine($"\nComparison: Multi-pass value = {GetCounterValue(multiPassCounter)}, Single-pass value = {GetCounterValue(singlePassCounter)}");
        
        TestContext.Out.WriteLine("\n=== Testing Multiple Clock Cycles ===");
        for (int cycle = 1; cycle <= 5; cycle++)
        {
            // Multi-pass
            multiPassCounter.Clk.Value = false;
            multiPassCounter.Update();
            multiPassCounter.Clk.Value = true;
            multiPassCounter.Update();
            
            // Single-pass  
            singlePassCounter.Clk.Value = false;
            singlePassCounter.UpdateSinglePass();
            singlePassCounter.Clk.Value = true;
            singlePassCounter.UpdateSinglePass();
            
            TestContext.Out.WriteLine($"Cycle {cycle}: Multi-pass = {GetCounterValue(multiPassCounter)}, Single-pass = {GetCounterValue(singlePassCounter)}");
        }
    }
    
    private static void PrintCounterState(RippleCounter counter)
    {
        TestContext.Out.WriteLine($"Counter value: {GetCounterValue(counter)}");
        TestContext.Out.WriteLine($"Clock: {counter.Clk.Value}");
        for (int i = 0; i < counter.Q.Count; i++)
        {
            TestContext.Out.WriteLine($"  Q[{i}]: {counter.Q[i].Value}");
        }
    }
    
    private static uint GetCounterValue(RippleCounter counter)
    {
        uint value = 0;
        for (int i = 0; i < counter.Q.Count; i++)
        {
            if (counter.Q[i].Value)
            {
                value |= 1u << i;
            }
        }
        return value;
    }
}