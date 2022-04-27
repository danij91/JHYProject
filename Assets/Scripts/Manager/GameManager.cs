using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameCamera camera;

    public GameCamera GameCamera => camera;
    public Player Player { get; private set; }

    public void Initialize()
    {
        UIManager.Instance.Show<InGameUI>();
        Player = PoolingManager.Instance.Create<Player>(EPoolingType.Character, "Player");
        Player.transform.position = MapManager.Instance.StartPos;
    }

    public void OnPlayerJump()
    {
        GameCamera.transform.position += Player.CurrentTargetDistance;
        MapManager.Instance.CreateMap();
        MapManager.Instance.RemoveMap();
    }
}
