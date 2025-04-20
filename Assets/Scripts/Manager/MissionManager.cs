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

    public void ReportProgress(MissionConditionType type, int amount)
    {
        foreach (var mission in allMissions.Where(m => m.conditionType == type))
        {
            string id = mission.id;
            var userData = UserManager.Instance.CurrentUserData;

            if (userData.claimedMissions.Contains(id)) continue;

            int current = userData.missionProgress.GetValueOrDefault(id, 0);
            int updated = Mathf.Min(current + amount, mission.requiredValue);

            userData.missionProgress[id] = updated;

            if (updated >= mission.requiredValue)
            {
                Debug.Log($"[MissionManager] 미션 달성: {mission.title}");
                // UI 팝업 연동 가능
            }
        }

        UserManager.Instance.UpdateUserData();
    }
    
    public int GetUnclaimedMissionCount()
    {
        var missions = MissionManager.Instance.GetAllMissions();
        var userData = UserManager.Instance.CurrentUserData;

        int count = 0;

        foreach (var mission in missions)
        {
            if (userData.claimedMissions.Contains(mission.id))
                continue;

            if (userData.missionProgress.TryGetValue(mission.id, out int progress) &&
                progress >= mission.requiredValue)
            {
                count++;
            }
            
            Debug.Log($"[MISSION DATA] id: {mission.id}, required: {mission.requiredValue}, current: {progress}");
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
        if (!userData.missionProgress.TryGetValue(missionId, out int progress) ||
            progress < mission.requiredValue)
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