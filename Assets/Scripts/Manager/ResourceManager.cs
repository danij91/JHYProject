using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, Object> _resourcesPool = new Dictionary<string, Object>();

    public T Load<T>(string path) where T : Object
    {
        T res;

        if (_resourcesPool.ContainsKey(path) == false)
        {
            res = Resources.Load<T>(path);
            if (res == null)
            {
                Debug.LogError("ResourceManager Load Fail : " + path);
            }

            _resourcesPool.Add(path, res);
        }

        res = _resourcesPool[path] as T;

        return res;
    }
}
