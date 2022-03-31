using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The abilities available for the player.
/// teleport to the axe.
/// </summary>
public class PlayerAbilities : MonoBehaviour
{
    GameObject launchedObject;
    private Rigidbody2D rb2d;
    private WeaponThrower weaponThrower;

    private void Start() {
        if(!(rb2d = GetComponent<Rigidbody2D>())){
            Debug.Log("RigidBody2D not found!");
        }
        if(!(weaponThrower = GetComponent<WeaponThrower>())){
            Debug.Log("Script: WeaponThrower not found!");
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.E)){
            TeleportToAxe();
        }
    }

    public void TeleportToAxe(){
        launchedObject = weaponThrower.LaunchedObject;
        if(launchedObject){
            transform.position = new Vector2(launchedObject.transform.position.x, launchedObject.transform.position.y - 0.4f);
            launchedObject.GetComponent<Axe>().DestroyAxe();
        }
        else{
            Debug.Log("Attempt to teleport to axe failed. Axe does not exist!");
        }
    }
}
