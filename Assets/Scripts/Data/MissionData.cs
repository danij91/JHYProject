// 리팩토링된 MissionData.cs
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Game/Mission Data")]
public class MissionData : ScriptableObject
{
    [Header("기본 정보")]
    public string id;                     // 고유 ID (ex. "daily_jump_10")
    public MissionType missionType;      // 일일, 업적, 이벤트
    public string title;                 // UI 제목 (Localization Key)
    public string description;           // UI 설명 (Localization Key)

    [Header("조건 설정")]
    public MissionConditionType conditionType;      // 콤보, 점프, 등 조건 종류
    public MissionEvaluateType evaluateType;        // 누적, 최고 등 평가 방식
    public MissionComplexityType complexityType;    // 단순 조건 or 복합 조건
    public int requiredValue;                       // 필요한 수치 (ex. 10번 점프)

    [Header("보상 설정")]
    public CurrencyType rewardType;       // 보상 화폐 종류
    public int rewardAmount;              // 보상 수치

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

public enum MissionEvaluateType
{
    Total,   // 누적 값
    Max      // 한 판에서의 최고 기록
}

public enum MissionComplexityType
{
    Simple,  // 핵심 스탯으로 판단 가능
    Complex  // 복합 조건 (별도 로직 필요)
}

public enum MissionConditionType
{
    ComboCount,
    JumpCount,
    ScoreReach,
    PlayCount,
    AdWatchedCount,
    CharacterUnlockedCount,
    TutorialCompleted
}

public enum CurrencyType
{
    Soft,    // 무료 화폐 (코인)
    Hard     // 유료 화폐 (젬)
}