using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator2Room2 : MonoBehaviour
{
    public Vector2 room2_elevatorBottomPosition;
    public Vector2 room2_elevatorTopPosition;
    public float elevatorVelocity;
    private bool isMoving = false;
    private bool isGoingTop = true;

    void Update()
    {
        if(isMoving){
            if((transform.position.y >= room2_elevatorBottomPosition.y) && (transform.position.y <= room2_elevatorTopPosition.y)){
                transform.position = new Vector2(transform.position.x, transform.position.y + (isGoingTop == true ? 1 : -1) * elevatorVelocity * Time.deltaTime);
            }
            else{
                isMoving = false;
                isGoingTop = !isGoingTop;
            }
        }
    }

    public void ActivateElevator(){
        isMoving = true;
    }
}
