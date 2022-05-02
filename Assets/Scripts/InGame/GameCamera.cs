using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    private Player player => GameManager.Instance.Player;
    private Vector3 differPos;

    public void Initialize()
    {
        differPos = transform.position - player.transform.position;
    }

    private void Update()
    {
        if(player != null)
        {
            transform.position = player.transform.position + differPos;
        }
        
    }

}
