using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : UIBase {
    [SerializeField] private Button btn_gameStart;
    [SerializeField] private Button btn_inventory;
    [SerializeField] private Button btn_setting;
    [SerializeField] private Button btn_ranking;
    [SerializeField] private Button btn_mission;
    [SerializeField] private Button btn_language;
    [SerializeField] private TMP_Text txt_unclaimedMission;
    [SerializeField] private GameObject go_unclaimedMission;

    protected override void PrevOpen(params object[] args)
    {
        btn_gameStart.AddOnClickListener(()=>SceneLoader.Instance.ChangeSceneAsync(EScene.INGAME, true).Forget());
        btn_setting.AddOnClickListener(()=>UIManager.Instance.Show<SettingPopup>());
        btn_inventory.AddOnClickListener(()=>UIManager.Instance.Show<CharacterInvenUI>());
        btn_ranking.AddOnClickListener(()=>UIManager.Instance.Show<RankingUI>());
        btn_mission.AddOnClickListener(()=>UIManager.Instance.Show<MissionUI>());
        btn_language.AddOnClickListener(()=>UIManager.Instance.Show<LanguagePopup>());
        
        RefreshMissionBtn();
    }

    private void RefreshMissionBtn()
    {
        int count = MissionManager.Instance.GetUnclaimedMissionCount();

        if (count != 0)
        {
            txt_unclaimedMission.text = count.ToString();
            go_unclaimedMission.SetActive(true);
        }
        else
        {
            go_unclaimedMission.SetActive(false);
        }
    }


    public override void OnResume()
    {
        Debug.Log("debug");
        RefreshMissionBtn();
    }
    
    protected override void PrevClose() { }
}
