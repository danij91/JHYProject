using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : PoolingObject
{
    public Vector3 CurrentTargetDistance { get; set; }
    internal override void OnInitialize(params object[] parameters)
    {
        transform.position = MapManager.Instance.StartPos;
    }

    protected override void OnUse()
    {
    }

    protected override void OnRestore()
    {
    }

    public void Jump(float touchTime)
    {
        CurrentTargetDistance = MapManager.Instance.GetLastDirection() * touchTime * EConfig.System.MOVE_SPEED;
        if(TryGetCorrectionPos(out Vector3 correctionPos))
        {
            CurrentTargetDistance = correctionPos;
            Debug.LogWarning("보정들어감");
        }
        
        Vector3 targetPos = transform.position + CurrentTargetDistance;
        transform.DOMove(targetPos, 1f).SetEase(Ease.Linear);
        transform.DOJump(targetPos, 1f, 1 , 1f).SetEase(Ease.InOutSine);

        GameManager.Instance.OnPlayerJump();
    }

    private bool TryGetCorrectionPos(out Vector3 correctionPos)
    {
        Vector3 targetDistance = MapManager.Instance.CurrentMapPos - transform.position;
        float differ = Vector3.Distance(CurrentTargetDistance.SetY(0f), targetDistance.SetY(0f));

        if(differ <= EConfig.System.CORRECTION_VALUE)
        {
            correctionPos = targetDistance.SetY(0f);
            return true;
        }
        else
        {
            correctionPos = transform.position;
            return false;
        }
    }
}
