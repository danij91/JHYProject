using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UIBase {
    [SerializeField] private Button btn_touch;
    [SerializeField] private GameObject tmp_touch;

    private float elapsedTime;

    protected override void PrevOpen(params object[] args) {
        elapsedTime = 0f;
        btn_touch.AddOnClickListener(OnClickTouch);
    }

    protected override void PrevClose() { }

    private void Update() {
        elapsedTime += Time.deltaTime;
        float ms = elapsedTime % 1;
        if (ms < 0.66f)
            tmp_touch.SetActive(true);
        else
            tmp_touch.SetActive(false);
    }

    private async void OnClickTouch()
    {
        try
        {
            if (UserManager.Instance.IsSignedIn())
            {
                await UserManager.Instance.LoadUserData();
                SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY).Forget();
                return;
            }
                
            UIManager.Instance.Show<SignInUI>();
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }
}
