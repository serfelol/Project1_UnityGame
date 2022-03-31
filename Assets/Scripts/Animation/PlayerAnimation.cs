using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState{
    Player_Idle,
    Player_Walk,
    Player_Jump,
    Player_Falling,
    Player_ThrowAxe,
    Player_CatchAxe
}

public class PlayerAnimation : MonoBehaviour
{
    private Animator playerAnimator;
    private PlayerState currentState;

    private void Awake() {
        if(!(playerAnimator = GetComponent<Animator>())){
            Debug.Log("Player animator not found!");
        }                
    }

    public void ChangeAnimationState(PlayerState newState){
        if(currentState == newState) return;
        if(currentState == PlayerState.Player_ThrowAxe || currentState == PlayerState.Player_CatchAxe){
            if(playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return; 
        }

        playerAnimator.Play(newState.ToString());

        currentState = newState;
    }
}
