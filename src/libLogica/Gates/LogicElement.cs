using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LibLogica.IO;

namespace LibLogica.Gates;

public abstract class LogicElement
{
    private static Int64 _gateCount;
    private readonly UInt64 _instanceCount;

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
        /// </summary>
        public DebugInfoBuilder AddChild(LogicElement child)
        {
            var childIds = child.GetIds().Select(x => _parent.IdPrefix() + x);
            var childValues = child.GetValues();

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
        /// Build and return the final IDs and values collections.
        /// </summary>
        public (IEnumerable<String> ids, IEnumerable<Boolean> values) Build()
        {
            return (_ids, _values);
        }

        /// <summary>
        /// Build and return only the IDs collection.
        /// </summary>
        public IEnumerable<String> BuildIds() => _ids;

        /// <summary>
        /// Build and return only the values collection.
        /// </summary>
        public IEnumerable<Boolean> BuildValues() => _values;
    }
}
