using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class PredicateList : MonoBehaviour
    {
        // Система даёт индивидуальный индекс имени.
        public static class NameSystem
        {
            public static Dictionary<string, int> nameDict = new Dictionary<string, int>();

            public static string GetName(string type)
            {
                string name = "error";
                if (nameDict.ContainsKey(type))
                    nameDict[type]++;
                else
                    nameDict.Add(type, 1);

                name = type + nameDict[type];
                return name;
            }

            public static void RemoveLastIndex(string type)
            {
                nameDict[type]--;
            }
        }

        // Все элементы описания.
        public class Predicate
        {
            public virtual Vector3 FixingPoint { get; set; }
            //public virtual Transform ObjectTransform { get; set; }
            public virtual Dictionary<int, Transform> ObjectsTransform { get; set; }
            public virtual string Name { get; set; }
            public virtual void OutLog() { }
        }

        /// <summary>
        /// Вершина/Метавершина.
        /// Важно учитывать, что у класса вершины есть Dictionary, который очень чувствителен
        /// к дублирующимся custom именам. 
        /// </summary>
        public class Vertex : Predicate
        {
            public override Vector3 FixingPoint { get; set; }
            //public override Transform ObjectTransform { get; set; }
            public override Dictionary<int, Transform> ObjectsTransform { get; set; }
            // Название объекта.
            public override string Name { get; set; }
            // Признак Мета принадлежности.
            public bool MetaType;
            // Подчинённые предикаты.
            public Dictionary<string, Predicate> Predicates;

            public Vertex(string name, Dictionary<string, Predicate> predicates = null, bool metatype = false)
            {
                ObjectsTransform = new Dictionary<int, Transform>();
                MetaType = metatype;
                Predicates = predicates;
                Name = name;
                Create();
                OutLog();
            }
            public void Create()
            {
                // Позиция центра вершины-связи и сферы.
                FixingPoint = new Vector3(Random.Range(-1.18f, 1.18f), Random.Range(-1f, 1f), Random.Range(0f, 4f)); // TO DO [ИЗ ЛОГИКИ]
                if (Predicates != null && Predicates.Count >= 1)
                {
                    // Вершина-связь.
                    Vector3 positionChild = new Vector3();
                    Color32 color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 128);

                    int n = 0;
                    foreach (var Predicate in Predicates)
                    {
                        // Не делаем связь с Edge и MetaEdge
                        if (Predicate.Value is Edge) continue;
                        positionChild = Predicate.Value.FixingPoint;
                        ObjectsTransform[n] = InitObject.Instance.InitLine(true, FixingPoint, positionChild, color, Name);
                        n++;
                    }
                }
                else
                {
                    // Вершина-сфера.
                    ObjectsTransform[0] = InitObject.Instance.InitGraph(FixingPoint, new Color32(0, 0, 0, 128), Name);
                }
            }
            public override void OutLog()
            {
                string NameObject = (MetaType == false) ? "Vertex" : "Metavertex";
                string output = "<b>" + NameObject + " |</b> Name: " + Name + " | HasChild: "
                    + ((Predicates != null && Predicates.Count != 0) ? "True" : "False");
                Debug.Log(output);
                if (Predicates != null && Predicates.Count != 0)
                {
                    output += "\nChildren:";
                    foreach (var Predicate in Predicates)
                    {
                        Debug.Log("\t └> " + Predicate.Value.Name);
                        output += "\n" + Predicate.Value.Name;
                    }
                }
                if (ObjectsTransform.Count != 0)
                {
                    foreach (var ot in ObjectsTransform)
                    {
                        ot.Value.GetComponentInParent<TooltipText>().text = output;
                    }
                }
                else
                {
                    ObjectsTransform[0].GetComponentInParent<TooltipText>().text = output;
                }
            }
        }

        /// <summary>
        /// Ребро/Метаребро.
        /// Ребро должно содержать в себе две любые вершины. Без них ребро не ребро.
        /// Ребро может быть МетаРебром, за это отвечает bool параметр MetaType.
        /// Ребро может быть направленным (eo=true), от указанного StartVertex до EndVertex. 
        /// </summary>
        public class Edge : Predicate
        {
            //public override Transform ObjectTransform { get; set; }
            public override Dictionary<int, Transform> ObjectsTransform { get; set; }
            // Название объекта.
            public override string Name { get; set; }
            // Стартовая вершина.
            public Vertex StartVertex = null;
            // Конечная вершина.
            public Vertex EndVertex = null;
            // Признак Мета принадлежности.
            public bool MetaType;
            // Признак направленности ребра.
            public bool EdgeDirection;
            Dictionary<string, Vertex> Bonds;
            // Подчинённые предикаты.
            public Dictionary<string, Predicate> Predicates;

            public Edge(string name, Dictionary<string, Vertex> bonds, Dictionary<string, Predicate> predicates = null, bool eo = false, bool metatype = false)
            {
                ObjectsTransform = new Dictionary<int, Transform>();
                MetaType = metatype;
                if (bonds.ContainsKey("start") == true) StartVertex = bonds["start"];
                if (bonds.ContainsKey("end") == true) EndVertex = bonds["end"];
                EdgeDirection = eo;
                Predicates = predicates;
                Bonds = bonds;
                Name = name;

                Create();
                OutLog();
            }
            public void Create()
            {
                Vector3 positionFirst = new Vector3();
                Vector3 positionSecond = new Vector3();
                Color32 color = new Color32(74, 161, 112, 128);

                if (Bonds.Count != 0)
                {
                    positionFirst = StartVertex.ObjectsTransform[0].position;
                    positionSecond = EndVertex.ObjectsTransform[0].position;

                    Debug.Log("Попытались соединить направленный");
                }
                else if (Predicates.Count > 2)
                {
                    positionFirst = StartVertex.ObjectsTransform[0].position;
                    positionSecond = new Vector3(Random.Range(-1.18f, 1.18f), Random.Range(-1f, 1f), Random.Range(0f, 4f)); // TO DO [ИЗ ЛОГИКИ]

                    Debug.Log("Попытались соединить несколько детей");
                }
                else if (Predicates.Count == 2)
                {
                    int k = 0;
                    foreach (var Predicate in Predicates)
                    {
                        if (k == 0)
                        {
                            positionFirst = Predicate.Value.ObjectsTransform[0].position;
                        }
                        if (k == 1)
                        {
                            positionSecond = Predicate.Value.ObjectsTransform[0].position;
                        }
                        k++;
                    }

                    Debug.Log("Соединили две простых вершины");
                }

                ObjectsTransform[0] = InitObject.Instance.InitLine(false, positionFirst, positionSecond, color, Name);
            }
            public override void OutLog()
            {
                string Chain = null;
                if (Predicates != null && Predicates.Count != 0)
                {
                    int numItemsSeen = 0;
                    foreach (var Predicate in Predicates)
                    {
                        if (++numItemsSeen == Predicates.Count)
                        {
                            Chain += Predicate.Value.Name;
                        }
                        else
                        {
                            Chain += Predicate.Value.Name + " --- ";
                        }
                    }
                    if (EdgeDirection)
                    {
                        Chain = StartVertex.Name + " --- " + Chain + " --> " + EndVertex.Name;
                    }
                }
                else
                {
                    if (EdgeDirection)
                    {
                        Chain = StartVertex.Name + " --> " + EndVertex.Name;
                    }
                }
                string NameObject = (MetaType == false) ? "Edge" : "Metaedge";
                string output = "<b>" + NameObject + " |</b> Name: " + Name + " | EdgeDirection: " + EdgeDirection
                    + " | HasChild: " + ((Predicates.Count != 0) ? "True" : "False") + " | <b>" + Chain + "</b>";
                Debug.Log(output);
                if (Predicates != null && Predicates.Count != 0)
                {
                    output += "\nChildren:";
                    foreach (var Predicate in Predicates)
                    {
                        Debug.Log("\t └> " + Predicate.Value.Name);
                        output += "\n" + Predicate.Value.Name;
                    }
                }
                ObjectsTransform[0].GetComponentInParent<TooltipText>().text = output;
            }
        }

        /// <summary>
        /// Метаграф/граф. Когда я это писал, я точно не знал нужно ли делать Graph двухуровневым.
        /// Поэтому настройка metatype стоит по стандарту на true, другими словами, Metagraph.
        /// </summary>
        public class Graph : Predicate
        {
            public override Vector3 FixingPoint { get; set; }
            //public override Transform ObjectTransform { get; set; }
            public override Dictionary<int, Transform> ObjectsTransform { get; set; }
            //public override Transform[] ObjectsTransform { get; set; }
            // Название объекта.
            public override string Name { get; set; }
            // Признак Мета принадлежности.
            public bool MetaType;
            // Подчинённые предикаты.
            public Dictionary<string, Predicate> Predicates;

            public Graph(string name, Dictionary<string, Predicate> predicates = null, bool metatype = true)
            {ObjectsTransform = new Dictionary<int, Transform>();
                MetaType = metatype;
                Predicates = predicates;
                Name = name;
                Create();
                OutLog();
            }
            public void Create()
            {
                // Позиция центра вершины-связи и сферы.
                FixingPoint = new Vector3(Random.Range(-1.18f, 1.18f), Random.Range(-1f, 1f), Random.Range(0f, 4f)); // TO DO [ИЗ ЛОГИКИ]
                if (Predicates != null && Predicates.Count >= 1)
                {
                    // Вершина-связь.
                    Vector3 positionChild = new Vector3();
                    Color32 color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 128);

                    int n = 0;
                    foreach (var Predicate in Predicates)
                    {
                        // Не делаем связь с Edge и MetaEdge
                        if (Predicate.Value is Edge) continue;
                        positionChild = Predicate.Value.FixingPoint;
                        ObjectsTransform[n] = InitObject.Instance.InitLine(true, FixingPoint, positionChild, color, Name);
                        n++;
                    }
                }
                else
                {
                    // Вершина-сфера.
                    ObjectsTransform[0] = InitObject.Instance.InitGraph(FixingPoint, new Color32(0, 0, 0, 128), Name);
                }
            }
            public override void OutLog()
            {
                string NameObject = (MetaType == false) ? "Graph" : "Metagraph";
                string output = "<b>" + NameObject + " |</b> Name: " + Name
                    + " | HasChild: " + ((Predicates != null) ? "True" : "False");
                Debug.Log(output);
                if (Predicates != null && Predicates.Count != 0)
                {
                    output += "\nChildren:";
                    foreach (var Predicate in Predicates)
                    {
                        Debug.Log("\t └> " + Predicate.Value.Name);
                        output += "\n" + Predicate.Value.Name;
                    }
                }
                ObjectsTransform[0].GetComponentInParent<TooltipText>().text = output;
            }
        }

        /// <summary>
        /// Атрибут.
        /// </summary>
        public class Attribute : Predicate
        {
            // Название объекта.
            public override string Name { get; set; }
            public string ValueType;
            public object Value;

            public Attribute(string name, string type = "string", string value = null)
            {
                //Dictionary<string, Predicate> predi = GameObject.Find("Reader").GetComponent<Reader>().predicate;
                Name = name;
                ValueType = type;
                //if (ValueType == "link" && rd.predicate.ContainsKey(value))
                //{
                //    Value
                //}
                //Reader.predicate[]
                OutLog();
            }
            public override void OutLog()
            {
                string output = "<b>Attribute |</b> Name: " + Name;
                Debug.Log(output);
                //if (ValueType == "int") Debug.Log("<b>Attribute |</b> Name: " + Name + " | " + ValueType + ": " + Value);
                //if (ValueType == "link") Debug.Log("<b>Attribute |</b> Name: " + Name + " | " + ValueType + ": " + (Predicate)Value);
            }
        }
    }
}