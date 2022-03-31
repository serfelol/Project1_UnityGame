using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Room{
    room1,
    room2,
    room3,
    roomDefaultValue
}

/// <summary>
/// Provides functionality to move a camera.
/// </summary>
public class CameraScript : MonoBehaviour
{
    private Camera thisCamera;
    private Room activeRoom;
    private bool changingCameraPosition = false;

    private Vector3 desiredCameraPosition;
    private Vector3 currentCameraPositionChangeVelocity;

    private float desiredCameraSize;
    private float currentCameraSizeChangeVelocity;

    private float timeForCameraToReachDestination = 1.2f;
    private float remaningTimeToChangeRoom;

    private void Awake() {
        thisCamera = transform.GetComponent<Camera>();
        activeRoom = Room.roomDefaultValue;
    }

    private void Update() {
        if(changingCameraPosition){
            if(Vector2.Distance(desiredCameraPosition, transform.position) == 0){
                changingCameraPosition = false;
                remaningTimeToChangeRoom = timeForCameraToReachDestination;
                return;
            }
            else{
                transform.position = Vector3.SmoothDamp(transform.position, desiredCameraPosition, ref currentCameraPositionChangeVelocity , remaningTimeToChangeRoom);
                thisCamera.orthographicSize = Mathf.SmoothDamp(thisCamera.orthographicSize, desiredCameraSize, ref currentCameraSizeChangeVelocity, remaningTimeToChangeRoom);
                remaningTimeToChangeRoom -= Time.deltaTime;
            }
        }
        else{
            remaningTimeToChangeRoom = timeForCameraToReachDestination;
        }
    }

    // drag the camera to a new position.
    public void SetCameraNewPosition(Room roomToEnter){
        if(roomToEnter != activeRoom){
            activeRoom = roomToEnter;
            SetDesiredRoomPosition(activeRoom);
            changingCameraPosition = true;
        }
    }
    // set desired position based on room.
    private void SetDesiredRoomPosition(Room roomToEnter){
        if(roomToEnter == Room.room1){
            desiredCameraPosition = new Vector3(-2.14f, -0.5f, -10);
            desiredCameraSize = 4.5f;
        }
        else if(roomToEnter == Room.room2){
            desiredCameraPosition = new Vector3(15.5f, 0.6f, -10);
            desiredCameraSize = 5.2f;
        }
        else if(roomToEnter == Room.room3){
            desiredCameraPosition = new Vector3(-0.66f, 10.6f, -10);
            desiredCameraSize = 5.2f;
        }
    }
}
