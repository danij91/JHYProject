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
    [SerializeField] private TMP_Text txt_bestcount;
    [SerializeField] private TMP_Text txt_combocount;
    [SerializeField] private TMP_Text txt_endscore;
    
    
    private string score;
    private string exitTitle;
    private string exitMessage;

    private float elapsedTime;
    public bool IsScreenBtnDown { get; set; }

    protected override void PrevOpen(params object[] args)
    {
        SetView();
        btn_back.AddOnClickListener(ExitGame);
        btn_screen.AddOnClickListener(OnClickScreen);
        btn_restart.AddOnClickListener(OnClickRestart);
        
        score = LocalizationManager.Instance.GetLocalizedText("inGame_score");
        exitTitle = LocalizationManager.Instance.GetLocalizedText("inGame_exitTitle");
        exitMessage = LocalizationManager.Instance.GetLocalizedText("inGame_exitMessage");
    }

    protected override void PrevClose()
    {
    }

    public void SetView()
    {
        txt_bestcount.text = UserManager.Instance.CurrentUserRecord != null
            ? UserManager.Instance.CurrentUserRecord.score.ToString()
            : "0";
        RefreshCount();
        CloseFailPopup();
    }

    public void RefreshCount()
    {
        txt_currentcount.text = GameManager.Instance.Score.ToString();
        txt_currentcount.transform.DOPunchScale(Vector3.one * 2f, 0.5f).SetEase(Ease.OutFlash);
        txt_combocount.text = GameManager.Instance.ComboCount.ToString();
        txt_combocount.transform.DOPunchScale(Vector3.one * 2f, 0.5f).SetEase(Ease.OutFlash);
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
        failPopup.SetActive(true);
        txt_endscore.text = $"{score} : {GameManager.Instance.JumpCount}";
    }

    private void CloseFailPopup()
    {
        failPopup.SetActive(false);
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