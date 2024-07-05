using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;

public class Oscillator : LogicElement
{
    // Outputs
    public Output O { get; } = new();

    public override void Update() => O.Value = !O.Value;

    public override IEnumerable<String> GetIds() => GetLocalIds();

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(O)}",
    ];

    public override IEnumerable<Boolean> GetValues() => GetLocalValues();

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        O.Value
    ];
}
