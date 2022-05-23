using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SignInUI : UIBase {
    [SerializeField] private Button btn_signUp;
    [SerializeField] private Button btn_guest;
    [SerializeField] private Button btn_skip;

    protected override void PrevOpen(params object[] args) {
#if UNITY_EDITOR
        btn_skip.gameObject.SetActive(true);
#endif
    }

    protected override void PrevClose() { }

    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_signUp):
                break;
            case nameof(btn_guest):
                DataManager.Instance.SignInAnonymously(() => { UIManager.Instance.Show<NicknamePopup>(); });
                break;
            case nameof(btn_skip):
                SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY).Forget();
                break;
        }
    }
}
