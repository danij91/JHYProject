using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMapType
{
    BrickMap,
    ConcreteMap,
    GrassMap,
    WoodMap,
}

public class CubeMap : PoolingObject
{
    [SerializeField] private EMapType mapType;

    internal override void OnInitialize(params object[] parameters)
    {
        SetRandomSize();
    }

    private void SetRandomSize()
    {
        float size = Random.Range(EConfig.Map.MIN_SIZE, EConfig.Map.MAX_SIZE);
        transform.localScale = new Vector3(size, transform.localScale.y, size);
    }

    protected override void OnUse()
    {
    }

    protected override void OnRestore()
    {
    }
}
