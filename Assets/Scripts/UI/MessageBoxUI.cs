using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MessageBoxUI : UIBase
{
    [SerializeField] private Button btn_confirm;
    [SerializeField] private Button btn_cancel;
    [SerializeField] private TextMeshProUGUI tmp_title;
    [SerializeField] private TextMeshProUGUI tmp_message;

    private Action ConfirmFunc;
    private Action CancelFunc;


    protected override void PrevOpen(params object[] args)
    {
        SetCanvasOrderInLayer(1000, true);
    }

    protected override void PrevClose()
    {
    }

    public void SetMessage(string message, string title, Action inConfirmFunc, Action inCancelFunc, bool isOneButton = false)
    {
        tmp_message.text = message;
        tmp_title.text = title.IsNullOrEmpty() ? "NOTICE" : title.ToUpper();

        ConfirmFunc = inConfirmFunc;
        CancelFunc = inCancelFunc;
        btn_cancel.gameObject.SetActive(!isOneButton);
    }

    public override void OnButtonEvent(Button inButton)
    {
        switch (inButton.name)
        {
            case nameof(btn_confirm):
                ConfirmFunc?.Invoke();
                Close();
                break;
            case nameof(btn_cancel):
                CancelFunc?.Invoke();
                Close();
                break;
        }
    }

}
