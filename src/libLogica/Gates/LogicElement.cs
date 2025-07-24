using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LibLogica.Blocks.Base;
using LibLogica.IO;

namespace LibLogica.Gates;

public abstract class LogicElement
{
    private static Int64 _gateCount;
    private readonly UInt64 _instanceCount;

    // Caching for debug info - Option 1 optimization
    private IEnumerable<String>? _cachedIds;
    private IEnumerable<Boolean>? _cachedValues;
    private Boolean _debugInfoCached = false;
    
    // Change detection to avoid unnecessary cache clearing
    private Boolean _stateChanged = true; // Initially true to ensure first cache is cleared

    protected LogicElement()
    {
        _instanceCount = GetNextGateCount();
    }

    private static UInt64 GetNextGateCount()
    {
        return (UInt64)Interlocked.Increment(ref _gateCount);
    }

    public abstract void Update();
    public abstract IEnumerable<String> GetIds();
    public abstract IEnumerable<Boolean> GetValues();

    protected String IdPrefix()
    {
        return $"{GetType().Name}_{_instanceCount}.";
    }

    /// <summary>
    /// Cached version of GetIds() - Option 1 optimization.
    /// Use this in DebugInfoBuilder.AddChild() to avoid redundant calculations.
    /// </summary>
    public IEnumerable<String> GetIdsCached()
    {
        if (!_debugInfoCached)
        {
            var (ids, values) = GetDebugInfoInternal();
            _cachedIds = ids.ToList(); // Materialize to avoid re-enumeration
            _cachedValues = values.ToList();
            _debugInfoCached = true;
        }
        return _cachedIds!;
    }

    /// <summary>
    /// Cached version of GetValues() - Option 1 optimization.
    /// Use this in DebugInfoBuilder.AddChild() to avoid redundant calculations.
    /// </summary>
    public IEnumerable<Boolean> GetValuesCached()
    {
        if (!_debugInfoCached)
        {
            var (ids, values) = GetDebugInfoInternal();
            _cachedIds = ids.ToList(); // Materialize to avoid re-enumeration
            _cachedValues = values.ToList();
            _debugInfoCached = true;
        }
        return _cachedValues!;
    }

    /// <summary>
    /// Internal method to get debug info tuple. Override this instead of GetIds/GetValues when using caching.
    /// </summary>
    protected virtual (IEnumerable<String> ids, IEnumerable<Boolean> values) GetDebugInfoInternal()
    {
        // Default implementation calls the abstract methods for backward compatibility
        return (GetIds(), GetValues());
    }

    /// <summary>
    /// Mark that the element's state has changed and cache should be cleared on next access.
    /// Call this from Update() methods when the element's state actually changes.
    /// </summary>
    protected void MarkStateChanged()
    {
        _stateChanged = true;
    }

    /// <summary>
    /// Clear the debug info cache if state has changed. Call this at the beginning of Update() methods.
    /// This provides better performance by avoiding false cache misses when nothing actually changed.
    /// </summary>
    protected void ClearDebugInfoCacheIfChanged()
    {
        if (_stateChanged)
        {
            ClearDebugInfoCache();
            _stateChanged = false;
        }
    }

    /// <summary>
    /// Clear the debug info cache. Call this if the element's state changes and debug info needs to be recalculated.
    /// </summary>
    protected void ClearDebugInfoCache()
    {
        _debugInfoCached = false;
        _cachedIds = null;
        _cachedValues = null;
    }

    /// <summary>
    /// Helper to create both GetIds() and GetValues() implementations with the same builder.
    /// This ensures perfect alignment and eliminates the need to duplicate the builder calls.
    /// Usage: protected (IEnumerable<string> ids, IEnumerable<bool> values) BuildDebugInfo() =>
    ///          DebugInfo().AddLocals(...).Build();
    /// Then: public override IEnumerable<string> GetIds() => BuildDebugInfo().ids;
    ///       public override IEnumerable<bool> GetValues() => BuildDebugInfo().values;
    /// </summary>
    protected DebugInfoBuilder DebugInfo() => new DebugInfoBuilder(this);

    /// <summary>
    /// Builder class to help construct aligned GetIds() and GetValues() results with reduced boilerplate.
    /// Usage: return DebugInfo().AddLocal(...).AddChild(...).Build();
    /// </summary>
    protected class DebugInfoBuilder
    {
        private readonly LogicElement _parent;
        private readonly List<String> _ids = new();
        private readonly List<Boolean> _values = new();

        internal DebugInfoBuilder(LogicElement parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Add a local input or output to the debug information.
        /// </summary>
        public DebugInfoBuilder AddLocal(String name, IInputOutput io)
        {
            _ids.Add($"{_parent.IdPrefix()}{name}");
            _values.Add(io.Value);
            return this;
        }

        /// <summary>
        /// Add a LogicArray to the debug information with indexed names.
        /// Items are added in reverse order (high to low index) to match existing patterns.
        /// </summary>
        public DebugInfoBuilder AddArray<T>(String baseName, LogicArray<T> array) where T : IInputOutput, new()
        {
            for (Int32 i = array.Count - 1; i >= 0; i--)
            {
                _ids.Add($"{_parent.IdPrefix()}{baseName}{i}");
                _values.Add(array[i].Value);
            }
            return this;
        }

        /// <summary>
        /// Add multiple local inputs/outputs at once using a params array of (name, io) tuples.
        /// </summary>
        public DebugInfoBuilder AddLocals(params (String name, IInputOutput io)[] locals)
        {
            foreach (var (name, io) in locals)
            {
                AddLocal(name, io);
            }
            return this;
        }

        /// <summary>
        /// Add a child LogicElement's debug information with proper prefixing.
        /// Uses cached debug info to avoid redundant calculations - Option 1 optimization.
        /// </summary>
        public DebugInfoBuilder AddChild(LogicElement child)
        {
            var childIds = child.GetIdsCached().Select(x => _parent.IdPrefix() + x);
            var childValues = child.GetValuesCached();

            _ids.AddRange(childIds);
            _values.AddRange(childValues);
            return this;
        }

        /// <summary>
        /// Add multiple children in the specified order.
        /// </summary>
        public DebugInfoBuilder AddChildren(params LogicElement[] children)
        {
            foreach (var child in children)
            {
                AddChild(child);
            }
            return this;
        }

        /// <summary>
        /// Add children from a BlockArray in MSB->LSB order for multibit consistency.
        /// </summary>
        public DebugInfoBuilder AddChildren<T>(BlockArray<T> blockArray) where T : LogicElement, new()
        {
            for (Int32 i = blockArray.Count - 1; i >= 0; i--)
            {
                AddChild(blockArray[i]);
            }
            return this;
        }

        /// <summary>
        /// Build and return the final IDs and values collections.
        /// </summary>
        public (IEnumerable<String> ids, IEnumerable<Boolean> values) Build()
        {
            return (_ids, _values);
        }
    }
}
