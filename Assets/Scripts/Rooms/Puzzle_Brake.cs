using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_Brake : MonoBehaviour
{
    [SerializeField] private bool brakeState;
    private Vector2 brakeDiactivatedRotation;
    private Vector2 brakeActivatedRotation;
    private float rotationSpeed = 2;

    private void Awake() {
        brakeActivatedRotation = Vector2.zero;
        brakeDiactivatedRotation = new Vector2(0,90);
        transform.rotation = Quaternion.Euler(brakeDiactivatedRotation);
        brakeState = false;
    }

    public bool BrakeState{
        get{
            return brakeState;
        }
        set{
            brakeState = value;
        }
    }
    
    private void Update() {
        if( brakeState && transform.rotation.y > Quaternion.Euler(brakeActivatedRotation).y ){
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y - rotationSpeed * Time.deltaTime, 0);
            if( transform.rotation.y <= Quaternion.Euler(brakeActivatedRotation).y ){
                transform.rotation = Quaternion.Euler(brakeActivatedRotation);
            }
        }
        else if( !brakeState && transform.rotation.y < Quaternion.Euler(brakeDiactivatedRotation).y ){
            transform.rotation = Quaternion.Euler(brakeDiactivatedRotation);
            if( transform.rotation.y >= Quaternion.Euler(brakeDiactivatedRotation).y ){
                transform.rotation = Quaternion.Euler(brakeDiactivatedRotation);
            }
        }
    }
}
