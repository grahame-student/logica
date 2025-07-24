using System;
using System.Collections.Generic;
using System.Linq;
using LibLogica.Gates;
using LibLogica.IO;

namespace LibLogica.Blocks.Base;

public class OnesComplementWide : LogicElement
{
    private readonly XorGate[] _gate;

    // Inputs
    public LogicArray<Input> A { get; }
    public Input Invert { get; } = new();

    // Outputs
    public LogicArray<Output> O { get; }

    public OnesComplementWide() : this(8) { }

    public OnesComplementWide(Int32 width)
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
        ClearValuesCache(); // Always clear values cache for educational observability
        
        foreach (XorGate gate in _gate)
        {
            gate.Update();
        }
    }

    protected (IEnumerable<String> ids, IEnumerable<Boolean> values) BuildDebugInfo() =>
        DebugInfo()
            .AddArray(nameof(A), A)
            .AddLocal(nameof(Invert), Invert)
            .AddArray(nameof(O), O)
            .AddChildren(_gate)
            .Build();

    public override IEnumerable<String> GetIds() => BuildDebugInfo().ids;

    public override IEnumerable<Boolean> GetValues() => BuildDebugInfo().values;
}
