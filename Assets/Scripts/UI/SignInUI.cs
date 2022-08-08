using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SignInUI : UIBase {
    [SerializeField] private Button btn_signInGoogle;
    [SerializeField] private Button btn_signInApple;
    [SerializeField] private Button btn_signInEmail;
    [SerializeField] private Button btn_signInGuest;
    [SerializeField] private Button btn_skip;
    [SerializeField] private Image img_guestKr;
    [SerializeField] private Image img_guestEn;

    protected override void PrevOpen(params object[] args) {
        bool isKorean = LocalizationManager.Instance.GetCurrentLanguage() == 1;
        img_guestKr.gameObject.SetActive(isKorean);
        img_guestEn.gameObject.SetActive(!isKorean);

        btn_signInGuest.targetGraphic = isKorean ? img_guestKr : img_guestEn;
#if UNITY_EDITOR
        btn_skip.gameObject.SetActive(true);
#endif
    }

    protected override void PrevClose() { }

    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_signInGoogle):
                UserManager.Instance.SignInWithGoogle(() => { UIManager.Instance.Show<NicknamePopup>(); },
                    () => { SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY).Forget(); });
                break;
            case nameof(btn_signInApple):
                UserManager.Instance.SignInWithApple(() => { UIManager.Instance.Show<NicknamePopup>(); });
                break;
            case nameof(btn_signInEmail):
                UserManager.Instance.SignInWithEmail(() => { UIManager.Instance.Show<NicknamePopup>(); });
                break;
            case nameof(btn_signInGuest):
                UserManager.Instance.SignInAnonymously(() => { UIManager.Instance.Show<NicknamePopup>(); });
                break;
            case nameof(btn_skip):
                SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY).Forget();
                break;
        }
    }
}
