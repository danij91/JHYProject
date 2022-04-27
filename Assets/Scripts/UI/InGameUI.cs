using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : UIBase
{
    [SerializeField] private Button btn_back;

    protected override void PrevOpen(params object[] args)
    {
    }

    protected override void PrevClose()
    {
    }

    public override void OnButtonEvent(Button inButton)
    {
        switch (inButton.name)
        {
            case nameof(btn_back):
                SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY, true);
                break;
        }
    }
}
