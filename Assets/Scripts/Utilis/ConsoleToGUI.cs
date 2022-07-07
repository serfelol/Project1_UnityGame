using UnityEngine;
    
public class ConsoleToGUI : MonoBehaviour
{
//#if !UNITY_EDITOR
    static string myLog = "";
    private string output;
    private string stack;
    bool doShow = false;

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void Update() { if (Input.GetKeyDown(KeyCode.P)) { doShow = !doShow; } }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
    }

    void OnGUI()
    {
        if (!doShow) { return; }
        else{
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            {
                myLog = GUI.TextArea(new Rect(Screen.width * 2/3, Screen.height * 2/3, Screen.width / 3, Screen.height / 3), myLog);
            }
        }
    }
//#endif
//https://answers.unity.com/questions/125049/is-there-any-way-to-view-the-console-in-a-build.html
}