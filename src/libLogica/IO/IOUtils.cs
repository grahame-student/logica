using System;

namespace LibLogica.IO;

/// <summary>
/// Utility methods for working with IInputOutput sources.
/// </summary>
public static class IOUtils
{
    /// <summary>
    /// Determines if a source is in high impedance state.
    /// </summary>
    /// <param name="source">The source to check</param>
    /// <returns>True if the source is in high impedance state, false otherwise</returns>
    public static Boolean IsSourceHighImpedance(IInputOutput source)
    {
        // Check if the source is an Output with high impedance
        if (source is Output output)
        {
            return output.IsHighImpedance;
        }

        // For other types (like InputOutput), assume they're always active
        return false;
    }
}
