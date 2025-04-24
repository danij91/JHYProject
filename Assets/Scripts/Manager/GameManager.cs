using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameCamera gameCamera;
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private CharacterDatabase characterDatabase;

    public enum GAME_STATE {
        READY,
        PLAY,
        END
    }

    public GAME_STATE CurrentState { get; private set; }
    public GameCamera GameGameCamera => gameCamera;
    public PlayerController PlayerController { get; private set; }
    public int JumpCount { get; private set; }         // 점프한 횟수
    public int Score { get; private set; }    
    public int ComboCount { get; private set; }
    public int TotalComboCount { get; private set; }
    public int MaxComboCount { get; private set; }

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
        TotalComboCount = 0;
        ComboCount = 0;
        Score = 0;
        PoolingManager.Instance.RestoreAll();
        MapManager.Instance.Initialize();
        CreatePlayer();
        GameGameCamera.Initialize();
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

        if (ComboCount != 0)
        {
            TotalComboCount += ComboCount;
            MaxComboCount = Mathf.Max(MaxComboCount, ComboCount);
            ComboCount = 0;
        }
        
        var currentUser = UserManager.Instance.CurrentUserData;
        currentUser.totalPlayCount++;
        currentUser.totalJump += JumpCount;
        currentUser.totalCombo += TotalComboCount;
        currentUser.totalScore += Score;
        currentUser.maxJump = Mathf.Max(currentUser.maxJump, JumpCount);
        currentUser.maxCombo = Mathf.Max(currentUser.maxCombo, MaxComboCount);
        currentUser.maxScore = Mathf.Max(currentUser.maxScore, Score);
        
        ReportRecord();
        
        if (playCount >= AD_PLAY_COUNT) {
            AdManager.Instance.LoadPlayAds();
            playCount = 0;
        }
    }


    private void ReportRecord()
    {
        MissionManager.Instance.ReportProgressAccumulate(MissionConditionType.JumpCount,JumpCount);
        MissionManager.Instance.ReportProgressAccumulate(MissionConditionType.ComboCount,TotalComboCount);
        MissionManager.Instance.ReportProgressAccumulate(MissionConditionType.ScoreReach,Score);
        MissionManager.Instance.ReportProgressAccumulate(MissionConditionType.PlayCount,1);
    }

    public void SaveBestScore() {
        int prevScore = UserManager.Instance.CurrentUserRecord?.score ?? 0;
        if (Score > prevScore) {
            UserManager.Instance.UpdateScore(Score);
        }
    }

    public void OnFail() {
        GameEnd();
        inGameUI.OpenFailPopup();
        PlayerController.ChangeState(PlayerController.PLAYER_STATE.FALL);
    }

    public void OnSuccess() {
        JumpCount++;
        Score += 1 + ComboCount;
        inGameUI.RefreshCount();
        MapManager.Instance.CreateMap();
        MapManager.Instance.RemoveMap();
        PlayerController.SetRotation();
    }

    public void SuccessCombo() {
        Debug.Log("success");
        IsPerfectJump = true;
        ComboCount++;
    }

    public void FailCombo() {
        Debug.Log("fail");
        IsPerfectJump = false;
        TotalComboCount += ComboCount;
        MaxComboCount = Mathf.Max(MaxComboCount, ComboCount);
        ComboCount = 0;
        
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            UserManager.Instance.RefreshEnergy();
        }
    }
}
