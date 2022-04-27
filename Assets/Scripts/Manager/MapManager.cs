using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private Transform startTr;

    public Vector3 StartPos => startTr.position;

    public void Initialize()
    {
        CreateMap();
    }

    private void CreateMap()
    {

    }
}
