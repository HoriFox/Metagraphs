using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologPredicate : MonoBehaviour {
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
    }

    // Все элементы описания.
    public class Predicate
    {
        public virtual string Name { get; set; }
    }

    /// <summary>
    /// Вершина/Метавершина.
    /// </summary>
    public class Vertex : Predicate
    {
        public string NameStandrt;
        public override string Name { get; set; }
        // Признак Мета принадлежности.
        public bool MetaType;
        // Подчинённые предикаты.
        public Dictionary<string, Predicate> Predicates;

        public Vertex(Dictionary<string, Predicate> predicates = null, string name = null, bool type = false)
        {
            MetaType = type;
            Predicates = predicates;

            NameStandrt = (type == false) ? "v" : "mv";
            Name = (name != null) ? name : NameSystem.GetName(NameStandrt);

            Create();
            Log();
        }
        /// <summary>
        /// Создание объекта в пространстве.
        /// </summary>
        public void Create()
        {
        }
        /// <summary>
        /// Вывод лога в консоль.
        /// </summary>
        public void Log()
        {
            string NameObject = (MetaType == false) ? "Vertex" : "Metavertex";
            Debug.Log("<b>" + NameObject + " |</b> Name: " + Name + " | NameStandrt: " + NameStandrt + 
                " | MetaType: " + MetaType);
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
        public string NameStandrt;
        public override string Name { get; set; }
        // Стартовая вершина.
        public Vertex StartVertex;
        // Конечная вершина.
        public Vertex EndVertex;
        // Признак Мета принадлежности.
        public bool MetaType;
        // Признак направленности ребра.
        public bool EdgeDirection;

        public Edge(Vertex start, Vertex end, bool eo = false, bool type = false, string name = null)
        {
            MetaType = type;
            StartVertex = start;
            EndVertex = end;
            EdgeDirection = eo;

            NameStandrt = (type == false) ? "e" : "me";
            Name = (name != null) ? name : NameSystem.GetName(NameStandrt);

            Create();
            Log();
        }
        /// <summary>
        /// Создание объекта в пространстве.
        /// </summary>
        public void Create()
        {
        }
        /// <summary>
        /// Вывод лога в консоль.
        /// </summary>
        public void Log()
        {
            string Direction = (EdgeDirection == false) ? " --- " : " --> ";
            string NameObject = (MetaType == false) ? "Edge" : "Metaedge";
            Debug.Log("<b>" + NameObject + " |</b> Name: " + Name + " | NameStandrt: " + NameStandrt +
                " | MetaType: " + MetaType + " | EdgeDirection: " + EdgeDirection + " | <b>" + 
                StartVertex.Name + Direction + EndVertex.Name + "</b>");
        }
    }

    public class Attribute : Predicate
    {
        public override string Name { get; set; }
        public Predicate Link;
        public int Value;

        public Attribute(string name, Predicate link)
        {
            Name = name;
            Link = link;
            Log(1);
        }
        public Attribute(string name, int value)
        {
            Name = name;
            Value = value;
            Log(2);
        }
        public void Log(int type)
        {
            if (type == 1)
            {
                Debug.Log("<b>Attribute |</b> Name: " + Name + " | ToName: " + Link.Name);
            }
            if (type == 2)
            {
                Debug.Log("<b>Attribute |</b> Name: " + Name + " | Value: " + Value);
            }
        }
    }

    public class Agent : Predicate
    {
        public Agent()
        {
        }
        public void Log()
        {
        }
    }

    public class Rule : Predicate
    {
        public Rule()
        {
        }
        public void Log()
        {
        }
    }

    public class Condition : Predicate
    {
        public Condition()
        {
        }
        public void Log()
        {
        }
    }

    public class Action : Predicate
    {
        public Action()
        {
        }
        public void Eval()
        {
        }
        public void Add()
        {
        }
        public void Log()
        {
        }
    }

    void Start()
    {
        // Создаём обычную вершину по умолчанию.
        Vertex v1 = new Vertex();
        // Создаём два количественных атрибута с разным value.
        Attribute a1 = new Attribute("Количество1", 10);
        Attribute a2 = new Attribute("Количество2", 20);
        // Создаём ссылочный атрибут с разным указывающий на ранеесозданную вершину v1.
        Attribute a3 = new Attribute("Ссылка", v1);
        // Создаём словарь предикатов.
        Dictionary<string, Predicate> vertexChild = new Dictionary<string, Predicate>();
        // Загружаем два созданных ранее количественных атрибута.
        vertexChild[a1.Name] = a1;
        vertexChild[a2.Name] = a2;
        // Создаём вершину с типом "Мета" и подчинёнными предикатами.
        Vertex mv1 = new Vertex(vertexChild, type:true);
        // Создаём обычную вершину по умолчанию.
        Vertex v2 = new Vertex();
        // Создаём обычное ребро по умолчанию соединяющую вершины v2 и v3.
        Edge edge1 = new Edge(v2, mv1);
        // Создаём направленное ребро "customEdge" с типом "Мета" соединяющую вершины v1 и v3.
        Edge edge2 = new Edge(v1, mv1, eo:true, type:true, name:"customEdge");
        // Проверка NameSystem. Следующим Vertex со NameStandart = "v" она присвоит индексы 3 и 4
        Vertex v3 = new Vertex();
        Vertex v4 = new Vertex();
    }
}
