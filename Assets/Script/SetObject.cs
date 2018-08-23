using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace nm
{
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // БОЛЬШАЯ ЧАСТЬ ЭТОГО КОДА ПОЙДЁТ В МУСОРКУ. ОСТАВИМ ТОЛЬКО ИНИЦИАЛИЗАЦИИ ДЛЯ КЛАССОВ.
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public class SetObject : MonoBehaviour
    {

        public Transform parent;
        public Transform linePrefab;
        public Transform graphPrefab;
        public Transform spherePrefab;
        public Vector3 scaleLGraph;
        public Vector3 scaleLink;

        private GameObject sphere;
        private GameObject leftSphere;
        private GameObject rightSphere;
        private GameObject line;

        private Dictionary<string, Transform> arrayObject;


        public void Awake()
        {
            scaleLGraph = new Vector3(0.2f, 0.2f, 0.2f);
            scaleLink = new Vector3(0.05f, 0.05f, 0.05f);

            arrayObject = new Dictionary<string, Transform>();
        }

        public void Create()
        {
            string nameObject;
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/Save/simple.txt");
            while ((line = file.ReadLine()) != null)
            {
                String[] words = line.Split(new char[] { ',', '(', ')', '[', ']', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (words[0] == "#")
                {
                    continue;
                }
                if (words[0] == "GRAPH")
                {
                    Vector3 pos = new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]));
                    Color32 color = new Color32(byte.Parse(words[5]), byte.Parse(words[6]), byte.Parse(words[7]), 128);
                    nameObject = PredicateList.NameSystem.GetName("GRAPH");
                    arrayObject[nameObject] = Instantiate(graphPrefab, pos, Quaternion.identity, parent);
                    arrayObject[nameObject].GetComponent<Renderer>().material.color = color;
                    arrayObject[nameObject].name = "[" + words[0] + "] " + words[1];
                    arrayObject[nameObject].GetComponentInParent<TooltipText>().text = nameObject;
                    continue; //[!]
                }
                if (words[0] == "LGRAPH")
                {
                    Vector3 firstPos = new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]));
                    Vector3 secondPos = new Vector3(float.Parse(words[5]), float.Parse(words[6]), float.Parse(words[7]));
                    Color32 color = new Color32(byte.Parse(words[8]), byte.Parse(words[9]), byte.Parse(words[10]), 128);
                    nameObject = PredicateList.NameSystem.GetName("LGRAPH");
                    arrayObject[nameObject] = CreateLine(true, firstPos, secondPos, color).GetComponent<Transform>();
                    arrayObject[nameObject].name = "[" + words[0] + "] " + words[1];
                    arrayObject[nameObject].GetComponentInParent<TooltipText>().text = nameObject;
                    continue; //[!]
                }
                if (words[0] == "LINK")
                {
                    Vector3 firstPos = new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]));
                    Vector3 secondPos = new Vector3(float.Parse(words[5]), float.Parse(words[6]), float.Parse(words[7]));
                    Color32 color = new Color32(0, 0, 0, 128);
                    nameObject = PredicateList.NameSystem.GetName("LINK");
                    arrayObject[nameObject] = CreateLine(false, firstPos, secondPos, color).GetComponent<Transform>();
                    arrayObject[nameObject].name = "[" + words[0] + "] " + words[1];
                    arrayObject[nameObject].GetComponentInParent<TooltipText>().text = nameObject;
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
            sphere = Instantiate<GameObject>(spherePrefab.gameObject, point, Quaternion.identity, parent);
            sphere = SetProperties(isLGraph, sphere, color);

            return sphere;
        }
        private GameObject InitLine(bool isLGraph, Transform linePrefab, Vector3 beginPoint, Vector3 endPoint, Color32 color)
        {
            line = Instantiate<GameObject>(linePrefab.gameObject, Vector3.zero, Quaternion.identity, parent);
            line = SetProperties(isLGraph, line, color);

            UpdateLinePosition(line, beginPoint, endPoint);

            return line;
        }
        private GameObject SetProperties(bool isLGraph, GameObject obj, Color32 color)
        {
            obj.transform.localScale = (isLGraph) ? scaleLGraph : scaleLink;
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
}
