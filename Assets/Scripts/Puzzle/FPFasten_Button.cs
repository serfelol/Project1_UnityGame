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
    //axe checks
    private bool axeOnButton = false;
    private bool axeEntered = false;
    private Transform axeReference;
    // player checks
    private bool playerOnButton = false;

    private void Update()
    {
        if (axeEntered)
        {
            // axe only becomes a child some frames after the triggerEnter
            axeReference = transform.parent.transform.Find("AxeParent");
            if(axeReference != null)
            {
                axeReference = transform.parent.Find("AxeParent");
                axeEntered = false;
                axeOnButton = true;
                FPScript.FastenPlatformButton = true;
                Debug.Log(axeReference.name);
            }
        }

        if (axeOnButton && axeReference == null)
        {
            axeOnButton = false;

            if (!playerOnButton) FPScript.FastenPlatformButton = false;
        }
    }
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
            playerOnButton = true;
        }
        else if (other.transform.CompareTag("Axe"))
        {
            axeEntered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.transform.CompareTag("Player") && axeOnButton == false)
        {
            FPScript.FastenPlatformButton = false;
            playerOnButton = false;
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
