using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UIBase
{
    [SerializeField] private Button btn_touch;
    [SerializeField] private GameObject tmp_touch;

    private float elapsedTime;

    protected override void PrevOpen(params object[] args)
    {
        elapsedTime = 0f;
    }

    protected override void PrevClose()
    {
    }

    public override void OnButtonEvent(Button inButton)
    {
        switch (inButton.name)
        {
            case nameof(btn_touch):
                SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY).Forget();
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
