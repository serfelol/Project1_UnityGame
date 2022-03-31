using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_Elevator : MonoBehaviour
{
    [SerializeField] private float restPositionY;
    [SerializeField] private float forceToDistanceConvertion = 0.8f;
    private float impulsiveForceMultiplier = 1.2f;
    private float currentAppliedForce;
    private float currentPositionY;
    private float positionToGoY;
    [SerializeField] private float elevatorSpeedPS;

    private float distanceToTravel_Impulse;
    private bool onImpulsiveForce;          // so the position elevator goes during impulsive force can't be changed in the mid of the event.
    float interpolationValue;

    private void Awake() {
        positionToGoY = restPositionY;
        currentAppliedForce = 0;
        distanceToTravel_Impulse = 0.1f;
        onImpulsiveForce = false;
    }


    private void Update() {
        if(!onImpulsiveForce){
            positionToGoY = restPositionY + currentAppliedForce * forceToDistanceConvertion;
        }

        if(onImpulsiveForce){
            if(transform.position.y < positionToGoY){
                // distancia a percorrer
                float distanceToTravel = positionToGoY - currentPositionY;
                // quero saber o tempo que demoro
                float timeToTake = distanceToTravel / elevatorSpeedPS;
                // quero saber quando vou andar por frame
                float distancePerFrame = distanceToTravel / (1 / Time.deltaTime) * timeToTake;
                // normalize this value between 0 and 1
                float normalized = Mathf.InverseLerp(0.0f, distanceToTravel, distancePerFrame);
                // increment the interpolation value
                interpolationValue += normalized;
                // set new position
                transform.position = new Vector2(transform.position.x, Mathf.Lerp(restPositionY, positionToGoY, Mathf.Pow(interpolationValue, 0.7f)));

                if(transform.position.y >= positionToGoY){
                    onImpulsiveForce = false;
                }
            }
        }
        else{
            if(positionToGoY > transform.position.y){
            transform.position = new Vector2(transform.position.x, transform.position.y + elevatorSpeedPS * Time.deltaTime);
                if(positionToGoY < transform.position.y){
                    transform.position = new Vector3(transform.position.x, positionToGoY);
                }
            }
            else if(positionToGoY < transform.position.y){
                transform.position = new Vector2(transform.position.x, transform.position.y - elevatorSpeedPS * Time.deltaTime);
                if(positionToGoY > transform.position.y){
                    transform.position = new Vector3(transform.position.x, positionToGoY);
                }
            }
        }
    }

    public void SetElevatorPositionEvenly(float force){
        currentAppliedForce = force;
        Debug.Log("1: " + currentAppliedForce); 
    }
    
    public void SetElevatorPositionImpulsively(float force){
        currentAppliedForce = force;
        Debug.Log("2: " + currentAppliedForce);
        if(!onImpulsiveForce){
            onImpulsiveForce = true;
            positionToGoY = restPositionY + currentAppliedForce * forceToDistanceConvertion * impulsiveForceMultiplier;
            interpolationValue = 0;
        }
    }
    public void SetCurrentForce(float force){
        currentAppliedForce = force;
    }
}
