namespace LibLogica.IO;

public class NullableOutput : IInputOutput
{
    public event EventHandler<SignalChangedArgs>? SignalChanged;
    public Boolean Value { get; set; }

    public Input IsEnabled { get; } = new();

    public void Connect(IInputOutput source)
    {
        // Set to current source value
        Value = source.Value;

        // Monitor for any future changes
        source.SignalChanged += (o, e) => Value = e.Value;
    }
}
