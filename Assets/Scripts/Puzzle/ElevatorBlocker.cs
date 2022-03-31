using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// blocker that prevents the elevator from going up.
/// activated by: blockerActivator_Button.
/// </summary>
public class ElevatorBlocker : MonoBehaviour
{
    // blocker fields
    [SerializeField] private bool blockerButtonStateRef;
    [SerializeField] private bool blockerState;
    private Vector2 brakeDiactivatedRotation;
    private Vector2 brakeActivatedRotation;
    private float interpolateValue;
    public float turningDuration = 2;

    // break blocker fields
    [SerializeField] private bool blockerBroken;
    public float delayToBreakeBlocker;
    public float reactivateTimeAfterBreake;
    public ParticleSystem destroyingEffect;
    public ParticleSystem destroyedEffect;
    
    // elevator script reference
    public Elevator elevatorScript;


    // set by the button gameObject
    public bool BlockerButtonState{
        set{blockerButtonStateRef = value;}
    }

    private void Awake() {
        brakeActivatedRotation = Vector2.zero;
        brakeDiactivatedRotation = new Vector2(0,90);
        interpolateValue = 0;
        transform.rotation = Quaternion.Euler(brakeDiactivatedRotation);
        blockerState = false;
    }

    private void Update() {
        // sets the right state for the blocker
        if(blockerButtonStateRef && !blockerState && !blockerBroken){
            Debug.Log("blocker true");
            blockerState = true;
            elevatorScript.BlockerStateRef = true;
            interpolateValue = 0;
        }
        else if(!blockerButtonStateRef && blockerState && !blockerBroken){
            Debug.Log("Blocker to false");
            blockerState = false;
            elevatorScript.BlockerStateRef = false;
            interpolateValue = 0;
        }

        // turns the blocker
        if(blockerState && transform.rotation != Quaternion.Euler(brakeActivatedRotation)){
            interpolateValue += Time.deltaTime * turningDuration;
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(brakeDiactivatedRotation), Quaternion.Euler(brakeActivatedRotation), interpolateValue);
        }
        else if(!blockerState && transform.rotation != Quaternion.Euler(brakeDiactivatedRotation)){
            interpolateValue += Time.deltaTime * turningDuration;
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(brakeActivatedRotation), Quaternion.Euler(brakeDiactivatedRotation), interpolateValue);
        }  
    }

    #region Break Blocker
    public void BreakTheBlocker(){
        if(!blockerBroken){
            blockerBroken = true;
            destroyingEffect.Play();
            Debug.Log("oiosdwda");
            StartCoroutine(WaitToDestroy());
        }
    }
    private IEnumerator WaitToDestroy(){
        yield return new WaitForSeconds(delayToBreakeBlocker);
        destroyedEffect.Play();
        blockerState = false;
        elevatorScript.BlockerStateRef = false;
        StartCoroutine(WaitToRepair());
        // animação de destruir
    }
    private IEnumerator WaitToRepair(){
        yield return new WaitForSeconds(reactivateTimeAfterBreake);
        blockerBroken = false;
    } 
    #endregion
}
