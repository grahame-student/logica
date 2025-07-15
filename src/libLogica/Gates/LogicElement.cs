using System;
using System.Collections.Generic;

namespace LibLogica.Gates;

public abstract class LogicElement
{
    private static UInt64 _gateCount;
    private readonly UInt64 _instanceCount;

    protected LogicElement()
    {
        _instanceCount = _gateCount;
        _gateCount++;
    }

    public abstract void Update();
    public abstract IEnumerable<String> GetIds();
    protected abstract IEnumerable<String> GetLocalIds();
    public abstract IEnumerable<Boolean> GetValues();
    protected abstract IEnumerable<Boolean> GetLocalValues();

    protected String IdPrefix()
    {
        return $"{GetType().Name}_{_instanceCount}.";
    }
}
