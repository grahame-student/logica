using LibLogica.IO;

namespace LibLogica.Blocks;

public interface IDTypeFlipFlop
{
    Input D { get; }
    Input Clock { get; }
    Output Q { get; }
    Output NQ { get; }
}
