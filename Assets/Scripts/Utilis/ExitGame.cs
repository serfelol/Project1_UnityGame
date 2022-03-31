using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Debug.Log("oi");
            Application.Quit();
        }       
    }
}
