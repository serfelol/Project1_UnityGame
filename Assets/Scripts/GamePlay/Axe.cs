using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public delegate void DeatchAction();
    public static event DeatchAction OnAxeDestroyed;

    private Rigidbody2D rb2d;
    private CircleCollider2D ccol;
    [SerializeField] private Transform ccolCenterPoint;
    private GameObject parentGameObject;

    #region Return Axe
    private Transform axeReturnPosition;    // this is normaly the hand of player.
    private float axeReturnVelocity = 5;
    private float checkPositionInterval = 0.1f;
    [SerializeField] private float timeForCatchAnimation;
    private bool isReturning = false;
    private bool beingThrown = false;
    [SerializeField] private float axeRotationSpeed = 180;
    private int facingRight;
    #endregion

    #region On Collision Fields
    private bool hit = false;
    private float penetrationDepth = 0.02f; // when stuck to something
    #endregion

    private void Awake() {
        if(!(rb2d = GetComponent<Rigidbody2D>())){
            Debug.Log("RigidBody2D not found!");
        }
        if(!(ccol = GetComponent<CircleCollider2D>())){
            Debug.Log("CicleCollider not found!");
        }
        rb2d.simulated = false;
        
    }
    private void Update() {
        if(isReturning){
            rb2d.simulated = false;
            Vector2 holdPosition = axeReturnPosition.position;

            float distance = Vector2.Distance(transform.position, axeReturnPosition.position);
            if(distance > checkPositionInterval){
                transform.position = Vector2.MoveTowards(transform.position, holdPosition, axeReturnVelocity * Time.deltaTime);
            }
            else{
                isReturning = false;
                Destroy(gameObject);
                OnAxeDestroyed();
            }
        }

        if(beingThrown){
            transform.Rotate(new Vector3(0,0,-facingRight * axeRotationSpeed * Time.deltaTime));
        }
    }

    public void ThrowAxe(Vector2 force, int direction){
        // so we can add the force with physics.
        rb2d.simulated = true;

        // rotates the object so the axe is facing the right way.
        facingRight = direction;
        if(facingRight == -1){
            Vector3 currentScale = transform.localScale;
            transform.localScale = new Vector3(-currentScale.x, currentScale.y, currentScale.z);
        }

        // add force to the axe to throw.
        rb2d.AddForce(force, ForceMode2D.Impulse);
        beingThrown = true;
    }
    public void ReturnAxe(Transform axeReturnPosition, float axeReturnVelocity){
        this.axeReturnPosition = axeReturnPosition;
        this.axeReturnVelocity = axeReturnVelocity;

        // set returning to true so it will be returned in Update().
        isReturning = true;

        // destroy the axe parent component created when it collides with something.
        if(hit){
            transform.SetParent(null);
            Destroy(parentGameObject);
            hit = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // axe sticks to the surface.
        rb2d.simulated = false;
        hit = true;
        Debug.Log(other.transform.name);

        // creat a intermediary parent so axe wont change in scale by the gameObject it gets stuck.
        if(parentGameObject == null){
            parentGameObject = new GameObject("AxeParent");
            parentGameObject.transform.position = other.transform.position;
            parentGameObject.transform.rotation = other.transform.rotation;
            parentGameObject.transform.parent = other.transform;       
            transform.parent = parentGameObject.transform;
        }

        // get the collider center position.
        float colliderCenterPosX = ccolCenterPoint.transform.position.x;
        float colliderCenterPosY = ccolCenterPoint.transform.position.y;

        // get contact point position.
        float contactPointX = other.GetContact(0).point.x;
        float contactPointY = other.GetContact(0).point.y;

        // calculate the direction of the collision to the axe.
        float collisionDirectionX = contactPointX - colliderCenterPosX;
        float collisionDirectionY = contactPointY - colliderCenterPosY;

        // set the right axe Rotation to stay on wall properly.
        if(facingRight == -1){
            if(collisionDirectionX < -0.1f){
                transform.rotation = Quaternion.Euler(0,20,0);
                transform.position = new Vector2(colliderCenterPosX - penetrationDepth, colliderCenterPosY); 
            }
            else if(collisionDirectionY > 0.1f){
                transform.rotation = Quaternion.Euler(20,0,-90);
                transform.position = new Vector2(colliderCenterPosX, colliderCenterPosY + penetrationDepth); 
            }
            else if(collisionDirectionY < -0.1f){
                transform.rotation = Quaternion.Euler(20,0,90);
                transform.position = new Vector2(colliderCenterPosX , colliderCenterPosY - penetrationDepth); 
            }
        }
        else{
            if(collisionDirectionX > 0.1f){
                transform.rotation = Quaternion.Euler(0,20,0);
                transform.position = new Vector2(colliderCenterPosX + penetrationDepth, colliderCenterPosY); 
            }
            else if(collisionDirectionY > 0.1f){
                transform.rotation = Quaternion.Euler(20,0,90);
                transform.position = new Vector2(colliderCenterPosX, colliderCenterPosY + penetrationDepth); 
            }
            else if(collisionDirectionY < -0.1f){
                transform.rotation = Quaternion.Euler(20,0,-90);
                transform.position = new Vector2(colliderCenterPosX , colliderCenterPosY - penetrationDepth); 
            }
        }

        beingThrown = false;
    }

    public void DestroyAxe(){
        if(parentGameObject != null){
            this.transform.SetParent(null);
            Destroy(parentGameObject);
        }
        Destroy(this.gameObject);
    }
}
