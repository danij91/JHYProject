using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private GameCamera camera;

    public enum GAME_STATE {
        READY,
        PLAY,
        END
    }

    public GAME_STATE CurrentState { get; set; }
    public GameCamera GameCamera => camera;
    public Player Player { get; private set; }
    public int JumpCount { get; private set; }
    public int ComboCount { get; private set; }
    public bool IsPerfectJump { get; private set; }
    public bool IsPlaying => CurrentState == GAME_STATE.PLAY;
    public int playCount;

    private InGameUI inGameUI;
    private const int AD_PLAY_COUNT = 3;

    public void Initialize() {
        GameStart();
        UIManager.Instance.Show<InGameUI>();
        if (inGameUI == null)
            inGameUI = UIManager.Instance.GetUI<InGameUI>();
    }

    public void GameStart() {
        JumpCount = 0;
        PoolingManager.Instance.RestoreAll();
        MapManager.Instance.Initialize();
        CreatePlayer();
        GameCamera.Initialize();
        CurrentState = GAME_STATE.PLAY;
        IsPerfectJump = false;
    }

    private void CreatePlayer() {
        ECharacterType type;
        if (LocalDataConfig.Instance.IsCharacterTest) {
            type = LocalDataConfig.Instance.StartCharacterType;
        } else {
            type = CharacterInventory.Instance.MainCharacter;
        }

        Player = PoolingManager.Instance.Create<Player>(EPoolingType.Character, $"Player_{type}", null, type);
    }

    public void GameEnd() {
        CurrentState = GAME_STATE.END;
        SaveBestScore();
        AudioManager.Instance.AllSFXStop();

        playCount++;
        
        if (playCount >= AD_PLAY_COUNT) {
            AdManager.Instance.LoadPlayAds();
            playCount = 0;
        }
    }

    public void SaveBestScore() {
        int prevCount = DataManager.Instance.CurrentUserRecord.score;
        if (JumpCount > prevCount) {
            DataManager.Instance.UpdateScore(JumpCount);
        }
    }

    public void OnFail() {
        GameEnd();
        inGameUI.OpenFailPopup();
        Player.ChangeState(Player.PLAYER_STATE.FALL);
    }

    public void OnSuccess() {
        JumpCount++;
        JumpCount += ComboCount;
        inGameUI.RefreshCount();
        MapManager.Instance.CreateMap();
        MapManager.Instance.RemoveMap();
        Player.SetRotation();
    }

    public void SuccessCombo() {
        IsPerfectJump = true;
        ComboCount++;
    }

    public void FailCombo() {
        IsPerfectJump = false;
        ComboCount = 0;
    }
}
