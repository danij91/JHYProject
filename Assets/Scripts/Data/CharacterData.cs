using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game/CharacterData")]
public class CharacterData : ScriptableObject {
    [Header("기본 정보")]
    public string characterId; 
    public string characterName;
    public GameObject modelPrefab;  // 모델 프리팹 (Animator 포함)
    public Sprite thumbnail;
    
    [Header("점프 설정")]
    public float maxChargeTime = 2.0f;         // 최대 충전 시간
    public float jumpDistance = 10f;           // 최대 점프 거리
    public float jumpPower = 1.5f;             // DOTween Jump Power
    public float jumpDuration = 0.3f;          // DOTween Duration

    [Header("애니메이션")]
    public string jumpAnimation = "JUMP";      // 점프 시 재생할 애니메이션 이름

    [Header("구매관련")] 
    public int gemPrice = -1;
    public int coinPrice = -1;
    
    // 향후 확장 가능:
    // public string fallAnimation = "FALL";
    // public AudioClip jumpSfx;
}