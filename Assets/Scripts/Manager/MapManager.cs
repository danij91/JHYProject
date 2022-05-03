using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EDirection {
    Right = 0,
    Left = 1,
}

public class MapManager : Singleton<MapManager> {
    [SerializeField]
    private Transform startTr;
    public Map CurrentMap { get; private set; }

    private EDirection lastDirection;
    private Queue<Map> mapQueue = new Queue<Map>();
    private FailChecker failChecker;

    public Vector3 StartPos => startTr.position;

    public void Initialize() {
        mapQueue?.Clear();
        failChecker = PoolingManager.Instance.Create<FailChecker>(EPoolingType.Map, "FailChecker");
        CreateMap(true);
        CreateMap();
    }

    public Map CreateMap(bool isInit = false) {
        string mapName = GetMapType().ToString();
        Map map = PoolingManager.Instance.Create<Map>(EPoolingType.Map, mapName);

        if (isInit) {
            map.transform.position = Vector3.zero;
            CurrentMap = map;
        }
        else {
            Vector3 nextPos = GetNextPosition();
            map.transform.position = nextPos;
            CurrentMap = map;
            failChecker.transform.position = nextPos.SetY(nextPos.y - 1);
        }

        mapQueue.Enqueue(map);
        return map;
    }

    private EMapType GetMapType() {
        return (EMapType) UnityEngine.Random.Range(0, Enum.GetValues(typeof(EMapType)).Length);
    }

    private Vector3 GetNextPosition() {
        var dist = UnityEngine.Random.Range(EConfig.Map.MIN_DISTANCE, EConfig.Map.MAX_DISTANCE);
        lastDirection = (EDirection) UnityEngine.Random.Range(0, 2);

        var nextPos = CurrentMap.transform.position + GetLastDirection() * dist;
        return nextPos;
    }

    public Vector3 GetLastDirection() {
        return lastDirection == EDirection.Right ? EConfig.System.RIGHT : EConfig.System.LEFT;
    }

    public void RemoveMap() {
        if (mapQueue.Count < 4)
            return;
        Map oldMap = mapQueue.Dequeue();
        oldMap.Restore();
    }
}
