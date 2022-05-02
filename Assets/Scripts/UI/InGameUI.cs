using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InGameUI : UIBase
{
    [SerializeField] private Button btn_back;
    [SerializeField] private Button btn_screen;
    [SerializeField] private Text txt_count;

    private float elapsedTime;
    public bool IsScreenBtnDown { get; set; }

    protected override void PrevOpen(params object[] args)
    {
        RefreshCount();
    }

    protected override void PrevClose()
    {
    }

    public void RefreshCount()
    {
        txt_count.text = GameManager.Instance.JumpCount.ToString();
        txt_count.transform.DOPunchScale(Vector3.one * 2f, 0.5f).SetEase(Ease.InSine);
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
                SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY, true);
                break;
            case nameof(btn_screen):
                GameManager.Instance.Player.Jump(elapsedTime);
                elapsedTime = 0;
                break;
        }
    }
}
