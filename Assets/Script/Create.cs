using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Create : MonoBehaviour
{
    [SerializeField]
    private Transform linePrefab;
    [SerializeField]
    public Transform graphPrefab;
    [SerializeField]
    private Vector3 scaleLine = new Vector3(0.2f, 0.2f, 0.2f);

    private GameObject leftSphere;
    private GameObject rightSphere;
    private GameObject cylinder;

    private float R = 0, G = 0, B = 0, A = 255; // Effect test
    private float newX;
    private int indexGraph = 0;
    private int indexLine = 0;
    private Transform[] arrayGraph = new Transform[200];
    private Transform[] arrayLine = new Transform[20];

    void Start()
    {
        string line;
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/Save/simple.txt");
        while ((line = file.ReadLine()) != null)
        {
            String[] words = line.Split(new char[] { ',', '(', ')', '[', ']', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words[0] == "GRAPH")
            {
                arrayGraph[indexGraph] = Instantiate(graphPrefab, new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4])), Quaternion.identity);
                arrayGraph[indexGraph].GetComponent<Renderer>().material.color = new Color32(byte.Parse(words[5]), byte.Parse(words[6]), byte.Parse(words[7]), 255);
                indexGraph++;
                print(words[0]);
            }
            if (words[0] == "LINK")
            {
                GameObject newline = CreateLine(new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4])), new Vector3(float.Parse(words[5]), float.Parse(words[6]), float.Parse(words[7])));
                arrayLine[indexGraph] = newline.GetComponent<Transform>();
                indexLine++;
            }
        }
        file.Close();

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
    }

    GameObject CreateLine(Vector3 firstPoint, Vector3 secondPoint)
    {
        leftSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftSphere.transform.position = firstPoint;
        rightSphere.transform.position = secondPoint;
        leftSphere.transform.localScale = scaleLine;
        rightSphere.transform.localScale = scaleLine;

        return InstantiateCylinder(linePrefab, leftSphere.transform.position, rightSphere.transform.position);
    }

    GameObject InstantiateCylinder(Transform cylinderPrefab, Vector3 beginPoint, Vector3 endPoint)
    {
        cylinder = Instantiate<GameObject>(cylinderPrefab.gameObject, Vector3.zero, Quaternion.identity);
        cylinder.transform.localScale = scaleLine;

        UpdateLinePosition(cylinder, beginPoint, endPoint);

        return cylinder;
    }

    private void UpdateLinePosition(GameObject cylinder, Vector3 beginPoint, Vector3 endPoint)
    {
        Vector3 offset = endPoint - beginPoint;
        Vector3 position = beginPoint + (offset / 2.0f);

        cylinder.transform.position = position;
        cylinder.transform.LookAt(beginPoint);
        Vector3 localScale = cylinder.transform.localScale;
        localScale.z = (endPoint - beginPoint).magnitude;
        cylinder.transform.localScale = localScale;
    }
}