using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PoolingObject
{
    public enum PLAYER_STATE
    {
        IDLE,
        CROUCH,
        JUMP,
        FALL
    }

    [SerializeField] private CharacterController characterController;
    [SerializeField] private CharacterVisual characterVisual;
    [SerializeField] private JumpGauge jumpGauge;

    [Header("Config")] [SerializeField] private float correctionThreshold = 0.5f;

    private Rigidbody rb;
    private CharacterData characterData;
    private Vector3 currentTargetPos;

    private readonly Dictionary<PLAYER_STATE, string> animationKeyMap = new()
    {
        { PLAYER_STATE.IDLE, "Idle_A" },
        { PLAYER_STATE.JUMP, "Fly" },
        { PLAYER_STATE.CROUCH, "Fear" },
        { PLAYER_STATE.FALL, "Death" },
    };

    public PLAYER_STATE CurrentState { get; private set; } = PLAYER_STATE.IDLE;
    public bool IsJumping => CurrentState == PLAYER_STATE.JUMP;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // 풀에서 꺼낼 때 호출됨
    internal override void OnInitialize(params object[] parameters)
    {
        rb.isKinematic = false;
        transform.position = MapManager.Instance.StartPos;

        if (parameters.Length > 0 && parameters[0] is CharacterData data)
        {
            SetCharacter(data);
        }

        characterVisual.SetRotation(MapManager.Instance.GetLastDirection());
        jumpGauge.SetJumpGauge(0f);
        jumpGauge.gameObject.SetActive(true);
    }

    protected override void OnUse()
    {
    }

    // 풀로 복귀할 때 호출됨
    protected override void OnRestore()
    {
        rb.isKinematic = true;
        jumpGauge.gameObject.SetActive(false);
    }

    public void SetRotation()
    {
        characterVisual.SetRotationFromMap();
    }

    public void SetCharacter(CharacterData data)
    {
        characterData = data;

        GameObject model = characterController.SpawnModel(data.modelPrefab);
        characterVisual.BindModel(model);
        characterVisual.SetRotation(MapManager.Instance.GetLastDirection());
    }

    public void ChangeState(PLAYER_STATE newState)
    {
        CurrentState = newState;
        characterVisual.PlayAnimation(animationKeyMap[newState]);

        if (newState == PLAYER_STATE.FALL)
        {
            AudioManager.Instance.SFXPlay(SFXType.Fall);
        }
    }

    public void UpdateGauge(float elapsedTime)
    {
        float ratio = Mathf.Clamp01(elapsedTime / characterData.maxChargeTime);
        jumpGauge.SetJumpGauge(ratio);
    }

    public void Jump(float elapsedTime)
    {
        jumpGauge.SetJumpGauge(0f);
        ChangeState(PLAYER_STATE.JUMP);

        Vector3 jumpDir = GetJumpDirection();
        currentTargetPos = jumpDir * elapsedTime * characterData.jumpDistance;

        if (TryGetCorrectionPos(out Vector3 correctionPos))
        {
            currentTargetPos = correctionPos;
            GameManager.Instance.SuccessCombo();
        }
        else
        {
            GameManager.Instance.FailCombo();
        }

        Vector3 target = transform.position + currentTargetPos;
        AudioManager.Instance.SFXPlay(SFXType.Jump);
        characterVisual.PlayAnimation(animationKeyMap[PLAYER_STATE.JUMP]);

        characterController.PerformJump(target, characterData.jumpPower, characterData.jumpDuration, this);
    }

    public void OnJumpComplete()
    {
        ChangeState(PLAYER_STATE.IDLE);
    }

    private bool TryGetCorrectionPos(out Vector3 correctionPos)
    {
        Vector3 mapTarget = MapManager.Instance.CurrentMap.transform.position - transform.position;
        float distance = Vector3.Distance(currentTargetPos.SetY(0f), mapTarget.SetY(0f));

        if (distance <= correctionThreshold)
        {
            correctionPos = mapTarget.SetY(0f);
            return true;
        }

        correctionPos = Vector3.zero;
        return false;
    }

    private Vector3 GetJumpDirection()
    {
        return (MapManager.Instance.CurrentMap.transform.position - transform.position).normalized;
    }
}