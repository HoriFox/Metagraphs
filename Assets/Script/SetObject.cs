using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SetObject : MonoBehaviour {

    [SerializeField]
    private Transform linePrefab;
    [SerializeField]
    private Transform graphPrefab;
    [SerializeField]
    private Vector3 scaleLGraph = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField]
    private Vector3 scaleLink = new Vector3(0.05f, 0.05f, 0.05f);

    private GameObject sphere;
    private GameObject leftSphere;
    private GameObject rightSphere;
    private GameObject line;

    private Transform[] arrayGraph = new Transform[200];
    private Transform[] arrayLine = new Transform[50];

    public void Create()
    {
        int indexGraph = 0;
        int indexLine = 0;
        float os_x = 0;
        float os_y = 0;
        float os_z = 0;
        string line;
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/Save/simple.txt");
        while ((line = file.ReadLine()) != null)
        {
            String[] words = line.Split(new char[] { ',', '(', ')', '[', ']', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words[0] == "#") continue;
            if (words[0] == "OFFSET")
            {
                if (words.Length > 1) os_x = float.Parse(words[1]);
                if (words.Length > 2) os_y = float.Parse(words[2]);
                if (words.Length > 3) os_z = float.Parse(words[3]);
                continue; //[!]
            }
            if (words[0] == "GRAPH")
            {
                Vector3 pos = new Vector3(float.Parse(words[2]) + os_x, float.Parse(words[3]) + os_y, float.Parse(words[4]) + os_z);
                Color32 color = new Color32(byte.Parse(words[5]), byte.Parse(words[6]), byte.Parse(words[7]), 255);
                arrayGraph[indexGraph] = Instantiate(graphPrefab, pos, Quaternion.identity);
                arrayGraph[indexGraph].GetComponent<Renderer>().material.color = color;
                arrayGraph[indexGraph].name = words[1];
                indexGraph++;
                continue; //[!]
            }
            if (words[0] == "LGRAPH")
            {
                Vector3 firstPos = new Vector3(float.Parse(words[2]) + os_x, float.Parse(words[3]) + os_y, float.Parse(words[4]) + os_z);
                Vector3 secondPos = new Vector3(float.Parse(words[5]) + os_x, float.Parse(words[6]) + os_y, float.Parse(words[7]) + os_z);
                Color32 color = new Color32(byte.Parse(words[8]), byte.Parse(words[9]), byte.Parse(words[10]), 255);
                arrayLine[indexLine] = CreateLine(true, firstPos, secondPos, color).GetComponent<Transform>();
                arrayLine[indexLine].name = words[1];
                indexLine++;
                continue; //[!]
            }
            if (words[0] == "LINK")
            {
                Vector3 firstPos = new Vector3(float.Parse(words[2]) + os_x, float.Parse(words[3]) + os_y, float.Parse(words[4]) + os_z);
                Vector3 secondPos = new Vector3(float.Parse(words[5]) + os_x, float.Parse(words[6]) + os_y, float.Parse(words[7]) + os_z);
                Color32 color = new Color32(0, 0, 0, 255);
                arrayLine[indexLine] = CreateLine(false, firstPos, secondPos, color).GetComponent<Transform>();
                arrayLine[indexLine].name = words[1];
                indexLine++;
                continue; //[!]
            }
        }
        file.Close();
    }

    public GameObject CreateLine(bool isLGraph, Vector3 firstPoint, Vector3 secondPoint, Color32 color)
    {
        leftSphere = InitSphere(isLGraph, firstPoint, color);
        rightSphere = InitSphere(isLGraph, secondPoint, color);

        return InitLine(isLGraph, linePrefab, leftSphere.transform.position, rightSphere.transform.position, color);
    }

    private GameObject InitSphere(bool isLGraph, Vector3 point, Color32 color)
    {
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = point;
        sphere = SetProperties(isLGraph, sphere, color);

        return sphere;
    }

    private GameObject InitLine(bool isLGraph, Transform linePrefab, Vector3 beginPoint, Vector3 endPoint, Color32 color)
    {
        line = Instantiate<GameObject>(linePrefab.gameObject, Vector3.zero, Quaternion.identity);
        line = SetProperties(isLGraph, line, color);

        UpdateLinePosition(line, beginPoint, endPoint);

        return line;
    }

    private GameObject SetProperties(bool isLGraph, GameObject obj, Color32 color)
    {
        if (isLGraph)
            obj.transform.localScale = scaleLGraph;
        else
            obj.transform.localScale = scaleLink;
        obj.GetComponent<Renderer>().material.color = new Color32(color.r, color.g, color.b, color.a);

        return obj;
    }

    private void UpdateLinePosition(GameObject lineGraph, Vector3 beginPoint, Vector3 endPoint)
    {
        Vector3 offset = endPoint - beginPoint;
        Vector3 position = beginPoint + (offset / 2.0f);

        lineGraph.transform.position = position;
        lineGraph.transform.LookAt(beginPoint);
        Vector3 localScale = lineGraph.transform.localScale;
        localScale.z = (endPoint - beginPoint).magnitude;
        lineGraph.transform.localScale = localScale;
    }
}
