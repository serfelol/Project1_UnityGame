using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Floating platforms serve to press the buttons
/// that give elevator force to go up.
/// Can be moved by: player and axe.
/// </summary>
public class FloatingPlatform : MonoBehaviour
{
    [Header("Activating Platform Fields")]
    public float activedPositionY;
    private float deactivatedPositionY;
    public float durationToActivate;
    private bool isPlatformActivating;
    private bool isPlatformActivated;
    private float interpolatedValue; // cumulative, clamped between [0,1]

    [Header("Bouncing Animation Fields")]
    public float oscillationRange;
    public float oscillationTime;
    private bool isPlatformGoingUp;
    private float lastFramePosition;

    // axe/player contact fields
    private bool isPlatformInContactWithPlayer;
    private bool isPlatformInContactWithAxe;
    private float axeCollisionWindowTime = 0.1f;        
    private float axeCollisionIteration;

    // ground contact fields
    private bool isPlatformOnGround;

    // hold platform on ground
    private bool buttonToFastenPlatformBeingPressed;
    private bool fastenPlatform;
    public event Action platformGroundCollisionEvent;

    // set by lever in game
    public bool ActivatePlatform{
        get{ return isPlatformActivating;}
        set{ if( !isPlatformActivated && value == true ) isPlatformActivating = value;}
    }
    public bool FastenPlatformButton{
        get{ return buttonToFastenPlatformBeingPressed;}
        set{ buttonToFastenPlatformBeingPressed = value;}
    }

    private void Awake() {
        isPlatformActivating = false;
        isPlatformActivated = false;
        deactivatedPositionY = transform.position.y;
        lastFramePosition = activedPositionY;
    }
    private void OnEnable() {
        FPUnfasten_Button.UnfastenFloatingPlatformsEvent += UnfastenPlatformFromTheGround;
    }
    private void OnDisable() {
        FPUnfasten_Button.UnfastenFloatingPlatformsEvent -= UnfastenPlatformFromTheGround;
    }

    private void Update() {
        if(isPlatformActivating) ActivatingPlataform();
        IsAxeInContact();
        PlatformMovement();
        FastenPlatformToTheGround();

    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            isPlatformInContactWithPlayer = true;
        }
        if(other.CompareTag("Ground")){
            // move the platform deeper into the button so it wont enter and exit trigger everyframe.
            MoveThePlatform(-0.7f);
            // calls the event for the button to catch.
            isPlatformOnGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            isPlatformInContactWithPlayer = false;
        }
        if(other.CompareTag("Ground")){
            isPlatformOnGround = false;
        }
    }

    // Manages the correct movement to be applied to the platform.
    private void PlatformMovement(){
        if(isPlatformActivated){
            if(isPlatformInContactWithPlayer || isPlatformInContactWithAxe){
                if(!isPlatformOnGround){
                    // lower the platform if player or axe on top.
                    MoveThePlatform(-0.7f);
                }
            }else{
                if(transform.position.y >= activedPositionY){
                    // after being activated the platform will start an animation if nothing is in contact.
                    OscillatePlataform();
                }
                else if(!fastenPlatform){
                    // platform goes back up to activated position after being pressed down.
                    MoveThePlatform(0.7f);                    
                }
            }
        }
    }

    // Move the platform in the desired vertical position.
    private void MoveThePlatform(float unitsBySecond){
        transform.position = new Vector2(transform.position.x, transform.position.y + unitsBySecond * Time.deltaTime); 
    }

    // Fasten the platform to the ground so it wont go up again.
    private void FastenPlatformToTheGround(){
        if(!fastenPlatform && buttonToFastenPlatformBeingPressed && isPlatformOnGround){
            fastenPlatform = true;
            if(platformGroundCollisionEvent != null) platformGroundCollisionEvent();
        }
    }
    private void UnfastenPlatformFromTheGround(){
        fastenPlatform = false;
    }

    // Sets the platform position from deactivated to activated position.
    // Update the state of platform - isPlatformActivated = true. 
    private void ActivatingPlataform(){
        if(interpolatedValue < 1){
            Vector2 deactivatePlatformPos = new Vector2(transform.position.x, deactivatedPositionY);
            Vector2 activePlatformPos = new Vector2(transform.position.x, activedPositionY);
            float clampValueIncrement = Time.deltaTime / durationToActivate;
            interpolatedValue += clampValueIncrement;    
            transform.position = Vector2.Lerp(deactivatePlatformPos, activePlatformPos, interpolatedValue);
        }
        else{
            isPlatformActivated = true;
            isPlatformActivating = false;
            interpolatedValue = 0;
        }
    }

    // Makes the platform oscillate between two positions like a idle animation.
    private void OscillatePlataform(){
        Vector2 lowestPlatformPos = new Vector2(transform.position.x, activedPositionY);
        Vector2 highestPlatformPos = new Vector2(transform.position.x, activedPositionY + oscillationRange);

        // defining whether the platform is going up or down.
        if(transform.position.y <= lowestPlatformPos.y){
            isPlatformGoingUp = true;
        }
        else if( transform.position.y >= highestPlatformPos.y){
            isPlatformGoingUp = false;
        }

        // saves a reference to platform current direction.
        float vectorSense = isPlatformGoingUp? 1 : -1;
        // get the current interpolation value. We need this because it may change if player jumps to the platform and leaves still inside the oscillation range.
        float oscillatePlatformInterpolateValue = Mathf.InverseLerp(lowestPlatformPos.y, highestPlatformPos.y, transform.position.y);
        // calculate the new interpolation value for platform position.
        oscillatePlatformInterpolateValue += vectorSense * Time.deltaTime / oscillationTime;
        // sets the new position for the platform.
        transform.position = Vector2.Lerp(lowestPlatformPos, highestPlatformPos, oscillatePlatformInterpolateValue);
    }

    // Checks if the axe is on the object (by parenting behaviour).
    // According to the game's needs, a time margin has been introduced
    // after the axe is no longer on the object - This way when the player teleports 
    // to an axe situated on the platform it will never return that neither of them is colliding with it.
    private void IsAxeInContact(){
        if(transform.Find("AxeParent")){
            axeCollisionIteration = axeCollisionWindowTime;
        }
        else if (axeCollisionIteration >= 0){
            axeCollisionIteration -= Time.deltaTime;
        }

        if(axeCollisionIteration > 0){
            isPlatformInContactWithAxe = true;    
        }
        else{
            isPlatformInContactWithAxe = false;
        }
    }
}
