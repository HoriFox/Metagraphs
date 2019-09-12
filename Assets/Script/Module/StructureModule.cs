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
        public string Description = null;
        public Vector3[] Position = null;
        public Color32 color = new Color32(0, 0, 0, 0);
        public string ObjectType = null;
        public bool Eo = false;
        public string Start = null;
        public string End = null;
        public string TypeValue = null;
        public string Value = null;

        public bool Arc = false;
        public bool Static = false;
        public float? Radius = null;

        public string[] ParentStructuresKeys = null;
        public string[] ChildStructuresKeys = null;

        // Дополняется с помощью специальной функции после чтения. Соединяем созданные в структуре объекты.
        [NonSerialized] public Dictionary<string, Structure> ParentStructures = new Dictionary<string, Structure>();
        [NonSerialized] public Dictionary<string, Structure> ChildStructures = new Dictionary<string, Structure>();

        // Добавляет программа.
        [NonSerialized] public List<GameObject> gameObject = new List<GameObject>();

        public Vector3 GetPosition(int index)
        {
            return Position[index];
        }
    }

    public class StructureModule : MonoBehaviour
    {
        [HideInInspector] public PredicateModule predicateM;
        [HideInInspector] public ChangeModule changeM;
        private static StructureModule Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            predicateM = PredicateModule.GetInit();
            changeM = ChangeModule.GetInit();
        }

        public static StructureModule GetInit()
        {
            return Instance;
        }

        public Dictionary<string, Structure> structure = new Dictionary<string, Structure>();

        public void NewStructure()
        {
            structure.Clear();
            structure = new Dictionary<string, Structure>();
        }

        // Загрузка из JSON.
        public void LoadingJson(string path)
        {
            // СТАДИЯ 1. ЗАГРУЗКА ИЗ JSON В СТРУКТУРУ.

            // Очищаем выделение, если оно и было. 
            changeM.ResetChange();
            // Очищаем объекты unity сцены.
            SceneCleaning.Instance.Clean();
            // Очищаем систему имён.
            PredicateModule.NameSystem.Clear();
            // Создаём новую структуру.
            NewStructure();

            using (StreamReader sr = new StreamReader(path))
            {
                string json = sr.ReadToEnd();
                json = json.Replace("\t", string.Empty);
                json = json.Replace("\n", string.Empty);
                json = json.Replace("\r", string.Empty);
                Structure[] structureArr = JsonHelper.FromJson<Structure>(json);
                foreach (var part in structureArr)
                {
                    structure[part.Name] = part;
                }
            }
            foreach (var part in structure)
            {
                foreach (var parent in part.Value.ParentStructuresKeys)
                {
                    part.Value.ParentStructures.Add(parent, structure[parent]);
                }
                foreach (var child in part.Value.ChildStructuresKeys)
                {
                    part.Value.ChildStructures.Add(child, structure[child]);
                }
                // Поправить как-нибудь. TO DO
                // При десериализации null становится empty.
                if (part.Value.Description == string.Empty)
                {
                    part.Value.Description = null;
                }
                if (part.Value.Start == string.Empty)
                {
                    part.Value.Start = null;
                }
                if (part.Value.End == string.Empty)
                {
                    part.Value.End = null;
                }
            }

            // Загружаем новые имена в менеджер имён.
            PredicateModule.NameSystem.LoadNameDict(ref structure);

            // СТАДИЯ 2. ПОСТРОЕНИЕ ГРАФА ПО СТРУКТУРЕ.
            predicateM.BuildGraphs();
        }

        // Выгрузка в JSON.
        public void UnloadingJson(string path)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                Structure[] structureArr = new Structure[structure.Count];

                int i = 0;
                foreach (var childStructure in structure)
                {
                    childStructure.Value.ParentStructuresKeys = new string[childStructure.Value.ParentStructures.Count];
                    int p = 0;
                    foreach (var parent in childStructure.Value.ParentStructures)
                    {
                        childStructure.Value.ParentStructuresKeys[p] = parent.Key;
                        p++;
                    }
                    childStructure.Value.ChildStructuresKeys = new string[childStructure.Value.ChildStructures.Count];
                    int c = 0;
                    foreach (var child in childStructure.Value.ChildStructures)
                    {
                        childStructure.Value.ChildStructuresKeys[c] = child.Key;
                        c++;
                    }
                    structureArr[i] = childStructure.Value;
                    i++;
                }
                string json = JsonHelper.ToJson(structureArr, true);
                stream.Write(json);
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
            if (childNames != null && childNames.Count != 0)
            {
                foreach (var childN in childNames)
                {
                    structure[name].ChildStructures[childN] = AddNode(childN);
                    // Доработка для дурака. Если указал ребёнка, то и у ребёнка должен появиться родитель.
                    structure[childN].ParentStructures[name] = structure[name];
                }
            }
        }

        public void AddNodeData(string name, Vector3[] position = null, string objectType = null, bool? eo = null, string start = null, string end = null, string typeValue = null, string value = null, bool? isArc = null)
        {
            if (!IsExistNode(name)) return;
            if (position != null)
            {
                structure[name].Position = position;
            }
            if (objectType != null)
            {
                structure[name].ObjectType = objectType;
                if (objectType == "Edge" || objectType == "Metaedge")
                {
                    structure[name].Static = true;
                }
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
            if (isArc != null)
            {
                structure[name].Arc = isArc.GetValueOrDefault();
            }
        }

        public void EditNodeData(string name, string newName = null)
        {
            // Если что-то изменилось.
            if (name != newName)
            {
                AddNode(newName);
                structure[newName] = structure[name];
                structure[newName].Name = newName;
                structure.Remove(name);
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

        //public void OutLog(string nameNode)
        //{
        //    if (!IsExistNode(nameNode)) return;
        //    if (structure[nameNode].ObjectType == null)
        //    {
        //        Debug.Log("Не установлен тип. Неопределённый вывод!");
        //        return;
        //    }
        //    if (structure[nameNode].ObjectType == "Vertex" || structure[nameNode].ObjectType == "Metavertex")
        //    {
        //        string NameObject = structure[nameNode].ObjectType;
        //        string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name;
        //        Debug.Log(output);
        //    }
        //    if (structure[nameNode].ObjectType == "Edge" || structure[nameNode].ObjectType == "Metaedge")
        //    {
        //        string NameObject = structure[nameNode].ObjectType;
        //        string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name +
        //            " | EdgeDirection: " + structure[nameNode].Eo;
        //        if (structure[nameNode].Start != null && structure[nameNode].End != null)
        //        {
        //            output += " | Start: " + structure[nameNode].Start + " | End: " + structure[nameNode].End;
        //        }
        //        Debug.Log(output);
        //    }
        //    if (structure[nameNode].ObjectType == "Graph" || structure[nameNode].ObjectType == "Metagraph")
        //    {
        //        string NameObject = structure[nameNode].ObjectType;
        //        string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name;
        //        Debug.Log(output);
        //    }
        //    if (structure[nameNode].ObjectType == "Attribute")
        //    {
        //        string NameObject = structure[nameNode].ObjectType;
        //        string output = "<b>" + NameObject + " |</b> Name: " + structure[nameNode].Name +
        //            " | TypeValue: " + structure[nameNode].TypeValue +
        //            " | Value: " + structure[nameNode].Value;
        //        Debug.Log(output);
        //    }
        //}

        //public void OutLogInfo(string nameNode)
        //{
        //    if (!IsExistNode(nameNode)) return;
        //    Debug.Log("Название: " + structure[nameNode].Name);
        //    if (structure[nameNode].ParentStructures.Count == 0)
        //    {
        //        Debug.Log("Родители узла отсутствует");
        //    }
        //    else
        //    {
        //        foreach (var parent in structure[nameNode].ParentStructures)
        //        {
        //            Debug.Log("Родитель: " + parent.Key);
        //        }
        //    }

        //    if (structure[nameNode].ChildStructures.Count == 0)
        //    {
        //        Debug.Log("Дети узла отсутствуют");
        //    }
        //    else
        //    {
        //        foreach(var child in structure[nameNode].ChildStructures)
        //        {
        //            Debug.Log("Ребёнок: " + child.Key);
        //        }
        //    }
        //}

        public bool IsExistNode(string nameNode)
        {
            if (nameNode == null || !structure.ContainsKey(nameNode))
            {
                return false;
            }
            return true;
        }
    }
}
