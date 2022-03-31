using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It works on click and just has the event to do something on that action.
/// To be used with Triggers.
/// </summary>
public class Button_Click : MonoBehaviour
{
    #region fields
    [SerializeField] private float depthWhenClicked;
    [SerializeField] private float clickDuration = 0.5f;
    protected bool beingUsed; // to be used as OnTriggerStay2D restriction.
    #endregion
    
    private void Awake() {
        beingUsed = false;
    }

    protected void PhysicallyActivateTheButton(){
        transform.GetChild(0).transform.position = new Vector2(transform.position.x, transform.position.y - depthWhenClicked);
        beingUsed = true;
        StartCoroutine(WaitForRelease());
    }

    // wait for button reset to unpressed position
    private IEnumerator WaitForRelease(){
        // TODO: I can put a transition to button movement with Mathf.Lerp().
        yield return new WaitForSeconds(clickDuration);
        transform.GetChild(0).transform.position = new Vector2(transform.position.x, transform.position.y);
        beingUsed = false;
    }
}
