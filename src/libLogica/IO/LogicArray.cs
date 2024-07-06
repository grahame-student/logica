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
}
