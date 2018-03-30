using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTestGraph : MonoBehaviour {

    public Transform graphPrefab;

    private float R = 0, G = 0, B = 0, A = 255; // Effect test
    private float newX;
    private int indexArray = 0;
    private Transform[] arrayGraph = new Transform[200];

    void Start ()
    {
        for (int x = 0; x < 10; x++)
        {
            G += 25.5f;
            B += 25.5f;
            newX = x * 0.5f + 3.2f;
            if (x == 5) A = 128;
            for (int y = 0; y < 10; y++)
            {
                R += 25.5f;
                arrayGraph[indexArray] = Instantiate(graphPrefab, new Vector3(newX, y * 0.5f - 2.2f, 4.5f), Quaternion.identity);
                arrayGraph[indexArray].GetComponent<Renderer>().material.color = new Color32((byte)R, (byte)G, (byte)B, (byte)A);
                indexArray++;
            }
            R = 0;
        }
    }

}