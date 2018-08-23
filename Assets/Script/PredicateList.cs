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
            // Название объекта.
            public override string Name { get; set; }
            // Признак Мета принадлежности.
            public bool MetaType;
            // Подчинённые предикаты.
            public Dictionary<string, Predicate> Predicates;

            public Vertex(string name, Dictionary<string, Predicate> predicates = null, bool metatype = false)
            {
                MetaType = metatype;
                Predicates = predicates;
                Name = name;

                Create();
                OutLog();
            }
            public void Create()
            {
            }
            public override void OutLog()
            {
                string NameObject = (MetaType == false) ? "Vertex" : "Metavertex";
                Debug.Log("<b>" + NameObject + " |</b> Name: " + Name  + " | HasChild: " 
                    + ((Predicates != null) ? "True" : "False"));
                if (Predicates != null && Predicates.Count != 0)
                {
                    foreach (var Predicate in Predicates)
                    {
                        Debug.Log("\t └> " + Predicate.Value.Name);
                    }
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
            // Стартовая вершина.
            public Vertex StartVertex = null;
            // Конечная вершина.
            public Vertex EndVertex = null;
            // Признак Мета принадлежности.
            public bool MetaType;
            // Признак направленности ребра.
            public bool EdgeDirection;
            // Подчинённые предикаты.
            public Dictionary<string, Predicate> Predicates;

            public Edge(string name, Dictionary<string, Vertex> bonds, Dictionary<string, Predicate> predicates = null, bool eo = false, bool metatype = false)
            {
                MetaType = metatype;
                if (bonds.ContainsKey("start") == true) StartVertex = bonds["start"];
                if (bonds.ContainsKey("end") == true) EndVertex = bonds["end"];
                EdgeDirection = eo;
                Predicates = predicates;
                Name = name;

                Create();
                OutLog();
            }
            public void Create()
            {
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
                Debug.Log("<b>" + NameObject + " |</b> Name: " + Name + " | EdgeDirection: " + EdgeDirection 
                    + " | HasChild: " + ((Predicates.Count != 0) ? "True" : "False") + " | <b>" + Chain + "</b>");
                if (Predicates != null && Predicates.Count != 0)
                {
                    foreach (var Predicate in Predicates)
                    {
                        Debug.Log("\t └> " + Predicate.Value.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Метаграф/граф. Когда я это писал, я точно не знал нужно ли делать Graph двухуровневым.
        /// Поэтому настройка metatype стоит по стандарту на true, другими словами, Metagraph.
        /// </summary>
        public class Graph : Predicate
        {
            // Название объекта.
            public override string Name { get; set; }
            // Признак Мета принадлежности.
            public bool MetaType;
            // Подчинённые предикаты.
            public Dictionary<string, Predicate> Predicates;

            public Graph(string name, Dictionary<string, Predicate> predicates = null, bool metatype = true)
            {
                MetaType = metatype;
                Predicates = predicates;
                Name = name;

                Create();
                OutLog();
            }
            public void Create()
            {
            }
            public override void OutLog()
            {
                string NameObject = (MetaType == false) ? "Graph" : "Metagraph";
                Debug.Log("<b>" + NameObject + " |</b> Name: " + Name  
                    + " | HasChild: " + ((Predicates != null) ? "True" : "False"));
                if (Predicates != null && Predicates.Count != 0)
                {
                    foreach (var Predicate in Predicates)
                    {
                        Debug.Log("\t └> " + Predicate.Value.Name);
                    }
                }
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
                //if (ValueType == "int") Debug.Log("<b>Attribute |</b> Name: " + Name + " | " + ValueType + ": " + Value);
                //if (ValueType == "link") Debug.Log("<b>Attribute |</b> Name: " + Name + " | " + ValueType + ": " + (Predicate)Value);
            }
        }
    }
}