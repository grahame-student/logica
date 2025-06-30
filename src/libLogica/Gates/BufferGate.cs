using LibLogica.IO;

namespace LibLogica.Gates;

public class BufferGate : LogicElement
{
    public Input A { get; } = new();
    public Input Enable { get; } = new();

    public NullableOutput O { get; } = new();

    public BufferGate()
    {
        O.IsEnabled.Connect(Enable);
    }

    public override void Update()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<String> GetIds()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<String> GetLocalIds()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Boolean> GetValues()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<Boolean> GetLocalValues()
    {
        throw new NotImplementedException();
    }
}
