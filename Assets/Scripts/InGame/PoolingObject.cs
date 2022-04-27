using UnityEngine;

public enum EPoolingState
{
    Waiting,
    Using,
}

public abstract class PoolingObject : MonoBehaviour
{
    private EPoolingState _poolingState = EPoolingState.Waiting;
    private Transform _cachedTransform;

    public new Transform transform
    {
        get
        {
            if (null == _cachedTransform)
            {
                _cachedTransform = GetComponent<Transform>();
            }

            return _cachedTransform;
        }
    }

    public EPoolingState PoolingState
    {
        get { return _poolingState; }
        private set
        {
            _poolingState = value;
            switch (_poolingState)
            {
                case EPoolingState.Using:
                    gameObject.SetActive(true);
                    OnUse();
                    break;
                case EPoolingState.Waiting:
                    OnRestore();
                    gameObject.SetActive(false);
                    break;
            }
        }
    }

    public void Use()
    {
        PoolingState = EPoolingState.Using;
    }

    public void Restore()
    {
        if (PoolingState == EPoolingState.Waiting)
        {
            return;
        }
        PoolingState = EPoolingState.Waiting;
    }

    internal abstract void OnInitialize(params object[] parameters);
    protected abstract void OnUse();
    protected abstract void OnRestore();
}