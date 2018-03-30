using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTestLine : MonoBehaviour {

    public Transform linePrefab;

    private Transform[] arrayLine = new Transform[10];

    void Start()
    {
        for (int index = 0; index < 10; index++)
        {
            arrayLine[index] = Instantiate(linePrefab);

            LineRenderer line = arrayLine[index].GetComponent<LineRenderer>();
            line.SetPosition(0, new Vector3(-0.5f * index - 2.5f, -2.5f , 4.5f));
            line.SetPosition(1, new Vector3(-0.5f * index - 2.5f, 0.25f * index, 4.5f)); //Random.Range(-3.0f, 3.0f)

            arrayLine[index].GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 128);
        }
    }
}
