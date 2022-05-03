using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    [SerializeField]
    private GameCamera camera;

    public enum GAME_STATE {
        READY,
        PLAY,
        END
    }

    public GAME_STATE CurrentState { get; set; }
    public GameCamera GameCamera => camera;
    public Player Player { get; private set; }
    public int JumpCount { get; private set; }
    public bool isPlaying => CurrentState == GAME_STATE.PLAY;

    private InGameUI inGameUI;

    private bool isJumping;

    public bool IsJumping {
        get => CurrentState == GAME_STATE.PLAY && isJumping;

        private set => isJumping = value;
    }

    public void Initialize() {
        UIManager.Instance.Show<InGameUI>();
        if(inGameUI == null)
            inGameUI = UIManager.Instance.GetUI<InGameUI>();
        GameStart();
    }

    public void GameStart() {
        JumpCount = 0;
        PoolingManager.Instance.RestoreAll();
        MapManager.Instance.Initialize();
        Player = PoolingManager.Instance.Create<Player>(EPoolingType.Character, "Player");
        GameCamera.Initialize();
        CurrentState = GAME_STATE.PLAY;
    }

    public void GameEnd() {
        CurrentState = GAME_STATE.END;
        SaveBestScore();
    }

    public void SaveBestScore() {
        int prevCount = LocalDataHelper.GetBestCount();
        if (JumpCount > prevCount)
            LocalDataHelper.SaveBestCount(JumpCount);
    }

    public void OnPlayerJump() {
        IsJumping = true;
    }

    public void OnPlayerJumpDone() {
        IsJumping = false;
    }

    public void OnFail() {
        GameEnd();
        inGameUI.OpenFailPopup();
    }

    public void OnSuccess() {
        JumpCount++;
        inGameUI.RefreshCount();
        MapManager.Instance.CreateMap();
        MapManager.Instance.RemoveMap();
    }
}
