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
    [SerializeField] private Button btn_getHeart;
    [SerializeField] private Button btn_getCoin;
    [SerializeField] private Button btn_getGem;
    [SerializeField] private Image image_missionIcon;
    [SerializeField] private TMP_Text txt_coin;
    [SerializeField] private TMP_Text txt_gem;
    [SerializeField] private TMP_Text txt_heart;
    [SerializeField] private Sprite sprite_missionIdle;
    [SerializeField] private Sprite sprite_missionWait;

    protected override void PrevOpen(params object[] args)
    {
        btn_gameStart.AddOnClickListener(()=>SceneLoader.Instance.ChangeSceneAsync(EScene.INGAME, true).Forget());
        btn_setting.AddOnClickListener(()=>UIManager.Instance.Show<SettingPopup>());
        btn_inventory.AddOnClickListener(()=>UIManager.Instance.Show<CharacterInvenUI>());
        btn_ranking.AddOnClickListener(()=>UIManager.Instance.Show<RankingUI>());
        btn_mission.AddOnClickListener(()=>UIManager.Instance.Show<MissionUI>());
        btn_language.AddOnClickListener(()=>UIManager.Instance.Show<LanguagePopup>());
        btn_getHeart.AddOnClickListener(()=>Debug.Log("getHeart called"));
        btn_getCoin.AddOnClickListener(()=>Debug.Log("getCoin called"));
        btn_getGem.AddOnClickListener(()=>Debug.Log("getGem called"));
        txt_coin.text = UserManager.Instance.CurrentUserData.coin.ToString();
        txt_gem.text = UserManager.Instance.CurrentUserData.gem.ToString();
        txt_heart.text = "1 / 5";
        RefreshMissionBtn();
    }

    private void RefreshMissionBtn()
    {
        int count = MissionManager.Instance.GetUnclaimedMissionCount();

        if (count != 0)
        {
            image_missionIcon.sprite = sprite_missionWait;
        }
        else
        {
            image_missionIcon.sprite = sprite_missionIdle;
        }
    }


    public override void OnResume()
    {
        Debug.Log("debug");
        RefreshMissionBtn();
    }
    
    protected override void PrevClose() { }
}
