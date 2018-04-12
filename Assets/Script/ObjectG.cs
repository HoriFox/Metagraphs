using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectG : MonoBehaviour {

    String[] names;
    Vector3 position;
    bool showInfoObject = false;

    Vector3 screenPos;
    public GUIStyle customButton;

    private void Start()
    {
        names = name.Split(new char[] { '[', ']', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        position = transform.position;
    }

    void OnGUI() {
        if (showInfoObject)
        {
            if (names[0] == "LGRAPH" || names[0] == "LINK")
            {
                GUI.Box(new Rect(screenPos.x + 1, screenPos.y + 1, 200, 50), names[0] + " \nName: " + names[1] + "\nConnects: in developing", customButton);
            } else if (names[0] == "GRAPH")
            {
                GUI.Box(new Rect(screenPos.x + 1, screenPos.y + 1, 200, 50), names[0] + " \nName: " + names[1] + "\nPosition: x: " + position.x.ToString()
               + " y: " + position.y.ToString() + " z: " + position.z.ToString(), customButton);
            }
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
