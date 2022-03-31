using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// makes all platforms activate on the room
// ----------------- marcado para sair
public class ButtonUnlockPlatforms : MonoBehaviour
{
    public Puzzle1Manager manager;
    private bool wasButtonPressed;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.CompareTag("Player")){
            OnButtonPressed();
        }
    }

    private void OnButtonPressed(){
        if( wasButtonPressed == false ){
            wasButtonPressed = true;
            manager.UnlockPlatformsFromGround();
            StartCoroutine(ButtonTimer());
        }
    }

    private IEnumerator ButtonTimer(){
        yield return new WaitForSeconds( 2.0f );
        wasButtonPressed = false;
    }
}
