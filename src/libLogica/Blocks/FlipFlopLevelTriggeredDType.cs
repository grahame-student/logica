using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLogica.Blocks;

using Gates;

using IO;

public class FlipFlopLevelTriggeredDType : LogicElement
{
    private readonly NotGate _not = new();
    private readonly AndGate _and1 = new();
    private readonly AndGate _and2 = new();
    private readonly FlipFlopRS _rs = new();

    // Inputs
    public Input D { get; } = new();
    public Input Clock { get; } = new();

    // Outputs
    public Output Q { get; } = new();
    public Output NQ { get; } = new();

    public FlipFlopLevelTriggeredDType()
    {
        _not.A.Connect(D);
        _and1.A.Connect(_not.O);
        _and1.B.Connect(Clock);
        _and2.A.Connect(Clock);
        _and2.B.Connect(D);

        _rs.S.Connect(_and2.O);
        _rs.R.Connect(_and1.O);

        Q.Connect(_rs.Q);
        NQ.Connect(_rs.NQ);
        Update();
    }

    public override void Update()
    {
        _not.Update();
        _and1.Update();
        _and2.Update();
        _rs.Update();
        /*
        Console.WriteLine($" D - {D.Value}");
        Console.WriteLine($" C - {Clock.Value}");
        Console.WriteLine($"NA - {_not.A.Value}");
        Console.WriteLine($"NO - {_not.O.Value}");
        Console.WriteLine($"1A - {_and1.A.Value}");
        Console.WriteLine($"1B - {_and1.B.Value}");
        Console.WriteLine($"1O - {_and1.O.Value}");
        Console.WriteLine($"2A - {_and2.A.Value}");
        Console.WriteLine($"2B - {_and2.B.Value}");
        Console.WriteLine($"2O - {_and2.O.Value}");
        Console.WriteLine($"FR - {_rs.R.Value}");
        Console.WriteLine($"FS - {_rs.S.Value}");
        Console.WriteLine($"FQ - {_rs.Q.Value}");
        Console.WriteLine($"FN - {_rs.NQ.Value}");
        Console.WriteLine($" Q - {Q.Value}");
        Console.WriteLine($" N - {NQ.Value}\n");
    */
    }

    public override IEnumerable<String> GetIds() => GetLocalIds()
        .Concat(_not.GetIds().Select(x => IdPrefix() + x))
        .Concat(_and1.GetIds().Select(x => IdPrefix() + x))
        .Concat(_and2.GetIds().Select(x => IdPrefix() + x))
        .Concat(_rs.GetIds().Select(x => IdPrefix() + x));

    protected override IEnumerable<String> GetLocalIds() =>
    [
        $"{IdPrefix()}{nameof(D)}",
        $"{IdPrefix()}{nameof(Clock)}",
        $"{IdPrefix()}{nameof(Q)}",
        $"{IdPrefix()}{nameof(NQ)}",
    ];


    public override IEnumerable<Boolean> GetValues() => GetLocalValues()
        .Concat(_not.GetValues())
        .Concat(_and1.GetValues())
        .Concat(_and2.GetValues())
        .Concat(_rs.GetValues());

    protected override IEnumerable<Boolean> GetLocalValues() =>
    [
        D.Value,
        Clock.Value,
        Q.Value,
        NQ.Value,
    ];
}
