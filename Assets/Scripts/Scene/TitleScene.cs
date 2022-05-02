
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : SceneBase
{
    public static bool IS_TITLESCENE_LOADED = false;

    protected override async void Start()
    {
        // ToDo Initialize 할 거 여기서 하면 됨 (보통은 AppStarter 만들어서 처리함)
        SceneLoader.Instance.Initalize();
        UIManager.Instance.Initialize();
        LocalData.Instance.Initialize();
        UIManager.Instance.Show<LoginUI>();
        IS_TITLESCENE_LOADED = true;
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void OnClear()
    {
        base.OnClear();
    }
}
