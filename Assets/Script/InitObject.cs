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
            //parentSimple = new GameObject("TestSimple").transform;

            resourceM = ResourceManager.GetInstance();
        }

        [HideInInspector] public ResourceManager resourceM;

        public Transform parentStandart;
        public Vector3 scaleLGraph = new Vector3(0.2f, 0.2f, 0.2f);
        public Vector3 scaleLink = new Vector3(0.05f, 0.05f, 0.05f);

        //Transform parentSimple;

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // БОЛЬШАЯ ЧАСТЬ ЭТОГО КОДА ПОЙДЁТ В МУСОРКУ. ОСТАВИМ ТОЛЬКО ИНИЦИАЛИЗАЦИИ ДЛЯ КЛАССОВ.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //public void Create()
        //{
        //    string line;
        //    System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/Save/simple.txt");
        //    while ((line = file.ReadLine()) != null)
        //    {
        //        String[] words = line.Split(new char[] { ',', '(', ')', '[', ']', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //        if (words[0] == "#") continue;
        //        if (words[0] == "GRAPH")
        //        {
        //            InitGraph(new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4])),
        //                new Color32(byte.Parse(words[5]), byte.Parse(words[6]), byte.Parse(words[7]), 128), words[1], parentSimple);
        //        }
        //        if (words[0] == "LGRAPH")
        //        {
        //            InitLine(true, new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4])),
        //                new Vector3(float.Parse(words[5]), float.Parse(words[6]), float.Parse(words[7])),
        //                new Color32(byte.Parse(words[8]), byte.Parse(words[9]), byte.Parse(words[10]), 128), words[1], parentSimple);
        //        }
        //        if (words[0] == "LINK")
        //        {
        //            InitLine(false, new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4])),
        //                new Vector3(float.Parse(words[5]), float.Parse(words[6]), float.Parse(words[7])),
        //                new Color32(0, 0, 0, 128), words[1], parentSimple);
        //        }
        //    }
        //    file.Close();
        //}
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public GameObject InitGraph(Vector3 position, Color32 color, string _name, Transform parent = null)
        {
            GameObject objectVar;
            Transform parentUse = parent ?? parentStandart;
            objectVar = Instantiate(resourceM.GetPrefab("GraphPrefab"), position, Quaternion.identity, parentUse).gameObject;
            objectVar.GetComponent<Renderer>().material.color = color;
            objectVar.name = _name;
            objectVar.GetComponentInParent<TooltipText>().text = _name;
            return objectVar;
        }

        public List<GameObject> InitLine(bool isLGraph, Vector3 positionFirst, Vector3 positionSecond, Color32 color, string _name, Transform parent = null)
        {
            Transform parentUse = parent ?? parentStandart;
            List<GameObject> gameObjects = CreateLine(isLGraph, positionFirst, positionSecond, color, parentUse);
            foreach (var part in gameObjects)
            {
                part.name = _name;
                part.GetComponentInParent<TooltipText>().text = _name;
            }
            return gameObjects;
        }

        private List<GameObject> CreateLine(bool isLGraph, Vector3 firstPoint, Vector3 secondPoint, Color32 color, Transform parent)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            gameObjects.Add(InitSphere(isLGraph, firstPoint, color, parent));
            gameObjects.Add(InitSphere(isLGraph, secondPoint, color, parent));
            gameObjects.Add(InitLine(isLGraph, gameObjects[0].transform.position, gameObjects[1].transform.position, color, parent));
            return gameObjects;
        }
        private GameObject InitSphere(bool isLGraph, Vector3 point, Color32 color, Transform parent)
        {
            GameObject sphere = Instantiate(resourceM.GetPrefab("SpherePrefab"), point, Quaternion.identity, parent);
            sphere = SetProperties(isLGraph, sphere, color);
            return sphere;
        }
        private GameObject InitLine(bool isLGraph, Vector3 beginPoint, Vector3 endPoint, Color32 color, Transform parent)
        {
            GameObject line = Instantiate(resourceM.GetPrefab("LinePrefab"), Vector3.zero, Quaternion.identity, parent);
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
