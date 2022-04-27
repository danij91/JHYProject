using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EPoolingType
{
    None,
    Character,
    Effect,
    Sound,
    CubeMap,
}

public class PoolingManager : Singleton<PoolingManager>
{
    private List<GameObject> _resourceParents = new List<GameObject>();
    private Dictionary<EPoolingType, List<PoolingObject>> _poolingList = new Dictionary<EPoolingType, List<PoolingObject>>();
    private const string DEFAULT_PATH = "Prefabs";


    public Transform GetParent(EPoolingType type)
    {
        GameObject go = null;
        foreach (var item in _resourceParents)
        {
            if (item.name.Equals(type.ToString()))
            {
                go = item;
            }
        }

        if (go == null)
        {
            go = new GameObject();
            go.name = type.ToString();
            go.transform.SetParent(transform);
            _resourceParents.Add(go);
        }

        return go.transform;
    }

    private T Load<T>(EPoolingType type, string resourceName) where T : PoolingObject
    {
        string path = $"{DEFAULT_PATH}/{type}/{resourceName}";

        return ResourceManager.Instance.Load<T>(path);
    }

    private T CreatePoolingObject<T>(EPoolingType type, string resourceName)
        where T : PoolingObject
    {
        var obj = Load<T>(type, resourceName);
        if (obj == null)
        {
            Debug.LogError($"{resourceName}이/가 해당 경로에 존재하지 않습니다 : {DEFAULT_PATH}/{type}/{resourceName}");
            return null;
        }

        var newObj = Instantiate(obj, GetParent(type));
        newObj.name = newObj.name.Replace("(Clone)", "");

        if (!_poolingList.ContainsKey(type))
        {
            _poolingList[type] = new List<PoolingObject>();
        }
        _poolingList[type].Add(newObj);

        return newObj;
    }


    public T Create<T>(EPoolingType type, string resourceName, Transform parent = null, params object[] parameters) where T : PoolingObject
    {
        bool isContainsResources = _poolingList.ContainsKey(type);
        if (isContainsResources)
        {
            var pObj = _poolingList[type].Find(x =>
                x.gameObject.name.StartsWith(resourceName)
                && x.PoolingState == EPoolingState.Waiting
                && !x.isActiveAndEnabled) as T;

            if (pObj != null)
            {
                pObj.OnInitialize(parameters);
                pObj.Use();
                return pObj;
            }
        }

        var newObj = CreatePoolingObject<T>(type, resourceName);
        if (parent != null) newObj.transform.SetParent(parent);

        newObj.OnInitialize(parameters);
        newObj.Use();

        return newObj;
    }

    public void RestoreAllByType(EPoolingType type)
    {
        if (_poolingList.ContainsKey(type))
        {
            foreach (var item in _poolingList[type])
            {
                if (item.PoolingState == EPoolingState.Waiting && !item.isActiveAndEnabled)
                    continue;
                if (item != null)
                {
                    item.Restore();
                }
            }
        }
    }
}
