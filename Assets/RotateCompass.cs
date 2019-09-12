using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCompass : MonoBehaviour
{
    public Transform[] compassObject;

    void FixedUpdate()
    {
        for (int i = 0; i < compassObject.Length; i++)
        {
            compassObject[i].LookAt(-Vector3.forward);
        }
    }
}
