using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    private PlayerController PlayerController => GameManager.Instance.PlayerController;
    private Vector3 differPos;
    private readonly float CAMERA_Y_POS = 13f;

    public void Initialize() {
        transform.position = new Vector3(0, CAMERA_Y_POS, -10); ;
        differPos = transform.position - PlayerController.transform.position;
    }

    private void Update() {
        if (PlayerController != null && PlayerController.IsJumping) {
            transform.position = (PlayerController.transform.position + differPos).SetY(CAMERA_Y_POS);
        }
    }
}
