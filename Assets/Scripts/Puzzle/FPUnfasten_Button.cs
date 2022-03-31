using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// button that unfasten the oscillating platform
/// pressed by: everything
/// </summary>
public class FPUnfasten_Button : Button_Click
{
    public static event Action UnfastenFloatingPlatformsEvent;
    private void OnTriggerStay2D(Collider2D other) {
        if(beingUsed == false){
            if(UnfastenFloatingPlatformsEvent != null) UnfastenFloatingPlatformsEvent();
            PhysicallyActivateTheButton();
        }
    }

}
