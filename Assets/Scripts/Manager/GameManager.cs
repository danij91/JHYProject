using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GameObject player;
    public Transform startTr;
    public void Initialize(Transform inStartTr)
    {
        startTr = inStartTr;
        var playerPrefab = ResourceManager.Instance.Load<GameObject>("Prefabs/Character/Player");
        player = Instantiate(playerPrefab, startTr);
    }
}
