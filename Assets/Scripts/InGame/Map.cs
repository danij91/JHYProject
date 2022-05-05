using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMapType {
    BrickMap,
    ConcreteMap,
    GrassMap,
    WoodMap,
}

public class Map : PoolingObject {
    [SerializeField]
    private EMapType mapType;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip normalSuccessClip;
    [SerializeField]
    private AudioClip perfectSuccessClip;

    private void OnTriggerEnter(Collider other) {
        audioSource.PlayOneShot(GameManager.Instance.IsPerfectJump ? perfectSuccessClip : normalSuccessClip);
        GameManager.Instance.Player.ChangeState(Player.PLAYER_STATE.IDLE);
        if (MapManager.Instance.CurrentMap == this)
            GameManager.Instance.OnSuccess();
    }

    internal override void OnInitialize(params object[] parameters) {
        audioSource = GetComponent<AudioSource>();
        SetRandomSize();
    }

    private void SetRandomSize() {
        float size = Random.Range(EConfig.Map.MIN_SIZE, EConfig.Map.MAX_SIZE);
        transform.localScale = new Vector3(size, transform.localScale.y, size);
    }

    protected override void OnUse() { }

    protected override void OnRestore() { }
}
