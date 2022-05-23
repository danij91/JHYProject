using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknamePopup : UIBase {
    [SerializeField] private TMP_InputField inputField_nickname;
    [SerializeField] private Button btn_ok;
    [SerializeField] private Button btn_skip;

    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_ok):
                var nickname = inputField_nickname.text;
                SaveUserData(nickname);
                break;
            case nameof(btn_skip):
                SaveUserData("unknown");
                break;
        }
    }

    private void SaveUserData(string nickname) {
        DataManager.Instance.SetUserNickname(nickname);
        SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY).Forget();
    }
}
