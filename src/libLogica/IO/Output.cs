using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLogica.IO;

public class Output : IInputOutput
{
    private Boolean _value;
    private Boolean _isHighImpedance;
    private readonly List<IInputOutput> _sources = new();

    public event EventHandler<SignalChangedArgs> SignalChanged = delegate { };

    public Boolean Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;

            // Only signal if not in high impedance
            if (!_isHighImpedance)
            {
                SignalChanged(this, new SignalChangedArgs(value));
            }
        }
    }

    public Boolean IsHighImpedance
    {
        get => _isHighImpedance;
        set
        {
            if (_isHighImpedance == value) return;
            _isHighImpedance = value;

            // Signal the change in impedance state
            // We need to signal any change so inputs can re-evaluate
            SignalChanged(this, new SignalChangedArgs(_value));
        }
    }

    public void Connect(IInputOutput source)
    {
        // Add source to our list
        _sources.Add(source);

        // Monitor for any future changes from this source
        source.SignalChanged += (o, e) => UpdateStateFromSources();

        // Update current state based on all sources
        UpdateStateFromSources();
    }

    private void UpdateStateFromSources()
    {
        // Get all sources that are not in high impedance
        var activeSources = _sources.Where(s => !IsSourceHighImpedance(s)).ToList();

        // Update high impedance state based on active sources
        IsHighImpedance = activeSources.Count == 0;

        if (activeSources.Count == 0)
        {
            // All sources are high impedance - keep current value
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
        }

        // Defensive check: Ensure we have at least one active source before accessing
        // This should always be true due to the early return above, but adding safety
        if (activeSources.Count > 0)
        {
            // Update value from the active source(s)
            Value = activeSources[0].Value;
        }
    }

    private static Boolean IsSourceHighImpedance(IInputOutput source)
    {
        // Check if the source is an Output with high impedance
        if (source is Output output)
        {
            return output.IsHighImpedance;
        }

        // For other types (like InputOutput), assume they're always active
        return false;
    }
}
