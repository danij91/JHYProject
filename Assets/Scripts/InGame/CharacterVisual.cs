using System.Collections.Generic;
using UnityEngine;

public class CharacterVisual : MonoBehaviour {
    private Animator animator;
    private SkinnedMeshRenderer skinnedMesh;
    
    /// <summary>
    /// 캐릭터 모델을 받아 내부 참조 연결 (Animator, MeshRenderer)
    /// </summary>
    public void BindModel(GameObject model, RuntimeAnimatorController overrideController = null) {
        animator = model.GetComponent<Animator>();
        skinnedMesh = model.GetComponentInChildren<SkinnedMeshRenderer>();

        if (animator && overrideController != null) {
            animator.runtimeAnimatorController = overrideController;
        }
    }

    /// <summary>
    /// 애니메이션 이름으로 안전하게 재생
    /// </summary>
    public void PlayAnimation(string animationName) {
        if (animator == null) return;

        int hash = Animator.StringToHash(animationName);
        if (animator.HasState(0, hash)) {
            animator.Play(hash);
        } else {
            Debug.LogWarning($"[CharacterVisual] Animator state not found: {animationName}");
        }
    }

    /// <summary>
    /// 캐릭터 회전 세팅 (외부 방향 기준)
    /// </summary>
    public void SetRotation(Vector3 direction) {
        if (direction.sqrMagnitude > 0.001f) {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// 맵 기준 초기 방향 세팅
    /// </summary>
    public void SetRotationFromMap() {
        SetRotation(MapManager.Instance.GetLastDirection());
    }

    /// <summary>
    /// 셰이프키 조절용 (확장 시 사용)
    /// </summary>
    public void SetBlendShape(string key, float value) {
        if (skinnedMesh == null) return;

        int index = skinnedMesh.sharedMesh.GetBlendShapeIndex(key);
        if (index >= 0) {
            skinnedMesh.SetBlendShapeWeight(index, value);
        }
    }

    /// <summary>
    /// 필요 시 캐릭터 모델 숨기기
    /// </summary>
    public void Hide() {
        if (animator != null) animator.gameObject.SetActive(false);
    }
}