using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCompass : MonoBehaviour
{
    public Transform compass;

    void Start()
    {
        Vector3 point = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width * 0.94f, Screen.height * 0.9f, 4f));
        compass.position = point;
    }
}
