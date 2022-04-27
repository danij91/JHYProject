using System;
using UnityEngine;

public class SceneBase : MonoBehaviour
{
    [SerializeField]
    protected GameObject[] _hideObjects;

    protected virtual void Start()
    {
        if (SceneLoader.Instance.IsInitalize() == false)
        {
            SceneLoader.Instance.Initalize();
        }
    }

    public virtual void OnInitalize()
    {
    }

    public virtual void OnStart()
    {
        for (int i = 0; i < _hideObjects.Length; i++)
        {
            if (_hideObjects[i].activeInHierarchy == false)
            {
                _hideObjects[i].SetActive(true);
            }
        }
    }

    public virtual void UpdateScene()
    {
    }

    public virtual void OnClear()
    {
        for (int i = 0; i < _hideObjects.Length; i++)
        {
            if (_hideObjects[i].activeInHierarchy == true)
            {
                _hideObjects[i].SetActive(false);
            }
        }
        Reset();
    }

    private void Reset()
    {
        UIManager.Instance.CloseAll();
        PoolingManager.Instance.RestoreAllByType(EPoolingType.Character);
        PoolingManager.Instance.RestoreAllByType(EPoolingType.Map);
    }
}
