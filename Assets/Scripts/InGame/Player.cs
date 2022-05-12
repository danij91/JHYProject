using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ECharacterType
{
    Chick,
    Crocodile,
    Dog,
    Dolphin,
    Dove,
    Lizard,
    SeaLion,
    Squid,
}

public class Player : PoolingObject {
    public enum PLAYER_STATE {
        IDLE,
        JUMP,
        CROUCH,
        FALL
    }

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private JumpGauge gauge;

    private Ease MOVE_EASE = Ease.OutFlash;
    private Ease JUMP_EASE = Ease.OutFlash;
    private float JUMP_DURATION = 0.3f;
    private float JUMP_POWER = 1.5f;

    private Rigidbody rigidbody;

    public ECharacterType CharacterType { get; private set; }
    public PLAYER_STATE CurrentState { get; private set; }
    public Vector3 CurrentTargetPos { get; private set; }
    public bool IsJumping => CurrentState == PLAYER_STATE.JUMP;

    internal override void OnInitialize(params object[] parameters) {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();

        if (parameters.Length > 0)
            CharacterType = (ECharacterType)parameters[0];

        rigidbody.isKinematic = false;
        transform.position = MapManager.Instance.StartPos;
        SetRotation();
    }

    protected override void OnUse() { }

    protected override void OnRestore() {
        rigidbody.isKinematic = true;
    }

    public void Jump(float touchTime) {
        gauge.SetJumpGauge(0f);
        ChangeState(PLAYER_STATE.JUMP);
        CurrentTargetPos = GetJumpDirection() * touchTime * EConfig.System.MOVE_SPEED;
        bool isPerfectJump = TryGetCorrectionPos(out Vector3 correctionPos);
        if (isPerfectJump) {
            CurrentTargetPos = correctionPos;
            GameManager.Instance.SuccessCombo();
        }
        else {
            GameManager.Instance.FailCombo();
        }

        Vector3 targetPos = transform.position + CurrentTargetPos;
        AudioManager.Instance.SFXPlay(SFXType.Jump);
        transform.DOMove(targetPos, JUMP_DURATION).SetEase(MOVE_EASE);
        transform.DOJump(targetPos, JUMP_POWER, 1, JUMP_DURATION).SetEase(JUMP_EASE);
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

    private Vector3 GetJumpDirection() {
        return (MapManager.Instance.CurrentMap.transform.position - transform.position).normalized;
    }

    public void SetRotation() {
        Vector3 direction = MapManager.Instance.GetLastDirection();
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void ChangeState(PLAYER_STATE state) {
        Debug.Log($"<color=yellow>[ChangeState] : {state}</color>");
        CurrentState = state;
        animator.Play(state.ToString());

        if (state == PLAYER_STATE.FALL) {
            AudioManager.Instance.SFXPlay(SFXType.Fall);
        }
    }

    public void UpdateGauge(float time) {
        float value = time;
        gauge.SetJumpGauge(value);
    }
}
