using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides long-range weapons throwing features.
/// </summary>
public class WeaponThrower : MonoBehaviour
{
    [Header("Throwing Fields")]
    [SerializeField] private float throwAngle = 45.0f;
    [SerializeField] private float throwMaxForce = 7;
    [SerializeField] private float axeChargeRate = 0.5f;
    [SerializeField] private Transform axeHandPosition;
    private float throwChargedForce;  
    private CharacterMovement2D characMovement2D;
    [SerializeField] private float returnAxeVelocity = 5;
    private Vector2 throwFinalForce;
    private int directionalForce;
    private bool mouseReleasedAfterCatchAxe;
    private bool spaceToThrowAvailable;
    
    [Header("Axe Field")]
    [SerializeField] private GameObject AxePrefab;
    GameObject launchedAxe;

    #region Animator
    private PlayerAnimation animatorController;
    #endregion

    #region properties
    public GameObject LaunchedObject{
        get{
            return launchedAxe;
        }
    }
    #endregion
    
    private void OnEnable() {
        Axe.OnAxeDestroyed += AnimationEventCatchAxe;
    }
    private void OnDisable() {
        Axe.OnAxeDestroyed -= AnimationEventCatchAxe;
    }

    private void Awake() {
        if(!(animatorController = GetComponent<PlayerAnimation>())){
            Debug.Log("Animator not found!");
        }
    }

    void Start()
    {
        if(!(characMovement2D = GetComponent<CharacterMovement2D>())){
            Debug.Log("Componente CharacterMovement2D não foi encontrada");
        }
        // sets the initial hold force.
        throwChargedForce = 0.1f;
    }

    void Update()
    {
        // Check if there is room to throw axe.
        if(Physics2D.OverlapCircle(axeHandPosition.position, 0.05f) == null) spaceToThrowAvailable = true;
        else spaceToThrowAvailable = false;

        if(Input.GetKeyDown(KeyCode.Mouse1) && !mouseReleasedAfterCatchAxe){
            mouseReleasedAfterCatchAxe = true;
        }
        // charges the attack force by holding the key until max force.
        if(Input.GetKey(KeyCode.Mouse1) && !launchedAxe && mouseReleasedAfterCatchAxe){
            if(throwChargedForce <= 1.0f){
                throwChargedForce += (axeChargeRate * Time.deltaTime);
            }
        }
        // return the axe, when it's somewhere in the environment
        else if(Input.GetKeyDown(KeyCode.Mouse1) && LaunchedObject){
            Debug.Log("Axe pulled");
            LaunchedObject.GetComponent<Axe>().ReturnAxe(axeHandPosition, returnAxeVelocity);
        }
        // Throw Axe if space available
        else if(Input.GetKeyUp(KeyCode.Mouse1) && !launchedAxe && mouseReleasedAfterCatchAxe ){
            if(spaceToThrowAvailable){
                // converts the throw angle to a vector2.
                Vector2 angToVector2 = Vector2.zero;
                angToVector2.y = throwAngle / 90.0f;
                angToVector2.x = 1 - angToVector2.y; 

                // calculates the final force to be applied to the axe.
                throwFinalForce = angToVector2 * throwMaxForce * throwChargedForce;

                // starts the throw animation.
                animatorController.ChangeAnimationState(PlayerState.Player_ThrowAxe); 

                Debug.Log("Axe thrown");
            }
            else Debug.Log("No space to throw");

            // charged force go back to 0 so the player can throw another axe.
            throwChargedForce = 0.1f;
        }

    }
    // made to be called by the throw animation at the right time. 
    public void AnimationEventThowAxe(){
        // create the axe.
        launchedAxe = Instantiate(AxePrefab, this.transform.position, Quaternion.identity);
        // put it in the right throw position.
        launchedAxe.transform.localPosition = axeHandPosition.position;
        Axe instantiatedAxeScript = launchedAxe.GetComponent<Axe>();
        // gets the direction character is facing to apply proper directional force.
        directionalForce = characMovement2D.FacingRight? 1: -1;
        // applies the directionalForce only to the x value.
        throwFinalForce.x *= directionalForce;
        instantiatedAxeScript.ThrowAxe(throwFinalForce,directionalForce);
    }

    public void AnimationEventCatchAxe(){
        animatorController.ChangeAnimationState(PlayerState.Player_CatchAxe);
        mouseReleasedAfterCatchAxe = false;
    }
}
