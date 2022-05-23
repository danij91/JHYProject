using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIManager : Singleton<UIManager>
{
    private List<UIBase> _uiList = new List<UIBase>();

    public Camera ViewCamera { get; set; }

    public void Initialize()
    {
        if (ViewCamera == null)
        {
            transform.position = Vector3.down * 10f;
            GameObject cam = Instantiate(ResourceManager.Instance.Load<GameObject>("Prefabs/UICamera"), transform);
            ViewCamera = cam.GetComponent<Camera>();
        }
    }

    public bool IsOpen<T>()
    {
        string uiName = typeof(T).Name;
        return _uiList.Exists(x => x.name.Equals(uiName) && x.isActiveAndEnabled);
    }

    protected void CreateUI(Type uiType, Action<UIBase> inCreationCallback)
    {
        UIBase ui = _uiList.Find(x => x.name.Equals(uiType.Name));

        if (ui != null)
        {
            if (ui.isActiveAndEnabled)
            {
                Debug.LogWarning($"{uiType.Name} UI가 이미 활성화되어 있습니다");
                return;
            }

            ui.ReUse();
            inCreationCallback?.Invoke(ui);
        }
        else
        {
            string path = $"UI/{uiType.Name}";
            Object obj = Resources.Load<GameObject>(path);
            if (obj == null)
            {
                Debug.LogError($"{uiType.Name} UI가 해당 경로에 존재하지 않습니다 : {path}");
                return;
            }

            inCreationCallback?.Invoke(InstantiateUI(uiType, obj));
        }

        UpdateUISortingOrder();
    }

    private void UpdateUISortingOrder()
    {
        int index = 0;
        foreach (UIBase uiBase in _uiList)
        {
            index++;
            if (uiBase.IsFixedOrderInLayer == false)
                uiBase.SetCanvasOrderInLayer(UIBase.POPUP_BASE_SORTING_ORDER + index * UIBase.POPUP_SORTING_ORDER_OFFSET);
        }
    }

    private UIBase InstantiateUI(Type uiType, Object inObj)
    {
        var gameObj = Instantiate(inObj) as GameObject;

        gameObj.SetActive(true);

        UIBase ui = gameObj.GetComponent(uiType.ToString()) as UIBase;
        ui.Manager = this;
        ui.gameObject.name = ui.gameObject.name.Replace("(Clone)", "");
        ui.transform.SetAsLastSibling();
        ui.transform.SetParent(transform, true);
        ui.ViewCamera = ViewCamera != null ? ViewCamera : Camera.main;

        _uiList.Add(ui);
        return ui;
    }

    public T GetUI<T>() where T : UIBase
    {
        T result = null;
        foreach (UIBase ui in _uiList)
        {
            result = ui as T;
            if (result != null)
                break;
        }
        return result;
    }

    public void Show<T>(Action<T> inCreationCallback = null, Action<T> inClosedCallback = null, params object[] args) where T : UIBase
    {
        Action<UIBase> creationCallback = null;
        Action<UIBase> closeCallback = null;

        if (inCreationCallback != null)
        {
            creationCallback = ui => inCreationCallback(ui as T);
        }

        if (inClosedCallback != null)
        {
            closeCallback = ui => inClosedCallback(ui as T);
        }

        Show(typeof(T), creationCallback, closeCallback, args);
    }

    public void Show(Type uiType, Action<UIBase> inCreationCallback = null, Action<UIBase> inClosedCallback = null, params object[] args)
    {
        CreateUI(uiType, (ui) => { inCreationCallback?.Invoke(ui); ui.Open(inClosedCallback, args); });
    }

    public void Close<T>(Action<T> inClosedCallback = null) where T : UIBase
    {
        UIBase ui = _uiList.Find(x => x.name.Equals(typeof(T).Name) && x.isActiveAndEnabled);

        if (ui == null)
        {
            Debug.LogError($"{typeof(T).Name} UI가 활성화되어 있지 않습니다");
        }
        else
        {
            Action<UIBase> closeCallback = null;
            if (inClosedCallback != null)
            {
                closeCallback = ui => inClosedCallback(ui as T);
            }

            ui.Close(true, closeCallback);
        }
    }

    public void Clear()
    {
        _uiList.Clear();
    }

    public void CloseAll()
    {
        foreach (UIBase ui in _uiList.Where(x => x != null))
        {
            ui.Close();
        }
    }

    public void Destroy(UIBase ui)
    {
        _uiList.Remove(ui);
        Destroy(ui.gameObject);
    }
}
