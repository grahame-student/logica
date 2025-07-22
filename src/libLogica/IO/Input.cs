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
            throw new InvalidOperationException("Multiple outputs are driving the same input - bus conflict");
        }

        Value = activeSources[0].Value;
    }
}
