using LibLogica.IO;

namespace LibLogica.Gates;

/// <summary>
/// Interface for binary logic gates with two inputs (A, B) and one output (O).
/// </summary>
public interface IBinaryGate
{
    /// <summary>
    /// First input of the binary gate.
    /// </summary>
    Input A { get; }

    /// <summary>
    /// Second input of the binary gate.
    /// </summary>
    Input B { get; }

    /// <summary>
    /// Output of the binary gate.
    /// </summary>
    Output O { get; }
}
