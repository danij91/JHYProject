
public class LobbyScene : SceneBase
{
    public override void OnStart()
    {
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
