using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : UIBase
{
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
        UserManager.Instance.RefreshEnergy();
        btn_gameStart.AddOnClickListener(OnClickGameStart);
        btn_setting.AddOnClickListener(() => UIManager.Instance.Show<SettingPopup>());
        btn_inventory.AddOnClickListener(() => UIManager.Instance.Show<CharacterInvenUI>());
        btn_ranking.AddOnClickListener(() => UIManager.Instance.Show<RankingUI>());
        btn_mission.AddOnClickListener(() => UIManager.Instance.Show<MissionUI>());
        btn_language.AddOnClickListener(() => UIManager.Instance.Show<LanguagePopup>());
        btn_getHeart.AddOnClickListener(OnClickChargeEnergy);
        btn_getCoin.AddOnClickListener(() => Debug.Log("getCoin called"));
        btn_getGem.AddOnClickListener(() => Debug.Log("getGem called"));
        RefreshLobbyUI();
    }

    private void RefreshLobbyUI()
    {
        int count = MissionManager.Instance.GetUnclaimedMissionCount();

        txt_coin.text = UserManager.Instance.CurrentUserData.coin.ToString();
        txt_gem.text = UserManager.Instance.CurrentUserData.gem.ToString();
        txt_heart.text = $"{UserManager.Instance.CurrentUserData.energy} / {EConfig.System.MAX_ENERGY_COUNT}";

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
        UserManager.Instance.RefreshEnergy();
        RefreshLobbyUI();
    }

    private void OnClickGameStart()
    {
        UserManager.Instance.RefreshEnergy();

        if (UserManager.Instance.TryConsumeEnergy())
        {
            SceneLoader.Instance.ChangeSceneAsync(EScene.INGAME, true).Forget();
        }
        else
        {
            Debug.Log("Out of energy");
        }
    }

    private void OnClickChargeEnergy()
    {
        var user = UserManager.Instance.CurrentUserData;
        string message = "";
        if (user.energy >= EConfig.System.MAX_ENERGY_COUNT)
        {
            message = "에너지가 모두 찼습니다!";
        }
        else
        {
            DateTime lastTime = user.energyLastUpdated.ToDateTime();
            TimeSpan timePassed = DateTime.UtcNow - lastTime;

            int passedSeconds = (int)timePassed.TotalSeconds;
            int intervalSeconds = EConfig.System.ENERGY_RECOVER_INTERVAL_MINUTES * 60;

            int remainingSeconds = Mathf.Max(0, intervalSeconds - passedSeconds);

            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            message = $"다음 충전까지 남은 시간: {minutes:D2}분 {seconds:D2}초";
        }

        Debug.Log(message);
    }

    protected override void PrevClose()
    {
    }
}