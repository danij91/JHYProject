using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDirection 
{
    Right = 0,
    Left = 1,
}

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private Transform startTr;
    private Vector3 currentMapPos;
    private EDirection lastDirection;
    private readonly Vector3 RIGHT = new Vector3(1,0,1);
    private readonly Vector3 LEFT = new Vector3(-1, 0, 1);
    private readonly float MIN_DISTANCE = 2;
    private readonly float MAX_DISTANCE = 3f;
    private Queue<CubeMap> mapQueue = new Queue<CubeMap>();

    public Vector3 StartPos => startTr.position;

    public void Initialize()
    {
        mapQueue?.Clear();
        CreateMap(true);
        CreateMap();
    }

    public CubeMap CreateMap(bool isInit = false)
    {
        CubeMap map = PoolingManager.Instance.Create<CubeMap>(EPoolingType.Map, "CubeMap");
        mapQueue.Enqueue(map);

        if (isInit)
        {
            map.transform.position = Vector3.zero;
            currentMapPos = map.transform.position;
        }
        else
        {

            Vector3 nextPos = GetNextPosition();
            map.transform.position = nextPos;
            currentMapPos = nextPos;
        }

        return map;
    }

    private Vector3 GetNextPosition()
    {
        var dist = Random.Range(MIN_DISTANCE, MAX_DISTANCE);
        lastDirection = (EDirection)Random.Range(0, 2);

        var nextPos = currentMapPos + GetLastDirection() * dist;
        return nextPos;
    }

    public Vector3 GetLastDirection()
    {
        return lastDirection == EDirection.Right ? RIGHT : LEFT;
    }

    public void RemoveMap()
    {
        if (mapQueue.Count < 4)
            return;
        CubeMap oldMap = mapQueue.Dequeue();
        oldMap.Restore();
    }

}
