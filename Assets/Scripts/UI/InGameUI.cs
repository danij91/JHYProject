using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class InGameUI : UIBase
{
    [SerializeField] private Button btn_back;
    [SerializeField] private Button btn_screen;
    [SerializeField] private Button btn_restart;
    [SerializeField] private GameObject failPopup;
    [SerializeField] private TMP_Text txt_currentcount;
    [SerializeField] private TMP_Text txt_combocount;
    [SerializeField] private TMP_Text txt_endscore;
    [SerializeField] private TMP_Text txt_bestscore;
    [SerializeField] private CanvasGroup canvasGroup_combo;

    private Tween currentTween;
    private string score;
    private string bestScore;
    private string exitTitle;
    private string exitMessage;

    private float elapsedTime;
    private float tweenDuration = 2.5f;
    private float scoreTweenScale = 1.5f;
    private float comboTweenScale = 1.5f;

    public bool IsScreenBtnDown { get; set; }

    protected override void PrevOpen(params object[] args)
    {
        SetView();
        btn_back.AddOnClickListener(ExitGame);
        btn_screen.AddOnClickListener(OnClickScreen);
        btn_restart.AddOnClickListener(OnClickRestart);

        score = LocalizationManager.Instance.GetLocalizedText("inGame_score");
        bestScore = LocalizationManager.Instance.GetLocalizedText("inGame_bestScore");
        exitTitle = LocalizationManager.Instance.GetLocalizedText("inGame_exitTitle");
        exitMessage = LocalizationManager.Instance.GetLocalizedText("inGame_exitMessage");
    }

    protected override void PrevClose()
    {
    }

    public void SetView()
    {
        RefreshCount();
        CloseFailPopup();
    }

    public void RefreshCount()
    {
        txt_currentcount.text = GameManager.Instance.Score.ToString();
        if (GameManager.Instance.Score != 0)
        {
            txt_currentcount.transform.DOPunchScale(Vector3.one * scoreTweenScale, 0.5f).SetEase(Ease.OutFlash);
        }

        txt_combocount.text = GameManager.Instance.ComboCount.ToString() + " Combos";
        currentTween?.Kill();
        canvasGroup_combo.alpha = 1f;
        currentTween = canvasGroup_combo.DOFade(0, tweenDuration).SetEase(Ease.OutQuad);
        // txt_combocount.transform.DOPunchScale(Vector3.one * tweenSize, 0.5f).SetEase(Ease.OutFlash);
    }

    private void Update()
    {
        if (IsScreenBtnDown)
        {
            elapsedTime += Time.deltaTime;
            GameManager.Instance.PlayerController.UpdateGauge(elapsedTime);
        }
    }

    private void ExitGame()
    {
        UIManager.Instance.Show<MessageBoxUI>(ui =>
        {
            ui.SetMessage(exitMessage, exitTitle,
                () => { SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY, true).Forget(); }, null);
        });
    }

    public void OpenFailPopup()
    {
        HidePlayScore();
        failPopup.SetActive(true);
        txt_endscore.text = $"{score} : {GameManager.Instance.JumpCount}";

        string userScore = UserManager.Instance.CurrentUserRecord != null
            ? UserManager.Instance.CurrentUserRecord.score.ToString()
            : "0";

        txt_bestscore.text = $"{bestScore} : {userScore}";
    }

    private void HidePlayScore()
    {
        txt_combocount.gameObject.SetActive(false);
        txt_currentcount.gameObject.SetActive(false);
    }

    private void ShowPlayScore()
    {
        txt_combocount.gameObject.SetActive(true);
        txt_currentcount.gameObject.SetActive(true);
    }

    private void CloseFailPopup()
    {
        failPopup.SetActive(false);
        ShowPlayScore();
    }

    private bool CheckJumpable()
    {
        return !GameManager.Instance.PlayerController.IsJumping && GameManager.Instance.IsPlaying;
    }

    public void OnScreenButtonDown()
    {
        if (!CheckJumpable()) return;
        IsScreenBtnDown = true;
        GameManager.Instance.PlayerController.ChangeState(PlayerController.PLAYER_STATE.CROUCH);
    }

    private void OnClickScreen()
    {
        IsScreenBtnDown = false;

        if (!CheckJumpable()) return;
        GameManager.Instance.PlayerController.Jump(elapsedTime);
        elapsedTime = 0;
    }

    private void OnClickRestart()
    {
        UserManager.Instance.RefreshEnergy();

        if (UserManager.Instance.TryConsumeEnergy())
        {
            CloseFailPopup();
            elapsedTime = 0;
            GameManager.Instance.GameStart();
            SetView();
        }
        else
        {
            Debug.Log("Out of energy");
        }
    }
}