
using UnityEngine;

public class InGameScene : SceneBase
{
    [SerializeField]
    private Transform startTr;

    public override void OnStart()
    {
        base.OnStart();
        GameManager.Instance.Initialize(startTr);

    }

    public override void OnClear()
    {
        base.OnClear();
    }

    public override void UpdateScene()
    {
    }
}
