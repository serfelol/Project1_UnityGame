using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines movement functionalities for 2D plataform characters.
/// - walk/Run - Horizontal Movement;
/// - Jumping and JumpCut;
/// </summary>
public class CharacterMovement2D : MonoBehaviour
{
    #region fields
    private Rigidbody2D rb2d;

    [Header("Movement Fields")]
    [SerializeField] private float maxVelocity;
    [SerializeField] private float accelerationValue;
    [SerializeField] private float deccelerationValue;
    [SerializeField] private float velPower;            // ideal is between 0.85 and 1.1.
    private bool facingRight = true;

    [Header("Jump Fields")]
    [SerializeField] private float jumpForce;

    [Header("Fall Feilds")]
    [SerializeField] private float gravityMultiplier;
    private float gravity;

    [Header("Ground Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded;

    [Header("Movement Fields Debug Values")]
    [SerializeField] private string acceleration_X;
    private float lastVelocity;

    #endregion

    #region properties

    public bool IsGrounded{
        get{ return isGrounded; }
    }
    public bool FacingRight{
        get{ return facingRight; }
    }
    #endregion

    private void Awake() {
        if(!(rb2d = GetComponent<Rigidbody2D>())){
            Debug.Log("RigidBody not found on character");
        }       
    }

    void Start()
    {
        gravity = rb2d.gravityScale;
    }

    void Update()
    {
        CheckIfGrounded();

        // falling multiplier
        if(rb2d.velocity.y < 0){
            rb2d.gravityScale = gravity * gravityMultiplier;
        }
        else{
            rb2d.gravityScale = gravity;
        }
    }

    public void Move(float directionalMovement){
        // calculates the speed magnitude with directional movement
        float intendedSpeed = directionalMovement * maxVelocity;
        // the closer to the limit velocity the less force will be add
        float speedDiferrence = intendedSpeed - rb2d.velocity.x;
        // register if we are acellarating or decelarating
        float accelRate = Mathf.Abs(directionalMovement) > 0.1 ? accelerationValue : deccelerationValue;
        // calculate the force to be add. Setpower > 1 makes initial aceleration faster in comparation to final; set power < 1 it's the opposite.
        float force = Mathf.Pow(Mathf.Abs(speedDiferrence) * accelRate, velPower) * Mathf.Sign(speedDiferrence);
        // add the force we have calculated. Multiply by Vector2.right so we have a Vector as argument.
        rb2d.AddForce(Vector2.right * force);

        #region DebugInspectorFields
        acceleration_X = ((rb2d.velocity.x - lastVelocity) / Time.fixedDeltaTime).ToString("0.00");
        lastVelocity = rb2d.velocity.x;
        #endregion
    }

    public void Jump(){
        Vector2 currentSpeed = rb2d.velocity;
        currentSpeed.y = 0;
        rb2d.velocity = currentSpeed;
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);  
    }

    /// <summary>
    /// Makes the jump be stopped and character start falling.
    /// minJumpDuration is used to guarantee the smallest jump can be configured.
    /// </summary>
    public void JumpCut(){
        Vector2 instantVelocity = rb2d.velocity;
        instantVelocity.y = 0;
        rb2d.velocity = instantVelocity;
    }

    public void TurnArround(float movementInput){
        if((facingRight && movementInput < 0) || (!facingRight && movementInput > 0)){
            // multiplies the x scale by -1
            Vector3 currentScale = transform.localScale;
            currentScale.x *= -1;
            transform.localScale = currentScale;
            // updates the facing reference
            facingRight = !facingRight;
        }
    }
    
    private void CheckIfGrounded(){
        if(Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer)){
            isGrounded = true; 
        }
        else
            isGrounded = false;
    } 
}
