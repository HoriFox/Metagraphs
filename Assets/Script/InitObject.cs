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

        Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float angle, float t)
        {
            float parabolicT = t * 2 - 1;
            //Когда Y старт и Y конец находятся практически на одном уровне, то упрощаем работу, иначе приходится ложнее.
            if (Mathf.Abs(start.y - end.y) < 0.1f)
            {
                // начало и конец примерно на одном уровне, представьте, что они более простое решение с меньшим количеством шагов
                Vector3 travelDirection = end - start;
                Vector3 result = start + t * travelDirection;
                result += Quaternion.AngleAxis(angle, travelDirection) * new Vector3(0, (-parabolicT * parabolicT + 1) * height, 0);
                return result;

            }
            else
            {
                // начало и конец не уровень, становится все сложнее
                Vector3 travelDirection = end - start;
                Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
                Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
                Vector3 up = Quaternion.AngleAxis(angle, travelDirection) * Vector3.Cross(right, travelDirection);
                if (end.y > start.y) up = -up;
                Vector3 result = start + t * travelDirection;
                result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
                return result;
            }
        }

        [Range(3, 20)]
        public int quality = 10;

        [Range(0, 10)]
        public int height;

        [Range(0, 360)]
        public int angle; // Поворот кривой линии.

        public List<GameObject> InitLine(bool isArc, float heightArc, float angleArc, bool isLGraph, Vector3 positionFirst, Vector3 positionSecond, bool isDirected, Color32 color, string _name, Transform parent = null, bool isSimple = false)
        {
            List<GameObject> gameObject;
            if (isArc)
            {
                gameObject = new List<GameObject>();

                float height = (heightArc == -1) ? Vector3.Distance(positionFirst, positionSecond) * 0.3f : heightArc;
                float angle = (angleArc == -1) ? 0 : angleArc;

                bool isLastPhase = false;
                Vector3 lastPoint = positionFirst;
                for (float i = 1; i < quality + 1; i++)
                {
                    if (i == quality)
                    {
                        isLastPhase = true;
                    }

                    Vector3 nextPoint = SampleParabola(positionFirst, positionSecond, height, angle, i / quality);
                    //Gizmos.color = i % 2 == 0 ? Color.blue : Color.green;
                    //Gizmos.DrawLine(lastP, p);

                    gameObject.AddRange(InitLine(false, lastPoint, nextPoint, isDirected, color, _name, isSimple: true, isLastPart: isLastPhase));

                    lastPoint = nextPoint;
                }
            }
            else
            {
                gameObject = new List<GameObject>();
                gameObject.AddRange(InitLine(isLGraph, positionFirst, positionSecond, isDirected, color, _name, parent, isSimple));
            }
            return gameObject;
        }

        public List<GameObject> InitLine(bool isLGraph, Vector3 positionFirst, Vector3 positionSecond, bool isDirected, Color32 color, string _name, Transform parent = null, bool isSimple = false, bool isLastPart = true)
        {
            Transform parentUse = parent ?? parentStandart;
            List<GameObject> gameObjects = new List<GameObject>();
            if (!isSimple)
            {
                GameObject sphereFirst = Instantiate(resourceM.GetPrefab("SpherePrefab"), positionFirst, Quaternion.identity, parentUse);
                sphereFirst.transform.localScale = (isLGraph) ? scaleLGraph : scaleLink;
                sphereFirst.GetComponent<Renderer>().material.color = new Color32(color.r, color.g, color.b, color.a);
                gameObjects.Add(sphereFirst);

                TooltipText tT = sphereFirst.GetComponent<TooltipText>();
                tT.text = _name;
                GameObject selectMarker = Instantiate(prefabSelectedContainer, sphereFirst.transform);
                selectMarker.transform.localScale = new Vector3(3f, 3f, 3f);
                tT.selectedContainer = selectMarker;

                GameObject sphereSecond = Instantiate(resourceM.GetPrefab("SpherePrefab"), positionSecond, Quaternion.identity, parentUse);
                sphereSecond.transform.localScale = (isLGraph) ? scaleLGraph : scaleLink;
                sphereSecond.GetComponent<Renderer>().material.color = new Color32(color.r, color.g, color.b, color.a);
                gameObjects.Add(sphereSecond);
            }

            GameObject line = Instantiate(resourceM.GetPrefab("LinePrefab"), Vector3.zero, Quaternion.identity, parentUse);
            line.transform.localScale = (isLGraph) ? scaleLGraph : scaleLink;
            line.GetComponent<Renderer>().material.color = new Color32(color.r, color.g, color.b, color.a);

            Vector3 offset = positionSecond - positionFirst;
            Vector3 position = positionFirst + (offset / 2.0f);
            line.transform.position = position;
            line.transform.LookAt(positionFirst);
            Vector3 localScale = line.transform.localScale;
            localScale.z = (positionSecond - positionFirst).magnitude;
            line.transform.localScale = localScale;
            gameObjects.Add(line);

            // Если это последний участок, то разрешаем создать стрелку, если нужно.
            if (isDirected && isLastPart)
            {
                GameObject array = Instantiate(resourceM.GetPrefab("Arrow"), positionSecond, Quaternion.identity, parentUse);
                array.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                array.transform.localRotation = line.transform.localRotation;
                array.transform.Rotate(new Vector3(-90f, 0, 0));
                array.transform.position = GetPositionBetween(positionFirst, positionSecond, 0.35f);
                array.GetComponent<Renderer>().material.color = new Color32(color.r, color.g, color.b, color.a);
                gameObjects.Add(array);
            }

            foreach (var part in gameObjects)
            {
                part.name = _name;
                part.GetComponent<TooltipText>().text = _name;
            }
            return gameObjects;
        }

        public Vector3 GetPositionBetween (Vector3 A, Vector3 B, float Rcb)
        {
            float Rab = Mathf.Sqrt(Mathf.Pow((B.x - A.x), 2)  + Mathf.Pow((B.y - A.y), 2) + Mathf.Pow((B.z - A.z), 2));
            float k = Rcb / Rab;
            float Xc = B.x + (A.x - B.x) * k;
            float Yc = B.y + (A.y - B.y) * k;
            float Zc = B.z + (A.z - B.z) * k;

            return new Vector3(Xc, Yc, Zc);
        }

    }
}
