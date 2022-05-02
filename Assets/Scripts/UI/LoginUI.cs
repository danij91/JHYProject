using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoginUI : UIBase
{
    [SerializeField] private Button btn_login;


    protected override void PrevOpen(params object[] args)
    {
    }

    private void SetView()
    {

    }

    protected override void PrevClose()
    {
    }

    public override void OnButtonEvent(Button inButton)
    {
        switch (inButton.name)
        {
            case nameof(btn_login):
                SceneLoader.Instance.ChangeScene(EScene.LOBBY);
                break;
        }
    }
}
