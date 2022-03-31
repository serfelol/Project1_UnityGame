using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// function as a trigger for something to happen in the game.
public class Lever : MonoBehaviour
{
    #region Fields
    private bool leverState = false;
    public RoomsManager roomsManager;
    #endregion

    private void Start() {
        leverState = false;
    }

    private void Update(){

    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Axe")){
            leverState = !leverState;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.3f, 0);
            roomsManager.Room2_LeverAction(leverState);
        }            
    }
}
