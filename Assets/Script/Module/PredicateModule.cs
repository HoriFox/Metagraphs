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

        //Система даёт индивидуальный индекс имени.
        public static class NameSystem
        {
            public static List<string> nameDict = new List<string>();
            public static Dictionary<string, int> countDict = new Dictionary<string, int>();

            public static void LoadNameDict (ref Dictionary<string, Structure> structure)
            {
                foreach (var part in structure)
                {
                    nameDict.Add(part.Key);
                }
            }

            public static string GetName(string type)
            {
                string name;

                // Будет пробовать подобрать имя, если оно уже есть в nameDick, при этом меняя значения в countDick для дальнейших генераций.
                do
                {
                    if (countDict.ContainsKey(type))
                        countDict[type]++;
                    else
                        countDict.Add(type, 1);

                    name = type + countDict[type];

                } while (nameDict.Contains(name));

                nameDict.Add(name);

                return name;
            }

            public static void Clear()
            {
                nameDict.Clear();
                countDict.Clear();
            }
        }

        // СТАДИЯ 2. ПОСТРОЕНИЕ ГРАФА ПО СТРУКТУРЕ. ПОЛНОЕ СОЗДАНИЕ.
        public void BuildGraphs()
        {
            foreach(var part in structureM.structure)
            {
                // Вызываем единичное обновление на каждый объект.
                TactBuild(part.Key, part.Value.ObjectType);
            }
        }

        // ЕДИНИЧНОЕ ОБНОВЛЕНИЕ ПРЕДИКАТА.
        public void TactBuild(string name, string objectType)
        {
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
                    new Edge(name, ref structureM.structure);
                    break;
                case "Attribute":
                    new Attribute(name, ref structureM.structure);
                    break;
            }
        }

        /// <summary>
        /// Метаграф/граф. Вершина/Метавершина.
        /// </summary>
        public class VertexGraph
        {
            public string Name { get; set; }
            private Structure m_structure;

            public VertexGraph(string name, ref Dictionary<string, Structure> structure)
            {
                Name = name;
                m_structure = structure[Name];
                Create();
                OutlogModule outlogM = OutlogModule.GetInit();
                outlogM.ConsoleLog(Name, ref structure, "Vertex");
                outlogM.OutTooltip();
            }
            public void Create()
            {
                Vector3 position = m_structure.GetPosition(0);
                Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
                // Если альфа канал 0, то цвет не установлен.
                if (m_structure.color.a == 0)
                {
                    SetColor(new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255));
                }

                if (m_structure.ChildStructures != null && m_structure.ChildStructures.Count >= 1)
                {
                    // Вершина-связь.
                    foreach (var part in m_structure.ChildStructures)
                    {
                        // Не делаем связь с Edge и MetaEdge
                        if (part.Value.Static) continue;
                        Vector3 childPosition = part.Value.GetPosition(0);
                        m_structure.gameObject.AddRange(InitObject.Instance.InitLine(true, position, childPosition, m_structure.color, Name));
                    }
                }
                else
                {
                    // Вершина-сфера.
                    m_structure.gameObject.Add(InitObject.Instance.InitGraph(position, size, m_structure.color, Name));
                }
            }
            public void SetColor(Color32 colorNew)
            {
                m_structure.color = colorNew;
            }
        }

        /// <summary>
        /// Ребро/Метаребро.
        /// Ребро должно содержать в себе две любые вершины. Без них ребро не ребро.
        /// Ребро может быть направленным (eo=true), от указанного StartVertex до EndVertex. 
        /// </summary>
        public class Edge
        {
            public string Name { get; set; }
            private Dictionary<string, Structure> m_structureDict;
            private Structure m_structure;

            public Edge(string name, ref Dictionary<string, Structure> structure)
            {
                Name = name;
                m_structureDict = structure;
                m_structure = structure[Name];
                Create();
                OutlogModule outlogM = OutlogModule.GetInit();
                outlogM.ConsoleLog(Name, ref structure, "Edge");
                outlogM.OutTooltip();
            }
            public void Create()
            {
                // Если альфа канал 0, то цвет не установлен. 
                if (m_structure.color.a == 0)
                {
                    SetColor(new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255));
                }

                // TO DO. Больше тестов!
                // По концепции не может работать без детей. А просто выставлять доп. координаты бессмысленно.
                if (m_structure.ChildStructures.Count > 0)
                {
                    List<Vector3> postionList = new List<Vector3>();                            //Создаём лист координатов, которые мы заполним, а потом и построим по ним.

                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Композитор
                    // Если есть направленная связь, то самое первое - начальный объект.
                    if (m_structure.Eo)
                    {
                        postionList.Add(m_structureDict[m_structure.Start].GetPosition(0));
                    }
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ <<

                    if (m_structure.Position.Length == 0)
                    {
                        foreach (var part in m_structure.ChildStructures)
                        {
                            postionList.Add(part.Value.Position[0]);
                        }
                    }
                    else
                    if (m_structure.Position.Length > 0 && m_structure.ChildStructures.Count == 2) // TO DO. Связи между объектами и текущие дополнительные позиции.
                    {
                        int k = 0;
                        // Плохой код.
                        foreach (var part in m_structure.ChildStructures)
                        {
                            if (k == 0)
                            {
                                postionList.Add(part.Value.Position[0]);
                                postionList.AddRange(m_structure.Position);
                            }
                            if (k == 1)
                            {
                                postionList.Add(part.Value.Position[0]);
                                break;
                            }
                            k++;
                        }
                    }
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ <<
                    // Если есть направленная связь, то самое последнее - конечный объект.
                    if (m_structure.Eo)
                    {
                        postionList.Add(m_structureDict[m_structure.End].GetPosition(0));
                    }
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Билдер
                    int n = 0;
                    Vector3 lastPosition = Vector3.zero;
                    Vector3 nextPosition = Vector3.zero;

                    foreach (var position in postionList)
                    {
                        if (n == 0)
                        {
                            lastPosition = position;
                            n++;
                            continue;
                        }
                        nextPosition = position;
                        // TO DO. Сделать свойство isArc у связи вариативным: прямая, дуга.
                        m_structure.gameObject.AddRange(InitObject.Instance.InitLine(m_structure.Arc, false, lastPosition, nextPosition, m_structure.color, Name, isSimple: true));
                        lastPosition = nextPosition;
                        n++;
                    }

                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                }
            }
            public void SetColor(Color32 colorNew)
            {
                m_structure.color = colorNew;
            }
        }

        /// <summary>
        /// Атрибут.
        /// </summary>
        public class Attribute
        {
            public string Name { get; set; }
            public Attribute(string name, ref Dictionary<string, Structure> structure)
            {
                Name = name;
                OutlogModule outlogM = OutlogModule.GetInit();
                outlogM.ConsoleLog(Name, ref structure, "Attribute");
            }
        }
    }
}