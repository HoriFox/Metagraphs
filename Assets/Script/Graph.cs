using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {

    Renderer rend;
    Color savedColode;

    private void Start()
    {
        rend = this.GetComponent<Renderer>();
        savedColode = rend.material.color;
    }

    private void OnMouseEnter()
    {
        rend.material.color = new Color32((byte)(savedColode.r * 255), (byte)(savedColode.g * 255), (byte)(savedColode.b * 255), 255);
    }

    private void OnMouseExit()
    {
        rend.material.color = savedColode;
    }

}
