using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : UIBase
{
    [SerializeField] private Button btn_back;
    [SerializeField] private Button btn_screen;
    private float elapsedTime;
    public bool IsScreenBtnDown { get; set; }

    protected override void PrevOpen(params object[] args)
    {
    }

    protected override void PrevClose()
    {
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
