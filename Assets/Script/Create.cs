using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour
{

    public Transform graphPrefab;
    public Transform linePrefab;

    private float R = 0, G = 0, B = 0, A = 255; // Effect test
    private float newX;
    private int indexGraph = 0;
    private int indexLine = 0;
    private Transform[] arrayGraph = new Transform[110];
    private Transform[] arrayLine = new Transform[20];

    void Start()
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
                arrayGraph[indexGraph] = Instantiate(graphPrefab, new Vector3(newX, y * 0.5f - 2.2f, 4.5f), Quaternion.identity);
                arrayGraph[indexGraph].GetComponent<Renderer>().material.color = new Color32((byte)R, (byte)G, (byte)B, (byte)A);
                indexGraph++;
            }
            R = 0;
        }

        for (indexLine = 0; indexLine < 10; indexLine++)
        {
            arrayLine[indexLine] = Instantiate(linePrefab);

            LineRenderer line = arrayLine[indexLine].GetComponent<LineRenderer>();
            line.SetPosition(0, new Vector3(-0.5f * indexLine - 2.5f, -2.5f, 4.5f));
            line.SetPosition(1, new Vector3(-0.5f * indexLine - 2.5f, 0.25f * indexLine, 4.5f));

            arrayLine[indexLine].GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        }


        for (int i = 0; i < 10; i++)
        {
            arrayGraph[indexGraph] = Instantiate(graphPrefab, new Vector3(Random.Range(-2f, 2f), Random.Range(3f, 6f), Random.Range(5f, 9f)), Quaternion.identity);
            arrayGraph[indexGraph].GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
            indexGraph++;
        }

        for (int indexLine = 11; indexLine < 20; indexLine++)
        {
            arrayLine[indexLine] = Instantiate(linePrefab);

            LineRenderer line = arrayLine[indexLine].GetComponent<LineRenderer>();

            line.SetPosition(0, new Vector3(arrayGraph[89 + indexLine].GetComponent<Transform>().position.x,
                                            arrayGraph[89 + indexLine].GetComponent<Transform>().position.y,
                                            arrayGraph[89 + indexLine].GetComponent<Transform>().position.z));

            line.SetPosition(1, new Vector3(arrayGraph[90 + indexLine].GetComponent<Transform>().position.x,
                                            arrayGraph[90 + indexLine].GetComponent<Transform>().position.y,
                                            arrayGraph[90 + indexLine].GetComponent<Transform>().position.z));

            arrayLine[indexLine].GetComponent<Renderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        }
    }

    void Update()
    {

    }
}