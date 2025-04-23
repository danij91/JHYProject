using UnityEngine;
using DG.Tweening;

public class CharacterController : MonoBehaviour {
    [SerializeField] private Transform visualRoot;

    [Header("DOTween Config")]
    [SerializeField] private Ease moveEase = Ease.OutFlash;
    [SerializeField] private Ease jumpEase = Ease.OutFlash;

    /// <summary>
    /// 외부에서 호출하는 점프 실행
    /// </summary>
    public void PerformJump(Vector3 targetPosition, float jumpPower, float jumpDuration, PlayerController player) {
        transform.DOMove(targetPosition, jumpDuration).SetEase(moveEase);
        
        transform.DOJump(targetPosition, jumpPower, 1, jumpDuration)
            .SetEase(jumpEase)
            .OnComplete(() => player.OnJumpComplete());
    }

    /// <summary>
    /// visualRoot 하위에 캐릭터 모델 프리팹을 생성
    /// </summary>
    public GameObject SpawnModel(GameObject prefab) {
        // 기존 모델 제거
        foreach (Transform child in visualRoot) {
            Destroy(child.gameObject);
        }

        GameObject instance = Instantiate(prefab, visualRoot);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one * 2f;
        instance.GetComponent<Collider>().enabled = false;
        return instance;
    }
}