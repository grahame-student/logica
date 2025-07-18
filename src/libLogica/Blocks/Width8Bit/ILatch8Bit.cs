using LibLogica.IO;

namespace LibLogica.Blocks.Width8Bit;

public interface ILatch8Bit
{
    LogicArray<Input> D { get; }
    Input Clock { get; }
    LogicArray<Output> Q { get; }
}
