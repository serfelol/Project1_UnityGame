using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the player behaviour in terms of movement calls and restrictions.
/// Requires a movement script for movement functionalities.
/// </summary>

public class PlayerControllerBehaviour : MonoBehaviour
{
    private CharacterMovement2D this_controller;
    private PlayerAnimation animationController;
    private float movementInput;
    private bool jumpInput;
    private bool jumpHoldInput;
    private float frictionAmount = 1.9f;

    [Header("Jump Behaviour Fields")]
    [SerializeField] private float jumpCoyoteTime = 0.2f;   // player can still jump within this time window after leaving the ground.
    private float timeSinceGrounded;
    [SerializeField] private float jumpBufferTime;          // if player is pressing jump button before touch the ground, jump will be buffered.
    private float lastJumpKeyInputTimer;
    [SerializeField] private float minJumpHeight = 0.1f;
    private Timer minJumpDurationTimer;
    private bool jumpAvailable;
    private Rigidbody2D rb2d;
    private bool isGrounded;
    private bool jumpKeyReleased;
    private bool wasJumpCut;

    public float MovementInput{
        get{ return movementInput; }
        set{ this.movementInput = value; }
    }

    private void Awake() {
        if(!(this_controller = GetComponent<CharacterMovement2D>())){
            Debug.Log("CharacterController2D não foi encontrado");
        }
        if(!(rb2d = GetComponent<Rigidbody2D>())){
            Debug.Log("RigidBody2D não foi encontrado");
        }
        if(!(animationController = GetComponent<PlayerAnimation>())){
            Debug.Log("Player animator script not found on character");
        }                
    }

    void Start()
    {
        minJumpDurationTimer = gameObject.AddComponent<Timer>(); 
    }

    void Update()
    {
        // animation.
        WalkAnimation();
        AirAnimation();

        // keep the timers updated.
        RunTimers();

        // user movement input.
        movementInput = Input.GetAxisRaw("Horizontal");

        // user jump input.
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        jumpHoldInput = Input.GetKey(KeyCode.Space);

        // executes a jump.
        JumpValidator();

        // call the method to cut the jump.
        if(!jumpHoldInput){
            if((rb2d.velocity.y > 0) && !wasJumpCut && !isGrounded){
                if(minJumpDurationTimer.Finished){
                    this_controller.JumpCut();
                    wasJumpCut = true;
                }
                else{
                    StartCoroutine(JumpCutTimer());
                }
            }            
            jumpKeyReleased = true;
            lastJumpKeyInputTimer = 0;
        }

        IEnumerator JumpCutTimer(){
            if(!minJumpDurationTimer.Finished){
                yield return new WaitForEndOfFrame();  
            }
        }

        // turn object based on the current facing direction and velocity.
        // TODO: more then just turn i can add something like 4 frame turn animation. 
        this_controller.TurnArround(movementInput);       
    }
    private void FixedUpdate() {
        // call to move
        this_controller.Move(movementInput);

        #region Friction
		//check if we're grounded and that we are trying to stop. (not pressing forwards or backwards)
		if (isGrounded && Mathf.Abs(movementInput) < 0.01f) 
		{
			//then we use either the friction amount (~ 0.2) or our velocity.
			float amount = Mathf.Min(Mathf.Abs(rb2d.velocity.x), Mathf.Abs(frictionAmount));
			amount *= Mathf.Sign(rb2d.velocity.x);
			//applies force against movement direction.
			rb2d.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
		}
		#endregion
    }
    private void RunTimers(){
        // reduces timer by the last frame delta time.
        if(lastJumpKeyInputTimer > 0){
            lastJumpKeyInputTimer -= Time.deltaTime;
        }
        if(timeSinceGrounded > 0){
            timeSinceGrounded -= Time.deltaTime;
        }
    }

    private void JumpValidator(){
        isGrounded = this_controller.IsGrounded;

        // sets the the grounded timer to the coyote time each time it is on the groud.
        if(isGrounded == true){
            timeSinceGrounded = jumpCoyoteTime;
        }
        // timer keep reseting until key stops to be pressed.
        if(jumpInput){
            lastJumpKeyInputTimer = jumpBufferTime;
        }

        if(isGrounded && jumpKeyReleased){
            jumpAvailable = true;
        }

        // call to jump.
        if( lastJumpKeyInputTimer > 0 && timeSinceGrounded > 0 && jumpAvailable){
            this_controller.Jump();

            jumpAvailable = false;

            // when we jump it's impossible for the jump to have been already cutted.
            wasJumpCut = false;
            jumpKeyReleased = false;

            // starting the timer for min jump duration.
            minJumpDurationTimer.Duration = minJumpHeight;
            minJumpDurationTimer.Run();
        }
    }

    public void RespawnPlayer(Vector2 position){
        rb2d.velocity = Vector2.zero;
        this.transform.position = position;
    }

    private void WalkAnimation(){
        if(isGrounded){
            if(movementInput != 0){
                animationController.ChangeAnimationState(PlayerState.Player_Walk);
            }
            else{
                animationController.ChangeAnimationState(PlayerState.Player_Idle);
            }
        }
    }
    
    private void AirAnimation(){
        if(!isGrounded){
            if(rb2d.velocity.y < 0){
                animationController.ChangeAnimationState(PlayerState.Player_Falling);
            }
            else{
                animationController.ChangeAnimationState(PlayerState.Player_Jump);
            }
        }
    }
}
