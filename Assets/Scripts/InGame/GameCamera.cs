using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    private Player player => GameManager.Instance.Player;
    private Vector3 differPos;
    private readonly Vector3 INITIAL_CAMERA_POSITION = new Vector3(0, 10, -10);

    public void Initialize() {
        transform.position = INITIAL_CAMERA_POSITION;
        differPos = transform.position - player.transform.position;
    }

    private void Update() {
        if (player != null && GameManager.Instance.IsJumping) {
            transform.position = (player.transform.position + differPos).SetY(10);
        }
    }
}
