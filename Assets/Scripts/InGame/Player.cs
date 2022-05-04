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

    [SerializeField]
    private Animator animator;

    private Rigidbody rigidbody;

    public PLAYER_STATE CurrentState { get; private set; }
    public Vector3 CurrentTargetPos { get; private set; }
    public bool IsJumping => CurrentState == PLAYER_STATE.JUMP;

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
        CurrentTargetPos = MapManager.Instance.GetLastDirection() * touchTime * EConfig.System.MOVE_SPEED;
        if (TryGetCorrectionPos(out Vector3 correctionPos)) {
            CurrentTargetPos = correctionPos;
        }

        Vector3 targetPos = transform.position + CurrentTargetPos;
        transform.DOMove(targetPos, 1f).SetEase(Ease.Linear);
        transform.DOJump(targetPos, 1f, 1, 1f).SetEase(Ease.InOutSine)
            .OnComplete(() => {
                SetRotation();
            });
    }

    private bool TryGetCorrectionPos(out Vector3 correctionPos) {
        Vector3 moveTargetPos = MapManager.Instance.CurrentMap.transform.position - transform.position;
        float differ = Vector3.Distance(CurrentTargetPos.SetY(0f), moveTargetPos.SetY(0f));

        if (differ <= EConfig.System.CORRECTION_VALUE) {
            correctionPos = moveTargetPos.SetY(0f);
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
        Debug.Log($"<color=yellow>[ChangeState] : {state}</color>");
        CurrentState = state;
        animator.Play(state.ToString());
    }
}
