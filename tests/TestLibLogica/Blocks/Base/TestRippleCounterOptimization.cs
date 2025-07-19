using System;
using LibLogica.Blocks.Base;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Base;

internal class TestRippleCounterOptimization
{
    private RippleCounter _counter;

    [SetUp]
    public void SetUp()
    {
        _counter = new RippleCounter(8);
    }

    [Test]
    public void SinglePassUpdate_ProducesCorrectResults()
    {
        // Test the single-pass approach against known expected values
        var expectedValues = new uint[] { 0, 1, 2, 3, 4, 5, 255, 0, 1 };
        
        for (int cycle = 0; cycle < expectedValues.Length; cycle++)
        {
            // Simulate clock edge with single pass
            _counter.Clk.Value = false;
            UpdateSinglePass();
            _counter.Clk.Value = true;
            UpdateSinglePass();
            
            var actualValue = GetCounterValue();
            TestContext.Out.WriteLine($"Cycle {cycle}: Expected {expectedValues[cycle]}, Actual {actualValue}");
            Assert.That(actualValue, Is.EqualTo(expectedValues[cycle]), 
                $"Single pass failed at cycle {cycle}");
        }
    }

    [Test]
    public void CompareMultiPassVsSinglePass()
    {
        // Test several cycles and compare results
        for (int cycle = 0; cycle < 10; cycle++)
        {
            // Create two identical counters
            var multiPassCounter = new RippleCounter(8);
            var singlePassCounter = new RippleCounter(8);
            
            // Apply same number of clock cycles to both
            for (int i = 0; i <= cycle; i++)
            {
                // Multi-pass counter (current implementation)
                multiPassCounter.Clk.Value = false;
                multiPassCounter.Update();
                multiPassCounter.Clk.Value = true;
                multiPassCounter.Update();
                
                // Single-pass counter
                singlePassCounter.Clk.Value = false;
                UpdateSinglePass(singlePassCounter);
                singlePassCounter.Clk.Value = true;
                UpdateSinglePass(singlePassCounter);
            }
            
            var multiPassValue = GetCounterValue(multiPassCounter);
            var singlePassValue = GetCounterValue(singlePassCounter);
            
            TestContext.Out.WriteLine($"Cycle {cycle}: MultiPass={multiPassValue}, SinglePass={singlePassValue}");
            Assert.That(singlePassValue, Is.EqualTo(multiPassValue), 
                $"Results differ at cycle {cycle}: MultiPass={multiPassValue}, SinglePass={singlePassValue}");
        }
    }

    private void UpdateSinglePass(RippleCounter? counter = null)
    {
        counter ??= _counter;
        counter.UpdateSinglePass();
    }

    private uint GetCounterValue(RippleCounter? counter = null)
    {
        counter ??= _counter;
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