using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Game/Mission Data")]
public class MissionData : ScriptableObject
{
    [Header("기본 정보")]
    public string id;                     // 고유 ID (ex. "daily_jump_10")
    public MissionType missionType;       // 일일, 업적, 이벤트
    public string title;                  // UI 제목
    public string description;            // UI 설명

    [Header("조건 설정")]
    public MissionConditionType conditionType;  // 콤보, 점프, 죽음 등
    public int requiredValue;             // 필요한 수치 (ex. 10번 점프)

    [Header("보상 설정")]
    public CurrencyType rewardType;       // 보상 화폐 종류
    public int rewardAmount;               // 보상 수치

    [Header("이벤트 한정 설정")]
    public bool isTimeLimited;
    public DateTime startTime;
    public DateTime endTime;
}

public enum MissionType
{
    Achievement,  // 영구 도전과제
    Daily,        // 일일 퀘스트
    Event         // 기간 한정 미션
}

public enum MissionConditionType
{
    ComboCount,
    JumpCount,
    DeathCount,
    ScoreReach,
    PlayCount,
}

public enum CurrencyType
{
    Soft,    // 무료 화폐 (코인)
    Hard     // 유료 화폐 (다이아)
}