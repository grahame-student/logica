using System;
using System.Collections.Generic;
using System.Linq;

namespace LibLogica.IO;

/// <summary>
/// Base class for managing multiple sources connected to an input/output.
/// Handles source tracking, high impedance detection, and conflict resolution.
/// </summary>
public abstract class SourceManagerBase : IInputOutput
{
    private readonly List<IInputOutput> _sources = new();

    public abstract event EventHandler<SignalChangedArgs> SignalChanged;
    public abstract Boolean Value { get; set; }

    public void Connect(IInputOutput source)
    {
        // Add source to our list
        _sources.Add(source);

        // Monitor for any future changes from this source
        source.SignalChanged += (o, e) => UpdateFromSources();

        // Update current state based on all sources
        UpdateFromSources();
    }

    /// <summary>
    /// Template method for updating state when sources change.
    /// Derived classes implement specific logic for handling active sources.
    /// </summary>
    private void UpdateFromSources()
    {
        // Get all sources that are not in high impedance
        var activeSources = _sources.Where(s => !IOUtils.IsSourceHighImpedance(s)).ToList();

        // Let derived class handle the specific logic
        HandleSourcesUpdate(activeSources);
    }

    /// <summary>
    /// Derived classes implement this to handle active sources according to their specific logic.
    /// </summary>
    /// <param name="activeSources">List of sources that are not in high impedance state</param>
    protected abstract void HandleSourcesUpdate(List<IInputOutput> activeSources);
}
