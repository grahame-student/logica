using LibLogica.Gates;

using System;

namespace LibLogica.Blocks.Base;

public class BlockArray<T> where T : LogicElement, new()
{
    private readonly T[] _blocks;

    public T this[Int32 index] => _blocks[index];

    public BlockArray(Int32 width)
    {
        _blocks = new T[width];
        for (Int32 i = 0; i < width; i++)
        {
            _blocks[i] = new T();
        }
    }

    public Int32 Count => _blocks.Length;
}
