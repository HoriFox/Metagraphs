using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class OutlogModule : MonoBehaviour
    {
        public bool ShowDebag = true;

        private string output = null;
        private string Name = null;
        private Dictionary<string, Structure> m_currentStructureDict;
        private Structure m_currentStructure;
        private static OutlogModule Instance;
        private StructureModule structureM;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            structureM = StructureModule.GetInit();
        }

        public static OutlogModule GetInit()
        {
            return Instance;
        }

        public void ConsoleLog(string name, ref Dictionary<string, Structure> structure, string objectType = "Vertex")
        {
            Name = name;
            m_currentStructureDict = structure;
            m_currentStructure = structure[Name];

            if (objectType == "Vertex")
            {
                OutLogVertexGraph();
            }
            if (objectType == "Edge")
            {
                OutLogEdge();
            }
            if (objectType == "Attribute")
            {
                OutLogAttribute();
            }
        }

        private void OutLogVertexGraph()
        {
            string NameObject = m_currentStructure.ObjectType;
            string descCont = m_currentStructure.Description;
            string Description = (descCont != null) ? (" | Description: " + m_currentStructure.Description) : null;
            output = "<b>" + NameObject + " |</b> Name: " + Name + Description;
            if (ShowDebag) Debug.Log(output);
            if (m_currentStructure.ChildStructures != null && m_currentStructure.ChildStructures.Count != 0)
            {
                output += "\nChildren:";
                foreach (var child in m_currentStructure.ChildStructures)
                {
                    if (ShowDebag) Debug.Log("\t └> " + child.Value.Name);
                    output += "\n" + child.Value.Name;
                }
            }
        }

        private void OutLogEdge()
        {
            string Chain = null;
            List<string> childList = new List<string>();

            childList.Add(m_currentStructure.Start);
            foreach (var child in m_currentStructure.ChildStructures)
            {
                if (child.Value.Name != m_currentStructure.Start && child.Value.Name != m_currentStructure.End)
                {
                    childList.Add(child.Value.Name);
                }
            }
            childList.Add(m_currentStructure.End);

            int i = 0;
            bool firstNull = false;
            foreach (var part in childList)
            {
                if (firstNull)
                {
                    firstNull = false;
                    Chain += part;
                }
                else
                {
                    // Если последний элемент.
                    if ((i + 1) == childList.Count)
                    {
                        // Значит End есть.
                        if (part != null)
                        {
                            if (m_currentStructure.Eo)
                            {
                                Chain += (" --> " + part);
                            }
                            else
                            {
                                Chain += (" --- " + part);
                            }
                        }
                    }
                    else
                    {
                        // Первый элемент.
                        if (i >= 1)
                        {
                            Chain += (" --- " + part);
                        }
                        else
                        {
                            if (part != null)
                            {
                                Chain += part;
                            }
                            else
                            {
                                firstNull = true;
                            }
                        }
                    }
                }
                i++;
            }

            // " | EdgeDirection: " + m_currentStructure.Eo

            string NameObject = m_currentStructure.ObjectType;
            string descCont = m_currentStructure.Description;
            string Description = (descCont != null) ? (" | Description: " + m_currentStructure.Description) : null;
            output = "<b>" + NameObject + " |</b> Name: " + Name + Description + " | <b>" + Chain + "</b>";
            if (ShowDebag) Debug.Log(output);
            if (m_currentStructure.ChildStructures != null && m_currentStructure.ChildStructures.Count != 0)
            {
                output += "\nChildren:";
                foreach (var child in m_currentStructure.ChildStructures)
                {
                    if (ShowDebag) Debug.Log("\t └> " + child.Value.Name);
                    output += "\n" + child.Value.Name;
                }
            }
        }

        private void OutLogAttribute()
        {
            string descCont = m_currentStructure.Description;
            string Description = (descCont != null) ? (" | Description: " + m_currentStructure.Description) : null;
            switch (m_currentStructure.TypeValue)
            {
                case "int":
                case "string":
                case "pointer":
                    if (ShowDebag) Debug.Log("<b>Attribute |</b> Name: " + Name + Description + " | " + m_currentStructure.TypeValue + ": " + m_currentStructure.Value);
                    break;
                case "link":
                    string output = null;
                    if (m_currentStructureDict.ContainsKey(m_currentStructure.Value))
                    {
                        Structure valueStructure = m_currentStructureDict[m_currentStructure.Value];
                        output = valueStructure.Value + " (" + valueStructure.ObjectType + ")";
                    }
                    if (ShowDebag) Debug.Log("<b>Attribute |</b> Name: " + Name + Description + " | " + output);
                    break;
            }
        }

        public void OutTooltip(string customOutput = null, string currentName = null)
        {
            List<GameObject> currentStructure = null;
            if (customOutput != null)
            {
                output = customOutput;
            }
            if (currentName != null)
            {
                currentStructure = structureM.structure[currentName].gameObject;
            }
            else
            {
                currentStructure = m_currentStructure.gameObject;
            }
            foreach (var childPart in currentStructure)
            {
                childPart.transform.GetComponentInParent<TooltipText>().text = output;
            }
        }
    }
}
