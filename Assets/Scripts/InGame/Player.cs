using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PoolingObject
{
    public Vector3 CurrentTargetDistance { get; set; }
    private readonly float moveSpeed = 5f;
    internal override void OnInitialize(params object[] parameters)
    {
    }

    protected override void OnUse()
    {
    }

    protected override void OnRestore()
    {
    }

    public void Jump(float touchTime)
    {
        CurrentTargetDistance = MapManager.Instance.GetLastDirection() * touchTime * moveSpeed;
        transform.position += CurrentTargetDistance;
        GameManager.Instance.OnPlayerJump();
    }
}
