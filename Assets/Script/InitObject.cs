using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class InitObject : MonoBehaviour
    {
        public static InitObject Instance;
        public GameObject prefabSelectedContainer;

        private void Start()
        {
            Instance = this;
            resourceM = ResourceManager.GetInstance();
        }

        [HideInInspector] public ResourceManager resourceM;

        public Transform parentStandart;
        public Vector3 scaleLGraph = new Vector3(0.2f, 0.2f, 0.2f);
        public Vector3 scaleLink = new Vector3(0.05f, 0.05f, 0.05f);

        public GameObject InitGraph(Vector3 position, Vector3 _scale, Color32 _color, string _name, Transform parent = null/*, bool Style3D = true*/)
        {
            Transform parentUse = parent ?? parentStandart;
            //string namePrefabObject = (Style3D) ? "GraphPrefab" : "2DVertexPrefab";
            GameObject objectVar = Instantiate(resourceM.GetPrefab("GraphPrefab"), position, Quaternion.identity, parentUse).gameObject;
            objectVar.transform.localScale = _scale;
            objectVar.GetComponent<Renderer>().material.color = _color;
            objectVar.name = _name;

            TooltipText tT = objectVar.GetComponent<TooltipText>();
            tT.text = _name;
            tT.selectedContainer = Instantiate(prefabSelectedContainer, objectVar.transform);

            objectVar.GetComponentInChildren<TextMesh>().text = _name;
            // Расчёт степени контрастности и соответствующего цвета.
            objectVar.GetComponentInChildren<TextMesh>().color = (_color.r * 0.299 + _color.g * 0.587 + _color.b * 0.114 <= 140) ? Color.white : Color.black;

            return objectVar;
        }

        private Vector3 GetPerpendicular(Vector3 inputVector, float factorAdditional)
        {
            Vector3 rotate = new Vector3(-inputVector.y, inputVector.x, inputVector.z);
            rotate = rotate.normalized * factorAdditional;
            Vector3 offset = rotate + inputVector / 2;
            return offset;
        }

        public Vector3 additional = new Vector3();

        [Range(3, 20)]
        public int quality = 6;

        [Range(-10.0f, 10.0f)]
        public float factorAdditional = 5.0f;

        public List<GameObject> InitLine(bool isArc, bool isLGraph, Vector3 positionFirst, Vector3 positionSecond, Color32 color, string _name, Transform parent = null, bool isSimple = false)
        {
            List<GameObject> gameObject;
            if (isArc)
            {
                gameObject = new List<GameObject>();

                float dividerPhase = 1.0f / quality;
                float currentPhase = 0.0f;

                additional = GetPerpendicular(positionSecond - positionFirst, factorAdditional) + positionFirst;

                Vector3 lastPoint = positionFirst;

                int numberPhase = 0;
                while (numberPhase <= quality)
                {
                    Vector3 m1 = Vector3.Lerp(positionFirst, additional, currentPhase);
                    Vector3 m2 = Vector3.Lerp(additional, positionSecond, currentPhase);

                    Vector3 nextPoint = Vector3.Lerp(m1, m2, currentPhase);
                    if (numberPhase != 0)
                    {
                        // "ArcEdge_" + _name + "_" + numberPhase
                        gameObject.AddRange(InitLine(false, lastPoint, nextPoint, color, _name, isSimple: true));
                    }

                    lastPoint = nextPoint;

                    currentPhase += dividerPhase;
                    numberPhase++;
                }
            }
            else
            {
                gameObject = new List<GameObject>();
                gameObject.AddRange(InitLine(isLGraph, positionFirst, positionSecond, color, _name, parent, isSimple));
            }
            return gameObject;
        }

        public List<GameObject> InitLine(bool isLGraph, Vector3 positionFirst, Vector3 positionSecond, Color32 color, string _name, Transform parent = null, bool isSimple = false)
        {
            Transform parentUse = parent ?? parentStandart;
            List<GameObject> gameObjects = CreateLine(isLGraph, positionFirst, positionSecond, color, _name, parentUse, isSimple);
            foreach (var part in gameObjects)
            {
                part.name = _name;
                part.GetComponent<TooltipText>().text = _name;
            }
            return gameObjects;
        }

        private List<GameObject> CreateLine(bool isLGraph, Vector3 firstPoint, Vector3 secondPoint, Color32 color, string _name, Transform parent, bool isSimple)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            if (!isSimple)
            {
                gameObjects.Add(InitSphere(isLGraph, firstPoint, color, parent));
                gameObjects.Add(InitSphere(isLGraph, secondPoint, color, parent));
            }
            gameObjects.Add(InitOneLine(isLGraph, firstPoint, secondPoint, color, _name, parent));
            return gameObjects;
        }
        private GameObject InitSphere(bool isLGraph, Vector3 point, Color32 color, Transform parent)
        {
            GameObject sphere = Instantiate(resourceM.GetPrefab("SpherePrefab"), point, Quaternion.identity, parent);
            sphere = SetProperties(isLGraph, sphere, color);
            return sphere;
        }
        private GameObject InitOneLine(bool isLGraph, Vector3 beginPoint, Vector3 endPoint, Color32 color, string _name, Transform parent)
        {
            GameObject line = Instantiate(resourceM.GetPrefab("LinePrefab"), Vector3.zero, Quaternion.identity, parent);
            line = SetProperties(isLGraph, line, color);
            //line.GetComponentInChildren<TextMesh>().text = _name;
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
