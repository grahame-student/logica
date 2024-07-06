using LibLogica.Blocks.Width8Bit;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks;

public class AccumulatingAdder : LogicElement
{
    private readonly Adder8Bit _adder8 = new();
    private readonly Latch8Bit _latch8 = new();

    // Inputs
    public LogicArray<Input> A { get; } = new(8);
    public Input Add { get; } = new();

    // Outputs
    public LogicArray<Input> O { get; } = new(8);

    public AccumulatingAdder()
    {
        _adder8.B.Connect(A);
        _latch8.D.Connect(_adder8.SumOut);
        _latch8.Clock.Connect(Add);
        A.Connect(_latch8.Q);
        O.Connect(_latch8.Q);
    }

    public override void Update()
    {
        _adder8.Update();
        _latch8.Update();
    }

    public override IEnumerable<String> GetIds() => GetLocalIds()
        .Concat(_adder8.GetIds().Select(x => IdPrefix() + x))
        .Concat(_latch8.GetIds().Select(x => IdPrefix() + x));

    protected override IEnumerable<String> GetLocalIds()
    {
        IList<String> result = new List<String>();
        for (Int32 i = A.Count - 1; i >= 0; i--)
        {
            result.Add($"{IdPrefix()}{nameof(A)}{i}");
        }
        result.Add($"{IdPrefix()}{nameof(Add)}");
        for (Int32 i = O.Count - 1; i >= 0; i--)
        {
            result.Add($"{IdPrefix()}{nameof(O)}{i}");
        }

        return result;
    }

    public override IEnumerable<Boolean> GetValues() => GetLocalValues()
        .Concat(_adder8.GetValues())
        .Concat(_latch8.GetValues());

    protected override IEnumerable<Boolean> GetLocalValues() => A.GetValues()
        .Append(Add.Value)
        .Concat(O.GetValues());
}
