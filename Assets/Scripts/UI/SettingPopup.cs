using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingPopup : UIBase
{
    [SerializeField] private ToggleSwitchButton tglBtn_bgm;
    [SerializeField] private ToggleSwitchButton tglBtn_sfx;
    [SerializeField] private Button btn_back;
    [SerializeField] private Button btn_signOut;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private string signOutTitle;
    private string signOutMessage;

    protected override void PrevOpen(params object[] args)
    {
        btn_back.AddOnClickListener(() => Close());
        tglBtn_bgm.Bind(() => !AudioManager.Instance.IsBgmMute,
            v => AudioManager.Instance.SetBGMSettings(GetAudioSettings(!v)));
        tglBtn_sfx.Bind(() => !AudioManager.Instance.IsSfxMute,
            v => AudioManager.Instance.SetSFXSettings(GetAudioSettings(!v)));
        btn_signOut.AddOnClickListener(OnClickSignOut);

        signOutTitle = LocalizationManager.Instance.GetLocalizedText("setting_signOutTitle");
        signOutMessage = LocalizationManager.Instance.GetLocalizedText("setting_signOutMessage");

        bgmSlider.minValue = 0.0001f;
        sfxSlider.minValue = 0.0001f;

        bgmSlider.value = AudioManager.Instance.BGMVolume;
        sfxSlider.value = AudioManager.Instance.SFXVolume;

        bgmSlider.onValueChanged.AddListener((value) => { AudioManager.Instance.SetBGMVolume(value); });

        sfxSlider.onValueChanged.AddListener((value) => { AudioManager.Instance.SetSFXVolume(value); });
    }

    protected override void PrevClose()
    {
    }

    // private void OnToggleSFXSettings() {
    //     bool value = !AudioManager.Instance.IsSfxMute;
    //     img_sfx_check.gameObject.SetActive(!value);
    //     AudioManager.Instance.SetSFXSettings(GetAudioSettings(value));
    // }

    private void OnClickSignOut()
    {
        if (UserManager.Instance.IsAnonymous())
        {
            UIManager.Instance.Show<MessageBoxUI>(ui =>
            {
                ui.SetMessage(
                    signOutMessage
                    , signOutTitle
                    , () =>
                    {
                        UserManager.Instance.SignOut();
                        SceneLoader.Instance.ChangeSceneAsync(EScene.TITLE).Forget();
                    }, null);
            });
            return;
        }

        UserManager.Instance.SignOutFromGoogle();
        SceneLoader.Instance.ChangeSceneAsync(EScene.TITLE).Forget();
    }

    private Preferences.EAudioSettings GetAudioSettings(bool isMute)
    {
        return isMute ? Preferences.EAudioSettings.Mute : Preferences.EAudioSettings.Play;
    }
}