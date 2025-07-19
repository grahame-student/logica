using System;
using LibLogica.Blocks;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Analysis;

[TestFixture]
public class TestBlockUpdateAnalysis
{
    [Test]
    public void AnalyzeUpdateRequirements()
    {
        Console.WriteLine("=== Block Analysis: Testing Update Requirements ===\n");
        
        // Test level-triggered gates (should stabilize in 1 update)
        TestLevelTriggeredGates();
        
        // Test level-triggered blocks (should stabilize in 1 update)  
        TestLevelTriggeredBlocks();
        
        // Test edge-triggered blocks (should stabilize in 2 updates)
        TestEdgeTriggeredBlocks();
    }
    
    private void TestLevelTriggeredGates()
    {
        Console.WriteLine("--- Level-Triggered Gates (Expected: 1 update) ---");
        
        // Test NotGate - change from false to true
        Console.WriteLine("Testing NotGate (false -> true):");
        var notGate = new NotGate();
        notGate.A.Value = false; // Input false, should output true
        int updates = CountUpdatesUntilStable(() => notGate.Update(), () => notGate.O.Value);
        Console.WriteLine($"NotGate: {updates} updates to stabilize\n");
        
        // Test AndGate - both inputs true
        Console.WriteLine("Testing AndGate (true && true):");
        var andGate = new AndGate();
        andGate.A.Value = true;
        andGate.B.Value = true; // Should output true
        updates = CountUpdatesUntilStable(() => andGate.Update(), () => andGate.O.Value);
        Console.WriteLine($"AndGate: {updates} updates to stabilize\n");
        
        // Test NorGate - both inputs false
        Console.WriteLine("Testing NorGate (false || false):");
        var norGate = new NorGate();
        norGate.A.Value = false;
        norGate.B.Value = false; // Should output true
        updates = CountUpdatesUntilStable(() => norGate.Update(), () => norGate.O.Value);
        Console.WriteLine($"NorGate: {updates} updates to stabilize\n");
        
        Console.WriteLine();
    }
    
    private void TestLevelTriggeredBlocks()
    {
        Console.WriteLine("--- Level-Triggered Blocks (Expected: 1 update) ---");
        
        // Test FlipFlopRS - needs special handling for cross-coupled feedback
        var ffRS = new FlipFlopRS();
        ffRS.S.Value = true;
        ffRS.R.Value = false;
        int updates = CountUpdatesUntilStableComplex(
            () => ffRS.Update(), 
            () => (ffRS.Q.Value, ffRS.NQ.Value));
        Console.WriteLine($"FlipFlopRS: {updates} updates to stabilize");
        
        // Test FlipFlopLevelTriggeredDType
        var ffLevel = new FlipFlopLevelTriggeredDType();
        ffLevel.D.Value = true;
        ffLevel.Clock.Value = true;
        updates = CountUpdatesUntilStableComplex(
            () => ffLevel.Update(), 
            () => (ffLevel.Q.Value, ffLevel.NQ.Value));
        Console.WriteLine($"FlipFlopLevelTriggeredDType: {updates} updates to stabilize");
        
        Console.WriteLine();
    }
    
    private void TestEdgeTriggeredBlocks()
    {
        Console.WriteLine("--- Edge-Triggered Blocks (Expected: 2 updates) ---");
        
        // Test FlipFlopEdgeTriggeredDType with proper edge simulation
        Console.WriteLine("Testing FlipFlopEdgeTriggeredDType (rising edge D=1):");
        var ffEdge = new FlipFlopEdgeTriggeredDType();
        ffEdge.D.Value = true;
        ffEdge.Clock.Value = false; // Start with clock low
        
        // Perform initial update to establish clock low state
        ffEdge.Update(); 
        Console.WriteLine($"    After initial setup - Clock: {ffEdge.Clock.Value}, D: {ffEdge.D.Value}, Q: {ffEdge.Q.Value}");
        
        // Now create rising edge and measure stabilization
        ffEdge.Clock.Value = true;
        Console.WriteLine($"    After clock rise - Clock: {ffEdge.Clock.Value}, D: {ffEdge.D.Value}");
        int updates = CountUpdatesUntilStableComplex(
            () => ffEdge.Update(), 
            () => (ffEdge.Q.Value, ffEdge.NQ.Value));
        Console.WriteLine($"FlipFlopEdgeTriggeredDType: {updates} updates to stabilize after rising edge\n");
        
        Console.WriteLine();
    }
    
    private int CountUpdatesUntilStable(Action updateFunc, Func<bool> getValue)
    {
        // Get initial output value (before any updates)
        bool initialValue = getValue();
        int updateCount = 0;
        const int maxUpdates = 10; // Safety limit
        
        bool finalStableValue = initialValue;
        
        // Keep updating until we find the stable value
        do
        {
            updateFunc();
            updateCount++;
            bool currentValue = getValue();
            
            // If value changed, this might be the new stable value
            if (currentValue != finalStableValue)
            {
                finalStableValue = currentValue;
            }
            
            // Test if this is stable by doing one more update
            updateFunc();
            bool testValue = getValue();
            
            if (testValue == finalStableValue)
            {
                // Found stable state
                Console.WriteLine($"    Initial: {initialValue}, Final: {finalStableValue}, Updates: {updateCount}");
                return updateCount;
            }
            else
            {
                // Not stable yet, continue (but count the extra update we just did)
                updateCount++;
                finalStableValue = testValue;
            }
        } 
        while (updateCount < maxUpdates);
        
        Console.WriteLine($"    Initial: {initialValue}, Final: {finalStableValue}, Updates: {updateCount} (max reached)");
        return updateCount;
    }
    
    private int CountUpdatesUntilStableComplex<T>(Action updateFunc, Func<T> getValue) where T : IEquatable<T>
    {
        // Get initial output value (before any updates)  
        T initialValue = getValue();
        int updateCount = 0;
        const int maxUpdates = 10; // Safety limit
        
        T finalStableValue = initialValue;
        
        // Keep updating until we find the stable value
        do
        {
            updateFunc();
            updateCount++;
            T currentValue = getValue();
            
            // If value changed, this might be the new stable value
            if (!currentValue.Equals(finalStableValue))
            {
                finalStableValue = currentValue;
            }
            
            // Test if this is stable by doing one more update
            updateFunc();
            T testValue = getValue();
            
            if (testValue.Equals(finalStableValue))
            {
                // Found stable state
                Console.WriteLine($"    Initial: {initialValue}, Final: {finalStableValue}, Updates: {updateCount}");
                return updateCount;
            }
            else
            {
                // Not stable yet, continue (but count the extra update we just did)
                updateCount++;
                finalStableValue = testValue;
            }
        } 
        while (updateCount < maxUpdates);
        
        Console.WriteLine($"    Initial: {initialValue}, Final: {finalStableValue}, Updates: {updateCount} (max reached)");
        return updateCount;
    }
}