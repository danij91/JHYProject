using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MissionUI : UIBase
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject missionItemPrefab;
    [SerializeField] private Button closeButton;

    protected override void PrevOpen(params object[] args) {
        closeButton.AddOnClickListener(()=>Close());
        RenderMissions();
    }
    
    private void RenderMissions()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
        var userData = UserManager.Instance.CurrentUserData;
        var missions = MissionManager.Instance.GetAllMissions()
            .OrderBy(m => userData.claimedMissions.Contains(m.id)) // false 먼저
            .ThenBy(m => m.missionType) // 업적/일일/이벤트 순
            .ToList();

        foreach (var mission in missions)
        {
            GameObject go = Instantiate(missionItemPrefab, contentParent);
            var itemUI = go.GetComponent<MissionItem>();

            int progress = userData.missionProgress.GetValueOrDefault(mission.id, 0);
            bool claimed = userData.claimedMissions.Contains(mission.id);

            itemUI.Set(mission, progress, claimed);
        }
    }
}