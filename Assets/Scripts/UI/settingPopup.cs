using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingPopup : UIBase {
    [SerializeField] private Button btn_bgm;
    [SerializeField] private Button btn_sfx;
    [SerializeField] private Button btn_back;
    [SerializeField] private Image img_bgm_check;
    [SerializeField] private Image img_sfx_check;

    protected override void PrevOpen(params object[] args) {
        img_bgm_check.gameObject.SetActive(AudioManager.Instance.IsBgmMute);
        img_sfx_check.gameObject.SetActive(AudioManager.Instance.IsSfxMute);
    }

    protected override void PrevClose() { }

    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_back):
                Close();
                break;
            case nameof(btn_bgm):
                OnToggleBGMSettings();
                break;
            case nameof(btn_sfx):
                OnToggleSFXSettings();
                break;
        }
    }

    private void OnToggleBGMSettings()
    {
        bool value = !AudioManager.Instance.IsBgmMute;
        img_bgm_check.gameObject.SetActive(value);
        AudioManager.Instance.SetBGMSettings(GetAudioSettings(value));
    }

    private void OnToggleSFXSettings()
    {
        bool value = !AudioManager.Instance.IsSfxMute;
        img_sfx_check.gameObject.SetActive(value);
        AudioManager.Instance.SetSFXSettings(GetAudioSettings(value));
    }

    private Preferences.EAudioSettings GetAudioSettings(bool isMute)
    {
        return isMute ? Preferences.EAudioSettings.Mute : Preferences.EAudioSettings.Play;
    }

    // public void OnToggleEvent(Toggle inToggle) {
    //     Preferences.EAudioSettings value = inToggle.isOn ? Preferences.EAudioSettings.Mute 
    //                                                     : Preferences.EAudioSettings.Play;
    //     switch (inToggle.name) {
    //         case nameof(tgl_bgm):
    //             AudioManager.Instance.SetBGMSettings(value);
    //             break;
    //         case nameof(tgl_sfx):
    //             AudioManager.Instance.SetSFXSettings(value);
    //             break;
    //     }
    // }
}
