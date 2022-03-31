using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF1 : MonoBehaviour
{
    public BoxCollider2D boxCollider2D;
    public LayerMask groundLayer;
    public Transform groundCheck;

    [Header("Activated Plataforma Fields")]   
    public float activedPlatformPositionY;
    private float deactivatedPlataformPositionY;
    public float activateTravelTime;
    private bool isPlatformActivating;
    private bool isPlatformInContactWithPlayer;
    private bool isPlatformActivated;
    private bool isPlatformOnGround;
    private bool isPlatformInContactWithAxe;
    
    private float axeToPlayerWindowTime = 0.1f;
    private float axeToPlayerTimer;

    [Header("Plataform Oscillating Fields")]
    [SerializeField] private float oscillationRange = 0.15f;
    [SerializeField] private float oscillationTime = 5.5f;
    private float cumulativeTime;
    private bool platformGoingUp;
    private float lastFramePosition;
    [SerializeField] private float returningSpeed = 1.0f;

    // lock button
    private Button_SuspendedPlat buttonPlatformScript;
    public BoxCollider2D groundCollider;

    private void Awake() {
        isPlatformActivating = false;
        isPlatformActivated = false;
        isPlatformInContactWithPlayer = false;
        axeToPlayerTimer = 0;
        cumulativeTime = 0;
        lastFramePosition = transform.position.y;
        deactivatedPlataformPositionY = transform.position.y;
        buttonPlatformScript = transform.GetChild(3).GetComponent<Button_SuspendedPlat>();
        if(!buttonPlatformScript){
            Debug.Log("suspendedPlat script not found");
        }
    }

    private void Update() {
        // while is being activated.
        if(isPlatformActivating){
            ActivatingPlataform();
        }

        // after being activated and resting at normal position.
        if(isPlatformActivated && !isPlatformInContactWithPlayer && !isPlatformInContactWithAxe && (transform.position.y >= activedPlatformPositionY)){
            OscillatePlataform();
        }

        // lower the platform if player or axe on top.
        if(isPlatformActivated && !isPlatformOnGround && (isPlatformInContactWithPlayer || isPlatformInContactWithAxe)){
            transform.position = new Vector2(transform.position.x, transform.position.y - 0.7f * Time.deltaTime); 
        }

        // coming back to the normal position after being pressed down.
        if(isPlatformActivated && !isPlatformInContactWithPlayer && !isPlatformInContactWithAxe && (transform.position.y < activedPlatformPositionY) && !buttonPlatformScript.wasButtonActivated){
            transform.position = new Vector2(transform.position.x, transform.position.y + returningSpeed * Time.deltaTime);  
        }

        lastFramePosition = transform.position.y;

        // activate button to hold the platform
        if(isPlatformOnGround && buttonPlatformScript.isBeingPressed == true){
            buttonPlatformScript.ActivateButton();
        }

        if(groundCollider.IsTouchingLayers(groundLayer)){
            isPlatformOnGround = true;
        }
        else{
            isPlatformOnGround = false;
        }

        IsAxeInObject();
    }

    // when player is no longer on the platform it will start to raise again.
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            isPlatformInContactWithPlayer = false;
        }
    }

    // when player in contact with triggers plataform will fall.
    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Player")){
            isPlatformInContactWithPlayer = true;            
        }
    }
        private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            isPlatformInContactWithPlayer = true;            
        }
    }

    public void ActivatePlataform(){
        isPlatformActivating = true;
    }

    // platform disappears and appears on the deactivated position.
    public void DeactivatePlataform(){
        transform.position = new Vector2(transform.position.x, deactivatedPlataformPositionY);
    }

    // activating the plataform takes time for the plataform to reach the destination.
    private void ActivatingPlataform(){
        if(transform.position.y != activedPlatformPositionY){
            // move the plataform for the intended position.
            cumulativeTime += Time.deltaTime / activateTravelTime;
            Vector2 deactivatePlatformPos = new Vector2(transform.position.x, deactivatedPlataformPositionY);
            Vector2 activePlatformPos = new Vector2(transform.position.x, activedPlatformPositionY);
            transform.position = Vector2.Lerp(deactivatePlatformPos, activePlatformPos, cumulativeTime);
        }
        else{
            isPlatformActivated = true;
            isPlatformActivating = false;
            cumulativeTime = 0;
        }
    }

    // Idle - Oscillate the platform between the initial position and any position above that one.
    private void OscillatePlataform(){
        Vector2 lowestPlatformPos = new Vector2(transform.position.x, activedPlatformPositionY);
        Vector2 highestPlatformPos = new Vector2(transform.position.x, activedPlatformPositionY + oscillationRange);
        float differenceDistance = highestPlatformPos.y - lowestPlatformPos.y;
        if(transform.position.y <= lowestPlatformPos.y){
            platformGoingUp = true;
        }
        else if( transform.position.y >= highestPlatformPos.y){
            platformGoingUp = false;
        }

        float plataformProgression;
        float magnitudeOne;
        if(platformGoingUp){
            magnitudeOne = (lastFramePosition - activedPlatformPositionY) / differenceDistance;
            plataformProgression = magnitudeOne + Time.deltaTime / oscillationTime;
            transform.position = Vector2.Lerp(lowestPlatformPos, highestPlatformPos, plataformProgression);
        }
        else{
            magnitudeOne = (lastFramePosition - activedPlatformPositionY) / differenceDistance;
            plataformProgression = magnitudeOne - Time.deltaTime / oscillationTime;
            transform.position = Vector2.Lerp(lowestPlatformPos, highestPlatformPos, plataformProgression);
        }
    }

    // create a window so the player can teleport to the axe and platform dont reset the break.
    private void IsAxeInObject(){
        if(axeToPlayerTimer>= 0){
            axeToPlayerTimer -= Time.deltaTime;
        }

        if(transform.Find("AxeParent")){
            axeToPlayerTimer = axeToPlayerWindowTime;
        }

        if(axeToPlayerTimer > 0){
            isPlatformInContactWithAxe = true;    
        }
        else{
            isPlatformInContactWithAxe = false;
        }
    }
    // Platform is inactivated until room trigger makes them go to the initial position (this position is the default position when they are enabled).
    // On the initial position there is an idle animation that make the platform oscillate (just for fun).
    // When player is above the platform it will fall until ground level. For this i use triggers and not collision.
    //  Because with collision the player always trigger exit collision when the platform falls and he stays on the air for that frame duration.
    // The axe makes the platform fall too, when it is stuck on it but the method has to be different because the axe turns to simulated on collisions
    //  and the platform dont know when it is colliding. For this i look for the axe on the hierarchy of the gameObject, because by this game behaviour the axe always 
    //  get the collided object as parent to stay in the same position.
}
