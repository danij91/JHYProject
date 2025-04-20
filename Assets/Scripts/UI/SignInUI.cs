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
        btn_signInGoogle.AddOnClickListener(OnClickSignInGoogle);
        btn_signInApple.AddOnClickListener(OnClickSignInApple);
        btn_signInEmail.AddOnClickListener(OnClickSignInEmail);
        btn_signInGuest.AddOnClickListener(OnClickSignInGuest);
        btn_skip.AddOnClickListener(OnClickSkip);
        
        bool isKorean = LocalizationManager.Instance.GetCurrentLanguage() == 1;
        img_guestKr.gameObject.SetActive(isKorean);
        img_guestEn.gameObject.SetActive(!isKorean);

        btn_signInGuest.targetGraphic = isKorean ? img_guestKr : img_guestEn;
#if UNITY_EDITOR
        btn_skip.gameObject.SetActive(true);
#endif
    }

    protected override void PrevClose() { }

    private void OnClickSignInGoogle()
    {
        UserManager.Instance.SignInWithGoogle(() => { UIManager.Instance.Show<NicknamePopup>(); },
            () => { SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY).Forget(); });
    }
    
    private void OnClickSignInApple()
    {
        UserManager.Instance.SignInWithApple(() => { UIManager.Instance.Show<NicknamePopup>(); });
    }
    
    private void OnClickSignInEmail()
    {
        UserManager.Instance.SignInWithEmail(() => { UIManager.Instance.Show<NicknamePopup>(); });
    }
    
    private void OnClickSignInGuest()
    {
        UserManager.Instance.SignInAnonymously(() => { UIManager.Instance.Show<NicknamePopup>(); });
    }
    
    private void OnClickSkip()
    {
        SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY).Forget();
    }
}
