
public class LobbyScene : SceneBase
{
    public override void OnStart()
    {
        if (TryLoadTitleScene()) return;

        base.OnStart();
        UIManager.Instance.Show<LobbyUI>();
    }

    public override void OnClear()
    {
        base.OnClear();
    }

    public override void UpdateScene()
    {
    }
}
