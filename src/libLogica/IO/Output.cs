namespace LibLogica.IO;

public class Output : IInputOutput
{
    private Boolean _value;
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
        // Set to current source value
        Value                =  source.Value;

        // Monitor for any future changes
        source.SignalChanged += (o, e) => Value = e.Value;
    }
}
