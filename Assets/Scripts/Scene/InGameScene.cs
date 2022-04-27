
using UnityEngine;

public class InGameScene : SceneBase
{
    public override void OnStart()
    {
        base.OnStart();
        MapManager.Instance.Initialize();
        GameManager.Instance.Initialize();
    }

    public override void OnClear()
    {
        base.OnClear();
    }

    public override void UpdateScene()
    {
    }
}
