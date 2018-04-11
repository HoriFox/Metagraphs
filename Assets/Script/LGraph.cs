using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGraph : MonoBehaviour {

    bool showInfoObject = false;

    Vector3 screenPos;
    public GUIStyle customButton;

    void OnGUI()
    {
        if (showInfoObject)
        {
            GUI.Box(new Rect(screenPos.x + 1, screenPos.y + 1, 200, 50), "LGRAPH \nName: " + name + "\nConnects: in developing", customButton);
        }
    }

    private void OnMouseEnter()
    {
        if (showInfoObject == false)
        {
            screenPos = Input.mousePosition;
            screenPos.y = Screen.height - screenPos.y;
            showInfoObject = true;
        }
    }

    private void OnMouseExit()
    {
        showInfoObject = false;
    }
}
