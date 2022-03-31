using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if something passes through the trigger area
/// and returns the object to the respawn point.
/// </summary>
public class RespawnAfterFall : MonoBehaviour
{
    private Vector2 respawnPosition;
    public BoxCollider2D bc2d;

    private void Awake() {
        respawnPosition = this.transform.GetChild(1).transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            other.GetComponent<PlayerControllerBehaviour>().RespawnPlayer(respawnPosition);
        }
    }
}
