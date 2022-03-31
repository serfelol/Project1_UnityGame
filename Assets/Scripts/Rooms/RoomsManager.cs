using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// to be singleton
public class RoomsManager : MonoBehaviour
{
    public CameraScript activeCamera;
    public Room startingRoom;
    private Room currentActiveRoom;
    public GameObject room2_lever;
    public GameObject[] room2_plataforms;
    public GameObject room2_elevator;

    private void Start() {
        activeCamera.SetCameraNewPosition(startingRoom);
        currentActiveRoom = startingRoom;
    }

    public void ChangeRoom(Room roomToGo){
        currentActiveRoom = roomToGo;
        activeCamera.SetCameraNewPosition(currentActiveRoom);
    }

    public void Room2_LeverAction(bool leverState){
        Debug.Log("Axe on Lever");
        if(leverState){
            foreach(GameObject plataform in room2_plataforms){
                if(plataform.GetComponent<FloatingPlatform>()){
                    plataform.GetComponent<FloatingPlatform>().ActivatePlatform = true;
                }
                room2_elevator.GetComponent<Elevator2Room2>().ActivateElevator();
            }
        }
    }
}
