using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class InGameUI : UIBase {
    [SerializeField]
    private Button btn_back;
    [SerializeField]
    private Button btn_screen;
    [SerializeField]
    private Button btn_restart;
    [SerializeField]
    private GameObject failPopup;
    [SerializeField]
    private TMP_Text txt_currentcount;
    [SerializeField]
    private TMP_Text txt_bestcount;
    [SerializeField]
    private TMP_Text txt_combocount;
    [SerializeField]
    private TMP_Text txt_endscore;
    private string score;
    private string exitTitle;
    private string exitMessage;

    private float elapsedTime;
    public bool IsScreenBtnDown { get; set; }

    protected override void PrevOpen(params object[] args) {
        SetView();
        score = LocalizationManager.Instance.GetLocalizedText("inGame_score");
        exitTitle = LocalizationManager.Instance.GetLocalizedText("inGame_exitTitle");
        exitMessage = LocalizationManager.Instance.GetLocalizedText("inGame_exitMessage");
    }

    protected override void PrevClose() { }

    public void SetView() {
        txt_bestcount.text = UserManager.Instance.CurrentUserRecord.score.ToString();
        RefreshCount();
        CloseFailPopup();
    }

    public void RefreshCount() {
        txt_currentcount.text = GameManager.Instance.JumpCount.ToString();
        txt_currentcount.transform.DOPunchScale(Vector3.one * 2f, 0.5f).SetEase(Ease.OutFlash);
        txt_combocount.text = GameManager.Instance.ComboCount.ToString();
        txt_combocount.transform.DOPunchScale(Vector3.one * 2f, 0.5f).SetEase(Ease.OutFlash);
    }

    private void Update() {
        if (IsScreenBtnDown) {
            elapsedTime += Time.deltaTime;
            GameManager.Instance.Player.UpdateGauge(elapsedTime);
        }
    }

    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_back):
                ExitGame();
                break;
            case nameof(btn_screen):
                IsScreenBtnDown = false;
                if (!CheckJumpable()) return;
                GameManager.Instance.Player.Jump(elapsedTime);
                elapsedTime = 0;
                break;
            case nameof(btn_restart):
                CloseFailPopup();
                elapsedTime = 0;
                GameManager.Instance.GameStart();
                SetView();
                break;
        }
    }

    private void ExitGame()
    {
        UIManager.Instance.Show<MessageBoxUI>(ui =>
        {
                ui.SetMessage(exitMessage, exitTitle, () =>
            {
                SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY, true).Forget();
            }, null);
        });
    }

    public void OpenFailPopup() {
        failPopup.SetActive(true);
        txt_endscore.text = $"{score} : {GameManager.Instance.JumpCount}";
    }

    private void CloseFailPopup() {
        failPopup.SetActive(false);
    }

    private bool CheckJumpable()
    {
        return !GameManager.Instance.Player.IsJumping && GameManager.Instance.IsPlaying;
    }

    public void OnScreenButtonDown() {
        if (!CheckJumpable()) return;
        IsScreenBtnDown = true;
        GameManager.Instance.Player.ChangeState(Player.PLAYER_STATE.CROUCH);
    }
}
