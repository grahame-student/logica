using LibLogica.IO;

namespace LibLogica.Blocks.Base;

/// <summary>
/// Interface for wide latch components with clock input.
/// </summary>
public interface IWideLatch
{
    /// <summary>
    /// Data inputs for the latch.
    /// </summary>
    LogicArray<Input> D { get; }
    
    /// <summary>
    /// Clock input for the latch.
    /// </summary>
    Input Clock { get; }
    
    /// <summary>
    /// Data outputs from the latch.
    /// </summary>
    LogicArray<Output> Q { get; }
}
