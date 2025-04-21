using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionManager : Singleton<MissionManager>
{
    public List<MissionData> GetAllMissions() => allMissions;
    
    private List<MissionData> allMissions;
    
    public void Initialize() {
        LoadAllMissions();
    }

    private void LoadAllMissions()
    {
        allMissions = Resources.LoadAll<MissionData>("Data/Missions").ToList();
        Debug.Log($"[MissionManager] 미션 {allMissions.Count}개 로드 완료");
    }

    public void ReportProgressAccumulate(MissionConditionType type, int amount)
    {
        var userData = UserManager.Instance.CurrentUserData;

        foreach (var mission in allMissions.Where(m => m.conditionType == type && m.complexityType == MissionComplexityType.Complex))
        {
            if (userData.claimedMissions.Contains(mission.id)) continue;

            int current = userData.missionProgress.GetValueOrDefault(mission.id, 0);
            int updated = current + amount;

            if (updated > current)
            {
                userData.missionProgress[mission.id] = Mathf.Min(updated, mission.requiredValue);

                if (updated >= mission.requiredValue)
                {
                    Debug.Log($"[MissionManager] 미션 달성: {mission.title}");
                    // TODO: UI 팝업 표시
                }
            }
        }

        UserManager.Instance.UpdateUserData(); // 저장은 한 번만
    }
    
    public void ReportProgress(MissionConditionType type, int newValue)
    {
        var userData = UserManager.Instance.CurrentUserData;

        foreach (var mission in allMissions.Where(m => m.conditionType == type && m.complexityType == MissionComplexityType.Complex))
        {
            if (userData.claimedMissions.Contains(mission.id)) continue;

            int updated = Mathf.Min(newValue, mission.requiredValue);
            int prev = userData.missionProgress.GetValueOrDefault(mission.id, 0);

            userData.missionProgress[mission.id] = updated;

            if (prev < mission.requiredValue && updated >= mission.requiredValue)
            {
                Debug.Log($"[MissionManager] 미션 달성: {mission.title}");
                // TODO: UI 알림 등
            }
        }

        UserManager.Instance.UpdateUserData();
    }
    
    public void ResetDailyMissions()
    {
        var user = UserManager.Instance.CurrentUserData;

        foreach (var mission in allMissions)
        {
            if (mission.missionType == MissionType.Daily)
            {
                // 진행도 초기화
                user.missionProgress[mission.id] = 0;

                // 클레임한 경우엔 제거
                if (user.claimedMissions.Contains(mission.id))
                {
                    user.claimedMissions.Remove(mission.id);
                }
            }
        }

        Debug.Log("🔄 Daily mission progress & claims reset.");
        UserManager.Instance.UpdateUserData();
    }


    
    public bool IsMissionCompleted(MissionData mission)
    {
        var user = UserManager.Instance.CurrentUserData;

        if (mission.complexityType == MissionComplexityType.Simple)
        {
            int value = mission.conditionType switch
            {
                MissionConditionType.JumpCount => mission.evaluateType == MissionEvaluateType.Total ? user.totalJump : user.maxJump,
                MissionConditionType.ComboCount => mission.evaluateType == MissionEvaluateType.Total ? user.totalCombo : user.maxCombo,
                MissionConditionType.ScoreReach => user.maxScore,
                MissionConditionType.PlayCount => user.totalPlayCount,
                MissionConditionType.AdWatchedCount => user.adWatchedCount,
                MissionConditionType.CharacterUnlockedCount => user.characters.Count,
            };

            return value >= mission.requiredValue;
        }
        else
        {
            return user.missionProgress.TryGetValue(mission.id, out int progress) && progress >= mission.requiredValue;
        }
    }
    
    public int GetCurrentProgress(MissionData mission)
    {
        var user = UserManager.Instance.CurrentUserData;

        return mission.complexityType == MissionComplexityType.Simple
            ? mission.conditionType switch
            {
                MissionConditionType.JumpCount => mission.evaluateType == MissionEvaluateType.Total ? user.totalJump : user.maxJump,
                MissionConditionType.ComboCount => mission.evaluateType == MissionEvaluateType.Total ? user.totalCombo : user.maxCombo,
                MissionConditionType.ScoreReach => user.maxScore,
                MissionConditionType.PlayCount => user.totalPlayCount,
                MissionConditionType.AdWatchedCount => user.adWatchedCount,
                MissionConditionType.CharacterUnlockedCount => user.characters.Count,
                _ => 0
            }
            : user.missionProgress.GetValueOrDefault(mission.id, 0);
    }
    
    public int GetUnclaimedMissionCount()
    {
        var missions = GetAllMissions();
        var user = UserManager.Instance.CurrentUserData;

        int count = 0;
        foreach (var mission in missions)
        {
            if (user.claimedMissions.Contains(mission.id))
                continue;

            if (IsMissionCompleted(mission))
                count++;
        }

        return count;
    }

    public void ClaimReward(string missionId)
    {
        var mission = allMissions.FirstOrDefault(m => m.id == missionId);
        var userData = UserManager.Instance.CurrentUserData;

        if (mission == null) return;

        // 이미 수령했는지 확인
        if (userData.claimedMissions.Contains(missionId)) return;

        // 미션 조건 만족 여부 확인
        if (!IsMissionCompleted(mission))
            return;

        // 보상 적용
        if (mission.rewardType == CurrencyType.Soft)
            userData.coin += mission.rewardAmount;
        else if (mission.rewardType == CurrencyType.Hard)
            userData.gem += mission.rewardAmount;

        // 수령 처리
        userData.claimedMissions.Add(missionId);

        // 저장
        UserManager.Instance.UpdateUserData();
    }
}