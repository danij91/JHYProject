using System;
using System.Collections.Generic;

[Serializable]
public class DataSet
{
    private Dictionary<string, object> _data = new Dictionary<string, object>();

    public DataSet()
    {
    }

    public void Clear()
    {
        _data.Clear();
    }

    public T Get<T>(string key)
    {
        if(_data.ContainsKey(key))
        {
            return (T)_data[key];
        }

        return default(T);
    }

    public bool Has<T>(string key)
    {
        return _data.ContainsKey(key);
    }

    public void Remove<T>(string key)
    {
        if (_data.ContainsKey(key))
        {
            _data.Remove(key);
        }
    }

    public void Set<T>(string key, object inValue)
    {
        if(_data.ContainsKey(key))
        {
            _data[key] = inValue;
        }
        else
        {
            _data.Add(key, inValue);
        }
    }
}
