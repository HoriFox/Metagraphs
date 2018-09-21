using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class PredicateModule : MonoBehaviour
    {
        private static PredicateModule init;
        private StructureModule structureM;

        private void Awake()
        {
            init = this;
        }

        private void Start()
        {
            structureM = StructureModule.GetInit();
        }

        public static PredicateModule GetInit()
        {
            return init;
        }

        // Система даёт индивидуальный индекс имени.
        //public static class NameSystem
        //{
        //    public static Dictionary<string, int> nameDict = new Dictionary<string, int>();

        //    public static string GetName(string type)
        //    {
        //        string name = "error";
        //        if (nameDict.ContainsKey(type))
        //            nameDict[type]++;
        //        else
        //            nameDict.Add(type, 1);

        //        name = type + nameDict[type];
        //        return name;
        //    }

        //    public static void RemoveLastIndex(string type)
        //    {
        //        nameDict[type]--;
        //    }
        //}

        public void BuildGraphs()
        {
            foreach(var part in structureM.structure)
            {
                TactBuild(part.Key, part.Value.ObjectType);
            }
        }

        public void TactBuild(string name, string objectType)
        {
            Debug.Log(name + " " + objectType);
            switch (objectType)
            {
                case "Vertex":
                case "Metavertex":
                case "Graph":
                case "Metagraph":
                    new VertexGraph(name, ref structureM.structure);
                    break;
                case "Edge":
                case "Metaedge":
                    Debug.Log("1");
                    new Edge(name, ref structureM.structure);
                    break;
                case "Attribute":
                    new Attribute(name, ref structureM.structure);
                    break;
            }
        }

        // Все элементы описания.
        public class Predicate
        {
            public virtual string Name { get; set; }
            public virtual void OutLog(ref Dictionary<string, Structure> structure) { }
        }

        /// <summary>
        /// Вершина/Метавершина.
        /// Важно учитывать, что у класса вершины есть Dictionary, который очень чувствителен
        /// к дублирующимся custom именам. 
        /// </summary>
        /// <summary>
        /// Метаграф/граф. Когда я это писал, я точно не знал нужно ли делать Graph двухуровневым.
        /// Поэтому настройка metatype стоит по стандарту на true, другими словами, Metagraph.
        /// </summary>
        public class VertexGraph : Predicate
        {
            // Название объекта.
            public override string Name { get; set; }

            public VertexGraph(string name, ref Dictionary<string, Structure> structure)
            {
                Name = name;
                Create(structure[Name]);
                OutLog(ref structure);
            }
            public void Create(Structure structure)
            {
                Vector3 position = structure.GetPosition();
                Color32 color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);

                if (structure.ChildStructures != null && structure.ChildStructures.Count >= 1)
                {
                    structure.transform = new Transform[structure.ChildStructures.Count];
                    // Вершина-связь.
                    int n = 0;
                    foreach (var part in structure.ChildStructures)
                    {
                        // Не делаем связь с Edge и MetaEdge
                        if (structure.ObjectType == "Edge" || structure.ObjectType == "Metaedge") continue;
                        Vector3 childPosition = part.Value.GetPosition();
                        structure.transform[n] = InitObject.Instance.InitLine(true, position, childPosition, color, Name);
                        n++;
                    }
                }
                else
                {
                    // Вершина-сфера.
                    // Обязательно класть transform в структуру.
                    structure.transform[0] = InitObject.Instance.InitGraph(position, color, Name);
                }
            }
            public override void OutLog(ref Dictionary<string, Structure> structure)
            {
                Structure thisStructure = structure[Name];
                string NameObject = thisStructure.ObjectType;
                string output = "<b>" + NameObject + " |</b> Name: " + Name + " | HasChild: "
                    + ((thisStructure.ChildStructures != null && thisStructure.ChildStructures.Count != 0) ? "True" : "False");
                Debug.Log(output);
                if (thisStructure.ChildStructures != null && thisStructure.ChildStructures.Count != 0)
                {
                    output += "\nChildren:";
                    foreach (var child in thisStructure.ChildStructures)
                    {
                        Debug.Log("\t └> " + child.Value.Name);
                        output += "\n" + child.Value.Name;
                    }
                }
                if (thisStructure.transform.Length != 0)
                {
                    foreach (var ot in thisStructure.transform)
                    {
                        ot.GetComponentInParent<TooltipText>().text = output;
                    }
                }
                else
                {
                    thisStructure.transform[0].GetComponentInParent<TooltipText>().text = output;
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
            // Название объекта.
            public override string Name { get; set; }

            public Edge(string name, ref Dictionary<string, Structure> structure)
            {
                Name = name;
                Debug.Log("1.1");
                Create(ref structure);
                OutLog(ref structure);
            }
            public void Create(ref Dictionary<string, Structure> structure)
            {
                Debug.Log("1.2");
                Structure thisStructure = structure[Name];
                Debug.Log("1.3");
                Color32 color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);

                //if (Bonds.Count != 0)
                //{
                //    positionFirst = StartVertex.ObjectsTransform[0].position;
                //    positionSecond = EndVertex.ObjectsTransform[0].position;

                //    Debug.Log("Попытались соединить направленный");
                //}
                //else if (Predicates.Count > 2)
                //{
                //    positionFirst = StartVertex.ObjectsTransform[0].position;
                //    positionSecond = new Vector3(Random.Range(-1.18f, 1.18f), Random.Range(-1f, 1f), Random.Range(0f, 4f)); // TO DO [ИЗ ЛОГИКИ]

                //    Debug.Log("Попытались соединить несколько детей");
                //}
                //else if (Predicates.Count == 2)
                //{
                //    int k = 0;
                //    foreach (var Predicate in Predicates)
                //    {
                //        if (k == 0)
                //        {
                //            positionFirst = Predicate.Value.ObjectsTransform[0].position;
                //        }
                //        if (k == 1)
                //        {
                //            positionSecond = Predicate.Value.ObjectsTransform[0].position;
                //        }
                //        k++;
                //    }

                //    Debug.Log("Соединили две простых вершины");
                //}
                Debug.Log("1.4");
                Vector3 firstPosition = structure[thisStructure.Start].GetPosition(); // ОШИБКА ТУТ. TO DO
                Debug.Log("1.5");
                Vector3 secondPosition = structure[thisStructure.End].GetPosition(); // ОШИБКА ТУТ. TO DO

                Debug.Log("2");
                thisStructure.transform[0] = InitObject.Instance.InitLine(false, firstPosition, secondPosition, color, Name);
                Debug.Log("3");
            }
            public override void OutLog(ref Dictionary<string, Structure> structure)
            {
                Structure thisStructure = structure[Name];
                string Chain = null;
                if (thisStructure.ChildStructures != null && thisStructure.ChildStructures.Count != 0)
                {
                    int numItemsSeen = 0;
                    foreach (var child in thisStructure.ChildStructures)
                    {
                        if (++numItemsSeen == thisStructure.ChildStructures.Count)
                        {
                            Chain += child.Value.Name;
                        }
                        else
                        {
                            Chain += child.Value.Name + " --- ";
                        }
                    }
                    if (thisStructure.Eo)
                    {
                        Chain = thisStructure.Start + " --- " + Chain + " --> " + thisStructure.End;
                    }
                }
                else
                {
                    if (thisStructure.Eo)
                    {
                        Chain = thisStructure.Start + " --> " + thisStructure.End;
                    }
                }
                string NameObject = thisStructure.ObjectType;
                string output = "<b>" + NameObject + " |</b> Name: " + Name + " | EdgeDirection: " + thisStructure.Eo + " | HasChild: " 
                    + ((thisStructure.ChildStructures != null && thisStructure.ChildStructures.Count != 0) ? "True" : "False") 
                    + " | <b>" + Chain + "</b>";
                Debug.Log(output);
                if (thisStructure.ChildStructures != null && thisStructure.ChildStructures.Count != 0)
                {
                    output += "\nChildren:";
                    foreach (var child in thisStructure.ChildStructures)
                    {
                        Debug.Log("\t └> " + child.Value.Name);
                        output += "\n" + child.Value.Name;
                    }
                }
                thisStructure.transform[0].GetComponentInParent<TooltipText>().text = output;
            }
        }

        /// <summary>
        /// Атрибут.
        /// </summary>
        public class Attribute : Predicate
        {
            // Название объекта.
            public override string Name { get; set; }

            public Attribute(string name, ref Dictionary<string, Structure> structure)
            {
                Name = name;
                Create(ref structure);
                OutLog(ref structure);
            }
            public void Create(ref Dictionary<string, Structure> structure)
            {
                //Structure thisStructure = structure[Name];
            }
            public override void OutLog(ref Dictionary<string, Structure> structure)
            {
                Structure thisStructure = structure[Name];

                switch(thisStructure.TypeValue)
                {
                    case "int":
                    case "string":
                    case "pointer":
                        Debug.Log("<b>Attribute |</b> Name: " + Name + " | " + thisStructure.TypeValue + ": " + thisStructure.Value);
                        break;
                    case "link":
                        string output = null;
                        if (structure.ContainsKey(thisStructure.Value))
                        {
                            Structure valueStructure = structure[thisStructure.Value];
                            output = valueStructure.Value + " (" + valueStructure.ObjectType + ")";
                        }
                        break;
                }
            }
        }
    }
}