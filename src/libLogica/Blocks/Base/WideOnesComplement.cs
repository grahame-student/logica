using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Base;

public class WideOnesComplement : LogicElement
{
    private readonly XorGate[] _gate;

    // Inputs
    public LogicArray<Input> A { get; }
    public Input Invert { get; } = new();

    // Outputs
    public LogicArray<Output> O { get; }

    public WideOnesComplement(Int32 width)
    {
        A = new LogicArray<Input>(width);
        O = new LogicArray<Output>(width);
        _gate = new XorGate[width];

        for (Int32 i = 0; i < width; i++)
        {
            _gate[i] = new XorGate();
            _gate[i].A.Connect(A[i]);
            _gate[i].B.Connect(Invert);
            O[i].Connect(_gate[i].O);
        }
    }

    public override void Update()
    {
        foreach (XorGate gate in _gate)
        {
            gate.Update();
        }
    }

    public override IEnumerable<String> GetIds() => _gate
        .Aggregate(GetLocalIds(), (current, t) => current
            .Concat(t.GetIds().Select(x => IdPrefix() + x)));

    protected override IEnumerable<String> GetLocalIds()
    {
        IEnumerable<String> result = new List<String>();
        for (Int32 i = A.Count - 1; i >= 0; i--)
        {
            result = result.Append($"{IdPrefix()}{nameof(A)}{i}");
        }
        result = result.Append($"{IdPrefix()}{nameof(Invert)}");
        for (Int32 i = O.Count - 1; i >= 0; i--)
        {
            result = result.Append($"{IdPrefix()}{nameof(O)}{i}");
        }

        return result;
    }

    public override IEnumerable<Boolean> GetValues() => _gate
        .Aggregate(GetLocalValues(), (current, t) => current.Concat(t.GetValues()));

    protected override IEnumerable<Boolean> GetLocalValues()
    {
        IEnumerable<Boolean> result = new List<Boolean>();
        for (Int32 i = A.Count - 1; i >= 0; i--)
        {
            result = result.Append(A[i].Value);
        }
        result = result.Append(Invert.Value);
        for (Int32 i = O.Count - 1; i >= 0; i--)
        {
            result = result.Append(O[i].Value);
        }

        return result;
    }
}
