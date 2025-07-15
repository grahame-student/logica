using System;

namespace LibLogica.IO;

public class Output : IInputOutput
{
    private Boolean _value;
    private Boolean _isHighImpedance;

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
        // Set to current source value
        Value = source.Value;

        // Monitor for any future changes
        source.SignalChanged += (o, e) => Value = e.Value;
    }
}
