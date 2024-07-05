namespace LibLogica.IO;

public interface IInputOutput
{
    event EventHandler<SignalChangedArgs> SignalChanged;

    public Boolean Value { get; set; }

    public void Connect(IInputOutput source);
}
