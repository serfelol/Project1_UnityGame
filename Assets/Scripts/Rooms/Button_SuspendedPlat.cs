using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_SuspendedPlat : MonoBehaviour
{
    public float insertedDistance;
    public bool isBeingPressed{set; get; }
    public bool wasButtonActivated{set; get;}

    private void Start() {
        wasButtonActivated = false;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.CompareTag("Player")){
            isBeingPressed = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.transform.CompareTag("Player")){
            isBeingPressed = false;
        }
    }

    public void ActivateButton(){
        if(wasButtonActivated == false){
            transform.GetChild(0).transform.position = new Vector2(transform.position.x, transform.position.y - insertedDistance);
            wasButtonActivated = true;
        }
    }

    public void DeactivateButton(){
        if(wasButtonActivated == true){
            transform.GetChild(0).transform.position = new Vector2(transform.position.x, transform.position.y);                  
            wasButtonActivated = false;            
        }
    }
}
