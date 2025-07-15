namespace LibLogica.IO;

public class SignalChangedArgs(Boolean value) : EventArgs
{
    public Boolean Value { get; } = value;
}
