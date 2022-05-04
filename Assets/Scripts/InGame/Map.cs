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

public class Map : PoolingObject
{
    [SerializeField] private EMapType mapType;

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.Player.ChangeState(Player.PLAYER_STATE.IDLE);
        if(MapManager.Instance.CurrentMap == this)
            GameManager.Instance.OnSuccess();
    }

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
