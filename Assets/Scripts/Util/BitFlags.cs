using System;
using System.Collections.Generic;
using System.Linq;

public struct BitFlags
{
    private byte _data;

    public BitFlags(int inInitailFlags)
    {
        _data = (byte)inInitailFlags;
    }

    public void Clear()
    {
        Reset();
    }

    public void Reset(int inInitailFlags = 0)
    {
        _data = (byte)inInitailFlags;
    }

    public void Add(byte inFlags)
    {
        _data |= inFlags;
    }

    public void Remove(byte inFlags)
    {
        _data = (byte)(_data & ~inFlags);
    }

    public bool Has(byte inFlags)
    {
        return (_data & inFlags) == inFlags;
    }

    public byte Value { get { return _data; } }

    public bool IsEmpty { get { return _data == 0; } }
}
