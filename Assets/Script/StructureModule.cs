﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace nm
{
    // История из раздела "Почему медленно работает прога?"
    [Serializable]
    public class Structure
    {
        public string Name = null;
        [NonSerialized]
        public string ObjectType = null;
        [NonSerialized]
        public bool Eo = false;
        [NonSerialized]
        public bool Metatype = false;
        [NonSerialized]
        public string Start = null;
        [NonSerialized]
        public string End = null;
        [NonSerialized]
        public string TypeValue = null;
        [NonSerialized]
        public string Value = null;

        public Vector3 position;
        public Vector3 rotationEuler;

        public bool isUsingCustomPosition = false;
        public bool isUsingCustomRotation = false;

        public Vector3 customPosition;
        public Vector3 customRotationEuler;
        [NonSerialized]
        public List<GameObject> gameObject = new List<GameObject>();

        public Color32 color = new Color32(0, 0, 0, 0);
        [NonSerialized]
        public Dictionary<string, Structure> ParentStructures = new Dictionary<string, Structure>();
        [NonSerialized]
        public Dictionary<string, Structure> ChildStructures = new Dictionary<string, Structure>();

        public Vector3 GetPosition()
        {
            return (isUsingCustomPosition) ? customPosition : position;
        }

        public Vector3 GetRotation()
        {
            return (isUsingCustomRotation) ? customRotationEuler : rotationEuler;
        }
    }

    //public class TransformStructure
    //{
    //    public Vector3 position;
    //    public Vector3 rotationEuler;

    //    public bool isUsingCustomPosition = false;
    //    public bool isUsingCustomRotation = false;

    //    public Vector3 customPosition;
    //    public Vector3 customRotationEuler;

    //    public Vector3 GetPosition()
    //    {
    //        return (isUsingCustomPosition) ? customPosition : position;
    //    }

    //    public Vector3 GetRotation()
    //    {
    //        return (isUsingCustomRotation) ? customRotationEuler : rotationEuler;
    //    }
    //}

    public class StructureModule : MonoBehaviour
    {
        private static StructureModule init;

        private void Awake()
        {
            init = this;
        }

        public static StructureModule GetInit()
        {
            return init;
        }

        public Dictionary<string, Structure> structure = new Dictionary<string, Structure>();

        public void NewStructure()
        {
            structure = new Dictionary<string, Structure>();
        }

        // Загрузка из JSON.
        public void LoadingJson(string path)
        {
            //using (StreamWriter stream = new StreamWriter(path))
            //{
            //    foreach (var child in structure)
            //    {
            //        string json = JsonUtility.ToJson(child.Value);
            //        stream.Write(json);
            //    }
            //}
        }

        // Выгрузка в JSON.
        public void UnloadingJson(string path)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                foreach(var child in structure)
                {
                    string json = JsonUtility.ToJson(child.Value);
                    stream.Write(json);
                }
            }
        }

        public Structure AddNode(string name)
        {
            if (!structure.ContainsKey(name))
            {
                structure[name] = new Structure
                {
                    Name = name,
                };
            }
            return structure[name];
        }

        public void AddEnvironment(string name, string parentName = null, List<string> childNames = null)
        {
            if (!IsExistNode(name)) return;
            if (parentName != null)
            {
                structure[name].ParentStructures[parentName] = AddNode(parentName);
                // Доработка для дурака. Если указал родителя, то и у родителя должен появиться ребёнок.
                structure[parentName].ChildStructures[name] = structure[name];
            }
            if ((childNames != null) && (!(childNames.Count == 0)))
            {
                foreach (var childN in childNames)
                {
                    structure[name].ChildStructures[childN] = AddNode(childN);
                    // Доработка для дурака. Если указал ребёнка, то и у ребёнка должен появиться родитель.
                    structure[childN].ParentStructures[name] = structure[name];
                }
            }
        }

        public void AddNodeData(string name, string objectType = null, bool? eo = null, string start = null, string end = null, string typeValue = null, string value = null)
        {
            if (!IsExistNode(name)) return;
            if (objectType != null)
            {
                structure[name].Metatype = (objectType.Substring(0, 4) == "Meta") ? true : false;
                structure[name].ObjectType = objectType;
            }
            if (eo != null)
            {
                structure[name].Eo = eo.GetValueOrDefault();
            }
            if (start != null)
            {
                structure[name].Start = start;
            }
            if (end != null)
            {
                structure[name].End = end;
            }
            if (typeValue != null)
            {
                structure[name].TypeValue = typeValue;
            }
            if (value != null)
            {
                structure[name].Value = value;
            }
        }

        public Dictionary<string, Structure> GetParent(string nameNode)
        {
            if (!IsExistNode(nameNode)) return null;
            return structure[nameNode].ParentStructures;
        }

        public Dictionary<string, Structure> GetChild(string nameNode)
        {
            if (!IsExistNode(nameNode)) return null;
            return structure[nameNode].ChildStructures;
        }

        public int GetChildCount(string nameNode)
        {
            List<string> verifiedNodes = new List<string>();
            return ChildCountGo(nameNode, ref verifiedNodes);
        }

        private int ChildCountGo(string nameNode, ref List<string> verifiedNodes)
        {
            int currentCount = 0;
            foreach (var partChild in GetChild(nameNode))
            {
                if (!verifiedNodes.Contains(partChild.Key))
                {
                    currentCount++;
                    verifiedNodes.Add(partChild.Key);
                    currentCount += ChildCountGo(partChild.Key, ref verifiedNodes);
                }
            }
            return currentCount;
        }

        public void OutLog(string nameNode)
        {
            if (!IsExistNode(nameNode)) return;
            if (structure[nameNode].ObjectType == null)
            {
                Debug.Log("Не установлен тип. Неопределённый вывод!");
                return;
            }
            if (structure[nameNode].ObjectType == "Vertex" || structure[nameNode].ObjectType == "Metavertex")
            {
                string NameObject = structure[nameNode].ObjectType;
                string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name;
                Debug.Log(output);
            }
            if (structure[nameNode].ObjectType == "Edge" || structure[nameNode].ObjectType == "Metaedge")
            {
                string NameObject = structure[nameNode].ObjectType;
                string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name +
                    " | EdgeDirection: " + structure[nameNode].Eo;
                if (structure[nameNode].Start != null && structure[nameNode].End != null)
                {
                    output += " | Start: " + structure[nameNode].Start + " | End: " + structure[nameNode].End;
                }
                Debug.Log(output);
            }
            if (structure[nameNode].ObjectType == "Graph" || structure[nameNode].ObjectType == "Metagraph")
            {
                string NameObject = structure[nameNode].ObjectType;
                string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name;
                Debug.Log(output);
            }
            if (structure[nameNode].ObjectType == "Attribute")
            {
                string NameObject = structure[nameNode].ObjectType;
                string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name +
                    " | TypeValue: " + structure[nameNode].TypeValue +
                    " | Value: " + structure[nameNode].Value;
                Debug.Log(output);
            }
        }

        public void OutLogInfo(string nameNode)
        {
            if (!IsExistNode(nameNode)) return;
            Debug.Log("Название: " + structure[nameNode].Name);
            if (structure[nameNode].ParentStructures.Count == 0)
            {
                Debug.Log("Родители узла отсутствует");
            }
            else
            {
                foreach (var parent in structure[nameNode].ParentStructures)
                {
                    Debug.Log("Родитель: " + parent.Key);
                }
            }

            if (structure[nameNode].ChildStructures.Count == 0)
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
                return false;
            }
            return true;
        }

        //private void Start()
        //{
            // Представим идеальные условия. Когда на каждом этапе мы указали максимальное количество информации.
            //AddNode("mv1");
            //AddNode("v1");
            //AddNode("v2");
            //AddNode("v3");    // Допустим, не указали.
            //AddNode("v4");    // Допустим, не указали.
            //AddNode("v5");
            //AddNode("v6");
            //AddNode("v7");

            //AddEnvironment("mv1", null, new List<string> { "v1" });
            //AddEnvironment("v1", "mv1", new List<string> { "v2", "v3", "v4" });
            //AddEnvironment("v2", "v1", new List<string> { "v7" });
            //AddEnvironment("v3", "v1");                                           // Допустим, не указали.
            //AddEnvironment("v4", "v1", new List<string> { "v5", "v6" });          // Допустим, не указали.
            //AddEnvironment("v5", "v4");
            //AddEnvironment("v6", "v4");
            //AddEnvironment("v7", "v2");

            ////OutLogInfo("v3");

            //AddNodeData("mv1", "Vertex", metatype:true);
            //AddNodeData("v1", "Vertex");
            //AddNodeData("v2", "Vertex");
            //AddNodeData("v3", "Vertex");
            //AddNodeData("v4", "Vertex");
            //AddNodeData("v5", "Vertex");
            //AddNodeData("v6", "Vertex");
            //AddNodeData("v7", "Vertex");

            //foreach (var st in structure)
            //{
            //    Debug.Log(st.Value.Name);
            //}
        //}
    }
}
