using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class OutlogModule : MonoBehaviour
    {
        public bool active = true;

        private string output = null;
        private string Name = null;
        private Dictionary<string, Structure> m_currentStructureDict;
        private Structure m_currentStructure;
        private static OutlogModule init;

        private void Awake()
        {
            init = this;
        }

        public static OutlogModule GetInit()
        {
            return init;
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
            output = "<b>" + NameObject + " |</b> Name: " + Name;
            if (active) Debug.Log(output);
            if (m_currentStructure.ChildStructures != null && m_currentStructure.ChildStructures.Count != 0)
            {
                output += "\nChildren:";
                foreach (var child in m_currentStructure.ChildStructures)
                {
                    if (active) Debug.Log("\t └> " + child.Value.Name);
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
            foreach (var part in childList)
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
                    if (i != 0)
                    {
                        Chain += (" --- " + part);
                    }
                    else
                    {
                        Chain += part;
                    }
                }
                i++;
            }
            string NameObject = m_currentStructure.ObjectType;
            output = "<b>" + NameObject + " |</b> Name: " + Name + " | EdgeDirection: " + m_currentStructure.Eo + " | <b>" + Chain + "</b>";
            if (active) Debug.Log(output);
            if (m_currentStructure.ChildStructures != null && m_currentStructure.ChildStructures.Count != 0)
            {
                output += "\nChildren:";
                foreach (var child in m_currentStructure.ChildStructures)
                {
                    if (active) Debug.Log("\t └> " + child.Value.Name);
                    output += "\n" + child.Value.Name;
                }
            }
        }

        private void OutLogAttribute()
        {
            switch (m_currentStructure.TypeValue)
            {
                case "int":
                case "string":
                case "pointer":
                    if (active) Debug.Log("<b>Attribute |</b> Name: " + Name + " | " + m_currentStructure.TypeValue + ": " + m_currentStructure.Value);
                    break;
                case "link":
                    string output = null;
                    if (m_currentStructureDict.ContainsKey(m_currentStructure.Value))
                    {
                        Structure valueStructure = m_currentStructureDict[m_currentStructure.Value];
                        output = valueStructure.Value + " (" + valueStructure.ObjectType + ")";
                    }
                    if (active) Debug.Log("<b>Attribute |</b> Name: " + Name + " | " + output);
                    break;
            }
        }

        public void OutTooltip(string customOutput = null)
        {
            if (customOutput != null)
            {
                output = customOutput;
            }
            foreach (var childPart in m_currentStructure.gameObject)
            {
                childPart.transform.GetComponentInParent<TooltipText>().text = output;
            }
        }
    }
}
