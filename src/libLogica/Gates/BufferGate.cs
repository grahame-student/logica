using LibLogica.IO;

namespace LibLogica.Gates;

public class BufferGate : LogicElement
{
    public Input A { get; } = new();
    public Input Enable { get; } = new();

    public Output O { get; } = new();

    public BufferGate()
    {
        // Buffer gate starts in high impedance state
        O.IsHighImpedance = true;
    }

    public override void Update()
    {
        if (Enable.Value)
        {
            O.IsHighImpedance = false;
            O.Value = A.Value;
        }
        else
        {
            O.IsHighImpedance = true;
        }
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
