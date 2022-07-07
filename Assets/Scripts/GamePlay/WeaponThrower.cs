using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides long-range weapons throwing features.
/// </summary>
public class WeaponThrower : MonoBehaviour
{
    #region Fields
    [Header("Throwing Fields")]
    [SerializeField] private float throwAngle;
    [SerializeField] private float throwMaxForce;
    [SerializeField] private float axeChargeRate;
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
    #endregion

    #region Animator
    private PlayerAnimation animatorController;
    #endregion

    #region Trajectory Line
    public GameObject trajectoryLinePrefab;
    private GameObject trajectoryLineRef;
    private ProjectileTrajectory projectileTrajectory;
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
        // Check if there is space to throw axe.
        if(Physics2D.OverlapCircle(axeHandPosition.position, 0.05f) == null) spaceToThrowAvailable = true;
        else spaceToThrowAvailable = false;

        if(Input.GetKeyDown(KeyCode.Mouse1) && !mouseReleasedAfterCatchAxe){
            mouseReleasedAfterCatchAxe = true;

            // create a line representing the trajectory of the axe
            trajectoryLineRef = Instantiate(trajectoryLinePrefab, transform);
            projectileTrajectory = trajectoryLineRef.GetComponent<ProjectileTrajectory>();
            projectileTrajectory.InitialPosition = axeHandPosition.position;
            projectileTrajectory.Angle = throwAngle;
        }
        // charges the attack force by holding the key until max force.
        if(Input.GetKey(KeyCode.Mouse1) && !launchedAxe && mouseReleasedAfterCatchAxe){
            if(throwChargedForce <= 1.0f){
                throwChargedForce += (axeChargeRate * Time.deltaTime);
                projectileTrajectory.ForceBeingApplied = throwMaxForce * throwChargedForce;
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
                angToVector2.x = Mathf.Cos(throwAngle * Mathf.Deg2Rad);
                angToVector2.y = Mathf.Sin(throwAngle * Mathf.Deg2Rad);

                // calculates the final force to be applied to the axe.
                throwFinalForce = angToVector2 * throwMaxForce * throwChargedForce;

                // starts the throw animation.
                animatorController.ChangeAnimationState(PlayerState.Player_ThrowAxe); 

                Debug.Log("Axe thrown");
            }
            else Debug.Log("No space to throw");

            // charged force go back to 0 so the player can throw another axe.
            throwChargedForce = 0.1f;

            // destroy the trajectory line for the axe
            Destroy(trajectoryLineRef);
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
        StartCoroutine("DEbugVelocity");
    }

    public void AnimationEventCatchAxe(){
        animatorController.ChangeAnimationState(PlayerState.Player_CatchAxe);
        mouseReleasedAfterCatchAxe = false;
    }
    IEnumerator DEbugVelocity()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Velocity: " + launchedAxe.GetComponent<Rigidbody2D>().velocity);
    }
}
