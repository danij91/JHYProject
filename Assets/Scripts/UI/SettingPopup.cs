using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPopup : UIBase {
    [SerializeField] private Button btn_bgm;
    [SerializeField] private Button btn_sfx;
    [SerializeField] private Button btn_back;
    [SerializeField] private Button btn_signOut;
    [SerializeField] private Image img_bgm_check;
    [SerializeField] private Image img_sfx_check;

    private string signOutTitle;
    private string signOutMessage;

    protected override void PrevOpen(params object[] args) {
        img_bgm_check.gameObject.SetActive(!AudioManager.Instance.IsBgmMute);
        img_sfx_check.gameObject.SetActive(!AudioManager.Instance.IsSfxMute);

        signOutTitle = LocalizationManager.Instance.GetLocalizedText("setting_signOutTitle");
        signOutMessage = LocalizationManager.Instance.GetLocalizedText("setting_signOutMessage");
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
            case nameof(btn_signOut):
                if (UserManager.Instance.IsAnonymous()) {
                    UIManager.Instance.Show<MessageBoxUI>(ui => {
                        ui.SetMessage(
                            signOutMessage
                            , signOutTitle
                            , () => {
                                UserManager.Instance.SignOut();
                                SceneLoader.Instance.ChangeSceneAsync(EScene.TITLE).Forget();
                            }, null);
                    });
                    return;
                }

                UserManager.Instance.SignOutFromGoogle();
                SceneLoader.Instance.ChangeSceneAsync(EScene.TITLE).Forget();

                break;
        }
    }

    private void OnToggleBGMSettings() {
        bool value = !AudioManager.Instance.IsBgmMute;
        img_bgm_check.gameObject.SetActive(!value);
        AudioManager.Instance.SetBGMSettings(GetAudioSettings(value));
    }

    private void OnToggleSFXSettings() {
        bool value = !AudioManager.Instance.IsSfxMute;
        img_sfx_check.gameObject.SetActive(!value);
        AudioManager.Instance.SetSFXSettings(GetAudioSettings(value));
    }

    private Preferences.EAudioSettings GetAudioSettings(bool isMute) {
        return isMute ? Preferences.EAudioSettings.Mute : Preferences.EAudioSettings.Play;
    }
}
