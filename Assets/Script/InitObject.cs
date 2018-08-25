using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace nm
{
    public class InitObject : MonoBehaviour
    {
        public static InitObject Instance;

        private void Start()
        {
            Instance = this;
        }

        public Transform parent;
        public Transform linePrefab;
        public Transform graphPrefab;
        public Transform spherePrefab;
        public Vector3 scaleLGraph = new Vector3(0.2f, 0.2f, 0.2f);
        public Vector3 scaleLink = new Vector3(0.05f, 0.05f, 0.05f);
        //private Dictionary<string, Transform> arrayObject = new Dictionary<string, Transform>();

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // БОЛЬШАЯ ЧАСТЬ ЭТОГО КОДА ПОЙДЁТ В МУСОРКУ. ОСТАВИМ ТОЛЬКО ИНИЦИАЛИЗАЦИИ ДЛЯ КЛАССОВ.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void Create()
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/Save/simple.txt");
            while ((line = file.ReadLine()) != null)
            {
                String[] words = line.Split(new char[] { ',', '(', ')', '[', ']', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (words[0] == "#") continue;
                if (words[0] == "GRAPH")
                {
                    InitGraph(new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4])), 
                        new Color32(byte.Parse(words[5]), byte.Parse(words[6]), byte.Parse(words[7]), 128), words[1]);
                }
                if (words[0] == "LGRAPH")
                {
                    InitLGraph(new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4])),
                        new Vector3(float.Parse(words[5]), float.Parse(words[6]), float.Parse(words[7])),
                        new Color32(byte.Parse(words[8]), byte.Parse(words[9]), byte.Parse(words[10]), 128), words[1]);
                }
                if (words[0] == "LINK")
                {
                    InitLink(new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4])),
                        new Vector3(float.Parse(words[5]), float.Parse(words[6]), float.Parse(words[7])),
                        new Color32(0, 0, 0, 128), words[1]);
                }
            }
            file.Close();
        }

        public Transform InitGraph(Vector3 position, Color32 color, string name)
        {
            Transform objectVar;
            string nameObject = PredicateList.NameSystem.GetName("GRAPH");
            objectVar = Instantiate(graphPrefab, position, Quaternion.identity, parent);
            objectVar.GetComponent<Renderer>().material.color = color;
            objectVar.name = "[GRAPH] " + name;
            objectVar.GetComponentInParent<TooltipText>().text = nameObject;
            return objectVar;
        }

        public Transform InitLGraph(Vector3 positionFirst, Vector3 positionSecond, Color32 color, string name)
        {
            Transform objectVar;
            string nameObject = PredicateList.NameSystem.GetName("LGRAPH");
            objectVar = CreateLine(true, positionFirst, positionSecond, color).GetComponent<Transform>();
            //objectVar.GetComponent<Renderer>().material.color = color;
            objectVar.name = "[LGRAPH] " + name;
            objectVar.GetComponentInParent<TooltipText>().text = nameObject;
            return objectVar;
        }

        public Transform InitLink(Vector3 positionFirst, Vector3 positionSecond, Color32 color, string name)
        {
            Transform objectVar;
            string nameObject = PredicateList.NameSystem.GetName("LINK");
            objectVar = CreateLine(true, positionFirst, positionSecond, color).GetComponent<Transform>();
            //objectVar.GetComponent<Renderer>().material.color = color;
            objectVar.name = "[LINK] " + name;
            objectVar.GetComponentInParent<TooltipText>().text = nameObject;
            return objectVar;
        }

        public GameObject CreateLine(bool isLGraph, Vector3 firstPoint, Vector3 secondPoint, Color32 color)
        {
            GameObject leftSphere = InitSphere(isLGraph, firstPoint, color);
            GameObject rightSphere = InitSphere(isLGraph, secondPoint, color);
            return InitLine(isLGraph, linePrefab, leftSphere.transform.position, rightSphere.transform.position, color);
        }
        private GameObject InitSphere(bool isLGraph, Vector3 point, Color32 color)
        {
            GameObject sphere = Instantiate<GameObject>(spherePrefab.gameObject, point, Quaternion.identity, parent);
            sphere = SetProperties(isLGraph, sphere, color);
            return sphere;
        }
        private GameObject InitLine(bool isLGraph, Transform linePrefab, Vector3 beginPoint, Vector3 endPoint, Color32 color)
        {
            GameObject line = Instantiate<GameObject>(linePrefab.gameObject, Vector3.zero, Quaternion.identity, parent);
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
