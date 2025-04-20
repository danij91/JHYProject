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

    protected override void PrevOpen(params object[] args)
    {
        btn_ok.AddOnClickListener(OnClickOk);
        btn_skip.AddOnClickListener(OnClickSkip);
    }

    private void SaveUserData(string nickname) {
        UserManager.Instance.SetUserNickname(nickname);
        SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY).Forget();
    }

    private void OnClickOk()
    {
        var nickname = inputField_nickname.text;
        SaveUserData(nickname);
    }

    private void OnClickSkip()
    {
        SaveUserData("unknown");
    }
}
