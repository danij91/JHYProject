using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameCamera camera;

    public GameCamera GameCamera => camera;
    public Player Player { get; private set; }
    public int JumpCount { get; private set; }

    private InGameUI inGameUI;

    public void Initialize()
    {
        JumpCount = 0;
        UIManager.Instance.Show<InGameUI>();
        inGameUI = UIManager.Instance.GetUI<InGameUI>();
        Player = PoolingManager.Instance.Create<Player>(EPoolingType.Character, "Player");
        GameCamera.Initialize();
    }

    public void OnPlayerJump()
    {
        JumpCount++;
        inGameUI.RefreshCount();
        MapManager.Instance.CreateMap();
        MapManager.Instance.RemoveMap();
    }
}
