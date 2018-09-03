using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class Structure
    {
        // Для Creator.
        public string Name = null;
        public string ObjectType = null;
        public Vector3 FixingPoint = new Vector3();
        public bool Eo = false;
        public bool Metatype = false;
        public string Start = null;
        public string End = null;


        public string ParentName = null;
        public Structure ParentStructure = null;

        public Dictionary<string, Structure> ChildStructures = new Dictionary<string, Structure>();

        public string Setting = null;
    }

    public class StructureModule : MonoBehaviour
    {
        Dictionary<string, Structure> structure = new Dictionary<string, Structure>();

        public Structure AddNode(string name)
        {
            if (!structure.ContainsKey(name))
            {
                structure[name] = new Structure
                {
                    Name = name,
                    Setting = "Настройки " + name
                };
            }
            return structure[name];
        }
        public void AddEnvironment(string name, string parentName = null, List<string> childNames = null)
        {
            if (!IsExistNode(name)) return;
            if (parentName != null)
            {
                structure[name].ParentName = parentName;
                structure[name].ParentStructure = AddNode(parentName);
                // Доработка для дурака. Если указал родителя, то и у родителя должен появиться ребёнок.
                structure[parentName].ChildStructures[name] = structure[name];
            }
            if ((childNames != null) && (!(childNames.Count == 0)))
            {
                foreach (var childN in childNames)
                {
                    structure[name].ChildStructures[childN] = AddNode(childN);
                    // Доработка для дурака. Если указал ребёнка, то и у ребёнка должен появиться родитель.
                    structure[childN].ParentName = name;
                    structure[childN].ParentStructure = structure[name];
                }
            }
        }
        public void AddNodeData(string name, string objectType, bool eo = false, bool metatype = false, string start = null, string end = null)
        {
            if (!IsExistNode(name)) return;
            structure[name].ObjectType = objectType;
            structure[name].Eo = eo;
            structure[name].Metatype = metatype;
            structure[name].Start = start;
            structure[name].End = end;
        }
        public Structure GetParent(string nameNode)
        {
            if (!IsExistNode(nameNode)) return null;
            return structure[nameNode].ParentStructure;
        }
        public Dictionary<string, Structure> GetChild(string nameNode)
        {
            if (!IsExistNode(nameNode)) return null;
            return structure[nameNode].ChildStructures;
        }

        public void OutLog(string nameNode)
        {
            if (!IsExistNode(nameNode)) return;
            if (structure[nameNode].ObjectType == null)
            {
                Debug.Log("Не установлен тип. Неопределённый вывод!");
                return;
            }
            if (structure[nameNode].ObjectType == "Vertex")
            {
                string NameObject = ((structure[nameNode].Metatype == false) ? "Vertex" : "Metavertex");
                string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name;
                Debug.Log(output);
            }
            if (structure[nameNode].ObjectType == "Edge")
            {
                string NameObject = ((structure[nameNode].Metatype == false) ? "Edge" : "Metaedge");
                string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name + 
                    " | EdgeDirection: " + structure[nameNode].Eo;
                Debug.Log(output);
            }
            if (structure[nameNode].ObjectType == "Graph")
            {
                Debug.Log("Graph");
            }
            if (structure[nameNode].ObjectType == "Attribute")
            {
                Debug.Log("Name: " + structure[nameNode].Name);
            }
        }
        public void OutLogInfo(string nameNode)
        {
            if (!IsExistNode(nameNode)) return;
            Debug.Log("Название: " + structure[nameNode].Name);
            //Debug.Log("Setting текущей node: " + structure[nameNode].Setting);
            if (structure[nameNode].ParentStructure == null)
            {
                Debug.Log("Родитель узла отсутствует");
            }
            else
            {
                Debug.Log("Родитель: " + GetParent(nameNode).Name);
                //Debug.Log("Setting parent node: " + GetParent(nameNode).Setting);
            }

            if (structure[nameNode].ChildStructures.Count == 0) // Не точно! TO DO
            {
                Debug.Log("Дети узла отсутствуют");
            }
            else
            {
                foreach(var child in structure[nameNode].ChildStructures)
                {
                    Debug.Log("Ребёнок: " + child.Key);
                }
            }
        }

        public bool IsExistNode(string nameNode)
        {
            if (nameNode == null || !structure.ContainsKey(nameNode))
            {
                Debug.Log("Узел с таким именем не существует!");
                return false;
            }
            return true;
        }

        private void Start()
        {
            // Представим идеальные условия. Когда на каждом этапе мы указали максимальное количество информации.
            AddNode("mv1");
            AddNode("v1");
            AddNode("v2");
            //AddNode("v3");    // Допустим, не указали.
            //AddNode("v4");    // Допустим, не указали.
            AddNode("v5");
            AddNode("v6");
            AddNode("v7");

            AddEnvironment("mv1", null, new List<string> { "v1" });
            AddEnvironment("v1", "mv1", new List<string> { "v2", "v3", "v4" });
            AddEnvironment("v2", "v1", new List<string> { "v7" });
            //AddEnvironment("v3", "v1");                                           // Допустим, не указали.
            //AddEnvironment("v4", "v1", new List<string> { "v5", "v6" });          // Допустим, не указали.
            AddEnvironment("v5", "v4");
            AddEnvironment("v6", "v4");
            AddEnvironment("v7", "v2");

            OutLogInfo("v3");

            //AddNodeData("mv1", "Vertex", metatype:true);
            //AddNodeData("v1", "Vertex");
            //AddNodeData("v2", "Vertex");
            //AddNodeData("v3", "Vertex");
            //AddNodeData("v4", "Vertex");
            //AddNodeData("v5", "Vertex");
            //AddNodeData("v6", "Vertex");
            //AddNodeData("v7", "Vertex");

            //OutLogParentInfo("v1");

            //OutLog("mv1");
            //OutLog("v2");

            foreach (var st in structure)
            {
                Debug.Log(st.Value.Name);
            }
        }
    }
}
