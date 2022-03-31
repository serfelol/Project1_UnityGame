using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// button that triggers the blocker.
/// can be pressed by: platform.
/// </summary>
public class BlockerActivator_Button : Button_PressAndRelease
{
    public ElevatorBlocker blockerReference;
    // set ElevatorBlocker button reference to true on trigger enter.
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Platform")){
            blockerReference.BlockerButtonState = true;
            PhysicallyPressTheButton();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Platform")){
            blockerReference.BlockerButtonState = false;
            PhysicallyReleaseTheButton();
        }
    }

}
