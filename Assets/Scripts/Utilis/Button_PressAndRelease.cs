using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Executes actions when activated and when deactivated. Maintains reference to its current state.
/// To be used with Triggers.
/// </summary>
public class Button_PressAndRelease : MonoBehaviour
{
    #region fields
    [SerializeField] private float depthWhenClicked = 0.04f;
    protected bool isBeingPressed;
    protected bool wasButtonActivated;
    #endregion

    protected void PhysicallyPressTheButton(){
        transform.GetChild(0).transform.position = new Vector2(transform.position.x, transform.position.y - depthWhenClicked);
    }

    protected void PhysicallyReleaseTheButton(){
        transform.GetChild(0).transform.position = new Vector2(transform.position.x, transform.position.y);
    }
}
