using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    private Player player => GameManager.Instance.Player;
    private Vector3 differPos;
    private readonly float CAMERA_Y_POS = 13f;

    public void Initialize() {
        transform.position = new Vector3(0, CAMERA_Y_POS, -10); ;
        differPos = transform.position - player.transform.position;
    }

    private void Update() {
        if (player != null && player.IsJumping) {
            transform.position = (player.transform.position + differPos).SetY(CAMERA_Y_POS);
        }
    }
}
