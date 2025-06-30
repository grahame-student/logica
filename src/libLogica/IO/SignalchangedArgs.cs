namespace LibLogica.IO;

public class SignalChangedArgs(Boolean value, Boolean isFloating = false) : EventArgs
{
    public Boolean Value { get; } = value;
}
