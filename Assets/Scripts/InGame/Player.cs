using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : PoolingObject {
    public enum PLAYER_STATE {
        IDLE,
        JUMP,
        CROUCH,
        FALL
    }

    public PLAYER_STATE currentState { get; private set; }
    public Vector3 CurrentTargetDistance { get; set; }
    private Rigidbody rigidbody;
    [SerializeField]
    private Animator animator;

    public bool IsJumping =>
        currentState == PLAYER_STATE.JUMP;


    internal override void OnInitialize(params object[] parameters) {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();

        rigidbody.isKinematic = false;
        transform.position = MapManager.Instance.StartPos;
        SetRotation();
    }

    protected override void OnUse() { }

    protected override void OnRestore() {
        rigidbody.isKinematic = true;
    }

    public void Jump(float touchTime) {
        ChangeState(PLAYER_STATE.JUMP);
        CurrentTargetDistance = MapManager.Instance.GetLastDirection() * touchTime * EConfig.System.MOVE_SPEED;
        if (TryGetCorrectionPos(out Vector3 correctionPos)) {
            CurrentTargetDistance = correctionPos;
        }

        Vector3 targetPos = transform.position + CurrentTargetDistance;
        transform.DOMove(targetPos, 1f).SetEase(Ease.Linear);
        transform.DOJump(targetPos, 1f, 1, 1f).SetEase(Ease.InOutSine)
            .OnComplete(() => {
                SetRotation();
            });
    }

    private bool TryGetCorrectionPos(out Vector3 correctionPos) {
        Vector3 targetDistance = MapManager.Instance.CurrentMap.transform.position - transform.position;
        float differ = Vector3.Distance(CurrentTargetDistance.SetY(0f), targetDistance.SetY(0f));

        if (differ <= EConfig.System.CORRECTION_VALUE) {
            correctionPos = targetDistance.SetY(0f);
            return true;
        }
        else {
            correctionPos = transform.position;
            return false;
        }
    }

    private void SetRotation() {
        Vector3 direction = MapManager.Instance.GetLastDirection();
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void ChangeState(PLAYER_STATE state) {
        Debug.Log(state);
        currentState = state;
        animator.Play(state.ToString());
    }
}
