
public class LobbyScene : SceneBase
{
    public override void OnStart()
    {
        if (TryLoadTitleScene()) return;

        base.OnStart();
        AudioManager.Instance.BGMPlay(BGMType.Lobby);
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
