using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axles : MonoBehaviour
{
    public LineRenderer x;
    public LineRenderer y;
    public LineRenderer z;
    public float widthLineFactor = 0.005f;

    void Update () {
        float distance = Camera.main.transform.position.magnitude;
        float width = distance * widthLineFactor;
        x.widthMultiplier = width;
        y.widthMultiplier = width;
        z.widthMultiplier = width;
    }
}
