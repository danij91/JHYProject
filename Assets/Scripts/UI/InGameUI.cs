using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InGameUI : UIBase
{
    [SerializeField] private Button btn_back;
    [SerializeField] private Button btn_screen;
    [SerializeField] private Text txt_currentcount;
    [SerializeField] private Text txt_bestcount;


    private float elapsedTime;
    public bool IsScreenBtnDown { get; set; }

    protected override void PrevOpen(params object[] args)
    {
        SetView();
    }

    protected override void PrevClose()
    {
    }

    public void SetView()
    {
        txt_bestcount.text = LocalDataHelper.GetBestCount().ToString();
        RefreshCount();
    }

    public void RefreshCount()
    {
        txt_currentcount.text = GameManager.Instance.JumpCount.ToString();
        txt_currentcount.transform.DOPunchScale(Vector3.one * 2f, 0.5f).SetEase(Ease.InSine);
    }

    private void Update()
    {
        if (IsScreenBtnDown)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public override void OnButtonEvent(Button inButton)
    {
        switch (inButton.name)
        {
            case nameof(btn_back):
                LocalDataHelper.SaveBestCount(GameManager.Instance.JumpCount);
                SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY, true);
                break;
            case nameof(btn_screen):
                GameManager.Instance.Player.Jump(elapsedTime);
                elapsedTime = 0;
                break;
        }
    }
}
