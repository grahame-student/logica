using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLogica.IO;

public class LogicArray<T> where T : IInputOutput, new()
{
    private readonly T[] _bits;

    public LogicArray(Int32 width)
    {
        _bits = new T[width];
        for (Int32 i = 0; i < _bits.Length; i++)
        {
            _bits[i] = new T();
        }
    }

    public IInputOutput this[Int32 index] => _bits[index];

    public Int32 Count => _bits.Length;

    public void Connect<TInputOutput>(LogicArray<TInputOutput> sources) where TInputOutput : IInputOutput, new()
    {
        // Generic needs to be different to T to allow for IO of different type to be passed in as an argument
        // for example if the function required LogicArray<T> then
        // <input>.connect<input>  would be valid, but
        // <input>.connect<output> would not be valid
        for (Int32 i = 0; i < Count; i++)
        {
            Int32 i1 = i;
            sources[i].SignalChanged += (o, e) => _bits[i1].Value = e.Value;
        }
    }

    public IEnumerable<Boolean> GetValues()
    {
        List<Boolean> result = [];
        result.AddRange(_bits.Select(bit => bit.Value));
        return result;
    }
}
