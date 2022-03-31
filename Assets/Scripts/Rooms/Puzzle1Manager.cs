using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle1Manager : MonoBehaviour
{
    // force fields
    private int currentForceApplied = 0;
    private bool wasForceApplied;
    private float destroyBrakeForce = 1.20f;
    private int brakeMaxForce = 4;

    // brake fields
    public GameObject brake;
    public Puzzle_Brake brakeScript;
    private bool isBrakeOn;                         // brake state varies by being pressed or when its busted
    private bool isBrakePlatformPressingButton;
    private float brakeResetTime = 1.5f;
    private float brakeTimer = 0;
    private bool brakeBrokeOneTimeEvent;
    public ParticleSystem sparksBrakeEffect;

    // PF1 lock button
    public List<Button_SuspendedPlat> buttonsSuspendedPlatforms;

    // elevator fields
    public Puzzle_Elevator elevatorScript;

    private void Awake() {
        brakeBrokeOneTimeEvent = false;
    }

    private void Update() {
        // update reference to the brake state
        isBrakeOn = brakeScript.BrakeState;

        // check if the platform for the brake is still pressing the button,
        if(isBrakePlatformPressingButton && brakeTimer <= 0){
            brakeScript.BrakeState = true;
        }
        else if(!isBrakePlatformPressingButton) {
            brakeScript.BrakeState = false;
        }

        // brake is broked
        if(isBrakeOn && currentForceApplied >= brakeMaxForce && !brakeBrokeOneTimeEvent){
            sparksBrakeEffect.Play();
            StartCoroutine(ExplodeBrake());
        }

        // run the timer
        if(brakeTimer > 0){
            brakeTimer -= Time.deltaTime;
        }

        // so it wont apply over again the impulse force after first time.
        if(currentForceApplied == 0){
            brakeBrokeOneTimeEvent = false;
            elevatorScript.SetCurrentForce(currentForceApplied);
        }
    }

    public void SetBrakeState(bool platformPressing){
        isBrakePlatformPressingButton = platformPressing;
    }

    public void AddForce(int buttonForce){
        currentForceApplied += buttonForce;
        wasForceApplied = false;
        if(!isBrakeOn){
            Debug.Log("olele");
            elevatorScript.SetElevatorPositionEvenly(currentForceApplied);
            wasForceApplied = true;
        }
    }

    // to deactivate the brake
    public void UnlockPlatformsFromGround(){
        foreach(Button_SuspendedPlat buttoni in buttonsSuspendedPlatforms){
            buttoni.DeactivateButton();
        }
    }

    private IEnumerator ExplodeBrake(){
        brakeBrokeOneTimeEvent = true;
        yield return new WaitForSeconds(2.5f);

        brakeScript.BrakeState = false;
        elevatorScript.SetElevatorPositionImpulsively(currentForceApplied);
        brakeTimer = brakeResetTime;
        foreach(Button_SuspendedPlat buttoni in buttonsSuspendedPlatforms){
            buttoni.DeactivateButton();
        }
    }
}
