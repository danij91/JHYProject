using UnityEngine;

public class InGameScene : SceneBase {
    public override void OnStart() {
        if (TryLoadTitleScene()) return;

        base.OnStart();
        AudioManager.Instance.BGMPlay(BGMType.InGame);
        GameManager.Instance.Initialize();
    }

    public override void OnClear() {
        GameManager.Instance.GameEnd();
        base.OnClear();
    }

    public override void UpdateScene() { }
}
