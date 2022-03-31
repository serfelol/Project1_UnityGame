using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// button that provides force to lift the elevator.
/// triggered by: floating platform.
/// </summary>
public class ElevatorForceProvider_Button : Button_PressAndRelease
{
    public Elevator elevatorScript;
    public float forceToApply;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.CompareTag("Platform")){
            PhysicallyPressTheButton();
            elevatorScript.AddForceToElevator(forceToApply);
        }
    }
    private void OnTriggerExit2D(Collider2D other){
        if(other.transform.CompareTag("Platform")){
            PhysicallyReleaseTheButton();
            elevatorScript.AddForceToElevator(-forceToApply);
        }
    }
}
