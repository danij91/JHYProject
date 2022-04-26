using System;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    public const int POPUP_BASE_SORTING_ORDER = 100;
    public const int POPUP_SORTING_ORDER_OFFSET = 10;

    [Tooltip("Sorting Layer")]
    [SerializeField] protected string _sortingLayer = "Popup";

    protected Transform _bg;
    protected Action<UIBase> _callback;
    protected Canvas _canvas;
    protected CanvasGroup _canvasGroup;

    protected bool _isOpend = false;
    protected bool _isClosed = false;
    protected bool _isInit = false;


    public Camera ViewCamera { set => _canvas.worldCamera = value; }
    public UIManager Manager { get; set; }
    public bool IsFixedOrderInLayer { get; private set; }


    public bool IsReady
    {
        get { return _canvasGroup.interactable; }
        set { _canvasGroup.interactable = value; }
    }

    protected virtual void Awake()
    {
        _bg = transform.Find("Background");
        if (_bg != null)
        {
            _canvasGroup = _bg.GetComponent<CanvasGroup>();
        }

        _canvas = GetComponent<Canvas>();
        if (_canvas != null)
        {
            _canvas.sortingLayerName = _sortingLayer;
        }
    }

    public void Open(Action<UIBase> callback = null, params object[] args)
    {
        if (!_isOpend)
        {
            _isClosed = false;
            _isOpend = true;
            IsReady = false;

            _callback = callback;

            PrevOpen(args);
            IsReady = true;
        }
    }

    public void Close(bool isCallbackEnabled = true, Action<UIBase> forceCloseCallback = null)
    {
        if (!_isClosed)
        {
            IsReady = false;

            if (!isCallbackEnabled)
            {
                _callback = null;
            }
            else if (forceCloseCallback != null)
            {
                _callback = forceCloseCallback;
            }

            _isOpend = false;
            _isClosed = true;

            Remove();
            _callback?.Invoke(this);
        }
    }

    public void ReUse()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public void Remove()
    {
        PrevClose();
        gameObject.SetActive(false);
    }

    public void SetCanvasOrderInLayer(int inOrder, bool isFixed = false)
    {
        _canvas.sortingOrder = inOrder;
        IsFixedOrderInLayer = isFixed;
    }

    protected virtual void PrevOpen(params object[] args)
    {
    }

    protected virtual void PrevClose()
    {
    }

    public virtual void OnButtonEvent(Button inButton)
    {
    }
}
