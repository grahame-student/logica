using System;
using System.Collections.Generic;

namespace LibLogica.IO;

public class Input : SourceManagerBase
{
    private Boolean _value;

    public override event EventHandler<SignalChangedArgs> SignalChanged = delegate { };

    public override Boolean Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            SignalChanged(this, new SignalChangedArgs(value));
        }
    }

    protected override void HandleSourcesUpdate(List<IInputOutput> activeSources)
    {
        if (activeSources.Count == 0)
        {
            // For now, keep the current value when all sources are high impedance
            // In real hardware, this would be an undefined state
            return;
        }

        if (activeSources.Count > 1)
        {
            // Multiple active sources - this represents a bus conflict scenario
            // In real hardware, this would typically result in undefined behavior,
            // potentially damaging the circuit or producing unpredictable voltage levels.
            //
            // Current implementation: Use the first active source's value to maintain
            // compatibility with existing behavior. This is a simplification that allows
            // the simulation to continue running.
            //
            // Future improvements could include:
            // - Logging warnings about bus conflicts
            // - Implementing proper bus conflict resolution (e.g., wired-OR, wired-AND)
            // - Allowing configurable conflict resolution strategies
            // - Modeling actual hardware behavior (indeterminate states, damage simulation)
            // throw new InvalidOperationException("Multiple outputs are driving the same input - bus conflict");
        }

        Value = activeSources[0].Value;
    }
}
