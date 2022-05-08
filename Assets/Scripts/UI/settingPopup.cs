using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingPopup : UIBase {
    [SerializeField] private Toggle tgl_bgm;
    [SerializeField] private Toggle tgl_sfx;
    [SerializeField] private Button btn_back;

    protected override void PrevOpen(params object[] args) {
        tgl_bgm.isOn = !AudioManager.Instance.IsBgmMute;
        tgl_bgm.isOn = !AudioManager.Instance.IsSfxMute;
    }

    protected override void PrevClose() { }

    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_back):
                UIManager.Instance.Close<settingPopup>();
                break;
        }
    }

    public void OnToggleEvent(Toggle inToggle) {
        switch (inToggle.name) {
            case nameof(tgl_bgm):
                AudioManager.Instance.SetBgmMute(!inToggle.isOn);
                break;
            case nameof(tgl_sfx):
                AudioManager.Instance.SetSfxMute(inToggle.isOn);
                break;
        }
    }
}
