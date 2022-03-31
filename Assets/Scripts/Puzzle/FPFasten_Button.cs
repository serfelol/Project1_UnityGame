using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// button that fasten the oscillating platform.
/// triggered by: player.
/// </summary>
public class FPFasten_Button : Button_PressAndRelease
{
    public FloatingPlatform FPScript;

    private void OnEnable() {
        FPScript.platformGroundCollisionEvent += SetButtonToPressed;
        FPUnfasten_Button.UnfastenFloatingPlatformsEvent += SetButtonToUnpressed;
    }
    private void OnDisable() {
        FPScript.platformGroundCollisionEvent -= SetButtonToPressed;
        FPUnfasten_Button.UnfastenFloatingPlatformsEvent -= SetButtonToUnpressed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.CompareTag("Player")){
            FPScript.FastenPlatformButton = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.transform.CompareTag("Player")){
            FPScript.FastenPlatformButton = false;
        }
    }

    // button only work if platform is at the bottom.
    private void SetButtonToPressed(){
        if(!wasButtonActivated){
            PhysicallyPressTheButton();
            wasButtonActivated = true;
        }
    }

    private void SetButtonToUnpressed(){
        if(wasButtonActivated){
            PhysicallyReleaseTheButton();
            wasButtonActivated = false;
        }
    }
}
