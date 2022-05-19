using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : UIBase {
    [SerializeField] private Button btn_gameStart;
    [SerializeField] private Button btn_inventory;
    [SerializeField] private Button btn_setting;

    protected override void PrevOpen(params object[] args) { }

    protected override void PrevClose() { }

    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_gameStart):
                SceneLoader.Instance.ChangeSceneAsync(EScene.INGAME, true).Forget();
                break;
            case nameof(btn_setting):
                UIManager.Instance.Show<settingPopup>();
                break;
            case nameof(btn_inventory):
                UIManager.Instance.Show<CharacterInvenUI>();
                break;
        }
    }
}
