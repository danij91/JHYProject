using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : UIBase
{
    [SerializeField] private Button btn_gameStart;

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
            case nameof(btn_gameStart):
                SceneLoader.Instance.ChangeSceneAsync(EScene.INGAME, true).Forget();
                break;
        }
    }
}
