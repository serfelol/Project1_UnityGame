using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchRoom_Trigger : MonoBehaviour
{
    [SerializeField] private RoomsManager roomsMangr;
    public Room roomToEnter;

    private void Awake() {
        if(!roomsMangr){
            Debug.Log("RoomsManager not found!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.CompareTag("Player")){
            roomsMangr.ChangeRoom(roomToEnter);
        }
    }
}
