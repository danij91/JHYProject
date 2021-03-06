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
    }

    public bool TryLoadTitleScene(bool isForced = false)
    {
        if (isForced || (!TitleScene.IS_TITLESCENE_LOADED && LocalDataConfig.Instance.IsStartTitleScene))
        {
            SceneLoader.Instance.ChangeScene(EScene.TITLE);
            return true;
        }

        return false;
    }
}
