using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {

    Vector3 position;
    bool showInfoObject = false;

    Vector3 screenPos;
    public GUIStyle customButton;

    private void Start()
    {
        position = transform.position;
    }

    void OnGUI() {
        if (showInfoObject)
        {
            GUI.Box(new Rect(screenPos.x + 1, screenPos.y + 1, 200, 50), "GRAPH \nName: " + name + "\nPosition: x: " + position.x.ToString()
               + " y: " + position.y.ToString() + " z: " + position.z.ToString(), customButton);
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
