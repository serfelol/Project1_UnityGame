using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    // force fields
    private float currentAppliedForce;
    public float forceToAltitudeRatio;
    public float elevatorSpeed;
    public float blockerMaxForceSupported;

    // impulsive force fields
    private bool onImpulsiveMotion;
    public float impulsivePositionModifier;
    public float elevatorSpeedIF = 2.0f;
    private bool elevatorTookOff = false;

    // position fields
    public float lowestElevatorPosition;
    public float lowestPositionAboveBlocker;
    private float altitudeToReach;
    private float interpolationValue;
    public bool isElevatorAboveBlocker;

    // blocker fields
    public ElevatorBlocker blockerScriptRef;
    private bool blockerStateRef;
    public bool BlockerStateRef{
        set{blockerStateRef = value;}
    }

    private void Update() {
        // calculates the position where the elevator needs to go.
        if(onImpulsiveMotion && !elevatorTookOff){
            // with elevatorTookOff variable even if player let platform raise and take out the force applied it will still go to the right altitude.
            altitudeToReach = lowestElevatorPosition + currentAppliedForce * forceToAltitudeRatio * impulsivePositionModifier;
        }
        else if (!onImpulsiveMotion){
            altitudeToReach = lowestElevatorPosition + currentAppliedForce * forceToAltitudeRatio;
        }

        // checks if platform is above or under the blocker
        isElevatorAboveBlocker = transform.position.y >= lowestPositionAboveBlocker? true : false;

        #region movementBehaviour
        if(blockerStateRef == true && !isElevatorAboveBlocker){
            if(currentAppliedForce >= blockerMaxForceSupported){
                // check if the force to be applied is greater than what the blocker can handle.
                onImpulsiveMotion = true;
                blockerScriptRef.BreakTheBlocker();
            }
            // don't let elevator pass through the blocker. When ascending and descending.
            return;
        }
        else if (!onImpulsiveMotion){
            // updates the elevator altitude based on the altitude it needs to go and its current position.
            if(altitudeToReach > transform.position.y){
                transform.position = new Vector2(transform.position.x, transform.position.y + elevatorSpeed * Time.deltaTime);
                if(altitudeToReach < transform.position.y){
                    transform.position = new Vector3(transform.position.x, altitudeToReach);
                }
            }
            else if(altitudeToReach < transform.position.y){
                Vector2 positionToGo = new Vector2(transform.position.x, transform.position.y - elevatorSpeed * Time.deltaTime);
                if(blockerStateRef && positionToGo.y < lowestPositionAboveBlocker) return;
                else transform.position = positionToGo;

                if(altitudeToReach > transform.position.y){
                    transform.position = new Vector3(transform.position.x, altitudeToReach);
                }
            }
        }
        else if(onImpulsiveMotion) {
            elevatorTookOff = true; 
            if(transform.position.y < altitudeToReach){
                // distance to go.
                float distanceToTravel = altitudeToReach - lowestElevatorPosition;
                // time it takes.
                float timeToTake = distanceToTravel / elevatorSpeedIF;
                // distance traveled per frame.
                float distancePerFrame = distanceToTravel / (1 / Time.deltaTime * timeToTake);
                // normalize this value between 0 and 1.
                float normalized = Mathf.InverseLerp(0.0f, distanceToTravel, distancePerFrame);
                // increment the interpolation value.
                interpolationValue += normalized;
                // set new position
                transform.position = new Vector2(transform.position.x, Mathf.Lerp(lowestElevatorPosition, altitudeToReach, Mathf.Pow(interpolationValue, 0.8f)));

                // check if destination has been reached.
                if(transform.position.y >= altitudeToReach){
                    onImpulsiveMotion = false;
                    elevatorTookOff = false;
                    interpolationValue = 0;
                }
            }
        }
        #endregion
    }

    // Change the force applied to the elevator.
    public void AddForceToElevator(float force){
        currentAppliedForce += force;
    }
}
