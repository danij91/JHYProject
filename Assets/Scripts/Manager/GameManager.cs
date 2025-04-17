using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameCamera camera;
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private CharacterDatabase characterDatabase;

    public enum GAME_STATE {
        READY,
        PLAY,
        END
    }

    public GAME_STATE CurrentState { get; private set; }
    public GameCamera GameCamera => camera;
    public PlayerController PlayerController { get; private set; }
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
        ComboCount = 0;
        PoolingManager.Instance.RestoreAll();
        MapManager.Instance.Initialize();
        CreatePlayer();
        GameCamera.Initialize();
        CurrentState = GAME_STATE.PLAY;
        IsPerfectJump = false;
    }

    private void CreatePlayer() {
        CharacterData data;

        if (LocalDataConfig.Instance.IsCharacterTest) {
            data = characterDatabase.GetCharacterDataById(LocalDataConfig.Instance.StartCharacterType.ToString().ToLower());
        } else {
            data = CharacterInventory.Instance.GetSelectedCharacterData();
        }

        PlayerController = PoolingManager.Instance.Create<PlayerController>(
            EPoolingType.Character, "Player", null, data);
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
        int prevCount = UserManager.Instance.CurrentUserRecord?.score ?? 0;
        if (JumpCount > prevCount) {
            UserManager.Instance.UpdateScore(JumpCount);
        }
    }

    public void OnFail() {
        GameEnd();
        inGameUI.OpenFailPopup();
        PlayerController.ChangeState(PlayerController.PLAYER_STATE.FALL);
    }

    public void OnSuccess() {
        JumpCount++;
        JumpCount += ComboCount;
        inGameUI.RefreshCount();
        MapManager.Instance.CreateMap();
        MapManager.Instance.RemoveMap();
        PlayerController.SetRotation(); // 🔄 바뀐 부분
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
