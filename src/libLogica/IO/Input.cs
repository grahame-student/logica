namespace LibLogica.IO;

public class Input : IInputOutput
{
    private Boolean _value;
    private readonly List<IInputOutput> _sources = new();

    public event EventHandler<SignalChangedArgs> SignalChanged = delegate { };

    public Boolean Value
    {
        get => _value;
        set
        {
            if (Value == value) return;
            _value = value;
            SignalChanged(this, new SignalChangedArgs(value));
        }
    }

    public void Connect(IInputOutput source)
    {
        // Add source to our list
        _sources.Add(source);

        // Monitor for any future changes from this source
        source.SignalChanged += (o, e) => UpdateValueFromSources();

        // Update current value based on all sources
        UpdateValueFromSources();
    }

    private void UpdateValueFromSources()
    {
        // Get all sources that are not in high impedance
        var activeSources = _sources.Where(s => !IsSourceHighImpedance(s)).ToList();

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
