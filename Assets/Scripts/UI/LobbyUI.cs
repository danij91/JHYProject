using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : UIBase
{
    [SerializeField] private Button btn_gameStart;
    [SerializeField] private Image img_bg;
    [SerializeField] private GameObject tmp_touch;

    private float elapsedTime;

    protected override void PrevOpen(params object[] args)
    {
        elapsedTime = 0f;
        SetBGResolution();
    }

    protected override void PrevClose()
    {
    }

    public void SetBGResolution()
    {
        RectTransform rectTr = GetComponent<RectTransform>();
        float screenAspectRatio = (float)Screen.height / (float)Screen.width;
        float defaultAspectRatio = EConfig.System.DEFAULT_CANVAS_HEIGHT / EConfig.System.DEFAULT_CANVAS_WIDTH;
        float bgRatio = screenAspectRatio < defaultAspectRatio ? rectTr.sizeDelta.y / img_bg.rectTransform.sizeDelta.x
                                                                : rectTr.sizeDelta.x / img_bg.rectTransform.sizeDelta.y;
                                                    
        img_bg.rectTransform.sizeDelta *= bgRatio;
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

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float ms = elapsedTime % 1;
        if (ms < 0.66f)
            tmp_touch.SetActive(true);
        else
            tmp_touch.SetActive(false);

    }
}
