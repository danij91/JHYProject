
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : SceneBase
{
    protected override async void Start()
    {
        // Initialize할거 여기서 하면 됨
        UIManager.Instance.Initialize();
        SceneLoader.Instance.Initalize();
        UIManager.Instance.Show<LoginUI>();
        //await SceneLoader.Instance.ChangeSceneAsync(EScene.LOBBY, true);
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
