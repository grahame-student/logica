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
        O.Value = A.Value;
    }

    public override IEnumerable<String> GetIds() => GetLocalIds();

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(A)}",
        $"{IdPrefix()}{nameof(Enable)}",
        $"{IdPrefix()}{nameof(O)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues();

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        A.Value,
        Enable.Value,
        O.Value
    ];
}
