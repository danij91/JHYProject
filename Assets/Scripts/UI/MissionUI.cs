using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MissionUI : UIBase
{
    [SerializeField] private GameObject missionItemPrefab;
    [SerializeField] private Button closeButton;
    
    [SerializeField] private GameObject scroll_achievement;
    [SerializeField] private GameObject scroll_daily;

    [SerializeField] private Transform content_achievement;
    [SerializeField] private Transform content_daily;

    [SerializeField] private Button btn_tab_achievement;
    [SerializeField] private Button btn_tab_daily;

    [SerializeField] private Image bg_btn_achievement;
    [SerializeField] private Image bg_btn_daily;

    private Color btnDefaultColor = new Color32(244, 209, 155,255);
    private Color btnActiveColor = new Color32(253,252,229,255);
    
    protected override void PrevOpen(params object[] args)
    {
        closeButton.AddOnClickListener(() => Close());
        InitTabs();
        RenderMissions();
        ShowTab(MissionType.Achievement);
        
    }
    
    private void ShowTab(MissionType type)
    {
        scroll_achievement.SetActive(type == MissionType.Achievement);
        scroll_daily.SetActive(type == MissionType.Daily);
        btn_tab_achievement.interactable = type == MissionType.Daily;
        btn_tab_daily.interactable = type == MissionType.Achievement;
        
        bg_btn_achievement.color = (type == MissionType.Achievement)? btnActiveColor:btnDefaultColor;
        bg_btn_daily.color = (type == MissionType.Achievement)?btnDefaultColor:btnActiveColor;
    }
    
    private void InitTabs()
    {
        btn_tab_achievement.AddOnClickListener(() => ShowTab(MissionType.Achievement));
        btn_tab_daily.AddOnClickListener(() => ShowTab(MissionType.Daily));
    }
    
    
    private void RenderMissions()
    {
        RenderMissionsForTab(MissionType.Achievement, content_achievement);
        RenderMissionsForTab(MissionType.Daily, content_daily);
    }
    
    private void RenderMissionsForTab(MissionType type, Transform contentParent)
    {
        var userData = UserManager.Instance.CurrentUserData;

        // 필터링
        var all = MissionManager.Instance.GetAllMissions().Where(m => m.missionType == type).ToList();

        var claimed = all.Where(m => userData.claimedMissions.Contains(m.id)).ToList();
        var readyToClaim = all.Where(m => !userData.claimedMissions.Contains(m.id)
                                          && MissionManager.Instance.IsMissionCompleted(m)).ToList();
        var inProgress = all.Where(m => !userData.claimedMissions.Contains(m.id)
                                        && !MissionManager.Instance.IsMissionCompleted(m)).ToList();

        // 순서대로 렌더링
        RenderMissionList(readyToClaim, contentParent, false);
        RenderMissionList(inProgress, contentParent, false);
        RenderMissionList(claimed, contentParent, true); // ✅ disable = true
    }
    
    private void RenderMissionList(List<MissionData> missions, Transform parent, bool disable)
    {
        var userData = UserManager.Instance.CurrentUserData;

        foreach (var mission in missions)
        {
            GameObject go = Instantiate(missionItemPrefab, parent);
            var itemUI = go.GetComponent<MissionItem>();

            int progress = MissionManager.Instance.GetCurrentProgress(mission);
            bool claimed = userData.claimedMissions.Contains(mission.id);

            itemUI.Set(mission, progress, claimed);

            if (disable)
            {
                // CanvasGroup 사용해서 흐리게 + 상호작용 비활성화
                var canvasGroup = go.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = go.AddComponent<CanvasGroup>();

                canvasGroup.alpha = 0.5f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}