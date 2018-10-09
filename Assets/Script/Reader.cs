using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace nm
{
    public class Reader : MonoBehaviour
    {
        private static Reader init;

        bool lastLoadCompleted = true;
        [HideInInspector]
        public string reservedContent = null;

        private StructureModule structureM;
        private Engine engineM;

        private void Awake()
        {
            init = this;
        }

        private void Start()
        {
            structureM = StructureModule.GetInit();
            engineM = Engine.GetInit();
        }

        public static Reader GetInit()
        {
            return init;
        }

        public void ReloadCode()
        {
            ChangeTransform.Instance.ResetChangeTransform();
            if (reservedContent != null && lastLoadCompleted)
            {
                engineM.ReadAndBuild(reservedContent);
            }
        }

        public void ReadCode(string content)
        {
            SceneCleaning.Instance.Clean();
            structureM.NewStructure();

            lastLoadCompleted = false;
            reservedContent = content;
            string input = content.Replace(" ", string.Empty);
            input = input.Replace("\t", string.Empty);
            input = input.Replace("\n", string.Empty);
            input = input.Replace("\r", string.Empty);
            var delimiters = new List<string> { ",", "(", ")" };
            string pattern = "(" + String.Join("|", delimiters.Select(d => Regex.Escape(d)).ToArray()) + ")";
            string[] result = Regex.Split(input, pattern);

            ReadAllSctor(result);
        }

        public void ReadAllSctor(string[] result)
        {
            KeyValuePair<string, int> resultCall;
            int nowSector = 0;

            while (nowSector != result.Length)
            {
                switch (result[nowSector])
                {
                    case "Vertex":
                    case "Metavertex":
                    case "Edge":
                    case "Metaedge":
                    case "Graph":
                    case "Metagraph":
                    case "Attribute":
                        break;
                    default:
                        nowSector++;
                        continue;
                }
                resultCall = ReadNowSector(result, nowSector);
                nowSector = resultCall.Value;
            }
            lastLoadCompleted = true;
        }

        public KeyValuePair<string, int> ReadNowSector(string[] array, int nowSector)
        {
            if (((nowSector) >= array.Length)) return new KeyValuePair<string, int>(null, array.Length);

            int nowLevel = 0;
            string nowType = null;
            string nowNameNode = null;
            List<string> namesChild = new List<string>();
            KeyValuePair<string, int> resultCall;

            while (nowSector <= array.Length) // !
            {
                if (array[nowSector].Contains("="))
                {
                    string pattern = "(=)";
                    string[] localArray = Regex.Split(array[nowSector], pattern);

                    string value = localArray[2];
                    switch (localArray[0])
                    {
                        case "Name":
                            nowNameNode = value;
                            structureM.AddNode(nowNameNode);
                            break;
                        case "Type":
                            structureM.AddNodeData(nowNameNode, typeValue: value);
                            break;
                        case "Value":
                            structureM.AddNodeData(nowNameNode, value: value);
                            break;
                        case "eo":
                            structureM.AddNodeData(nowNameNode, eo: (bool.Parse(value)));
                            break;
                        case "vS":
                            if (!structureM.IsExistNode(value))
                            {
                                structureM.AddNode(value);
                                structureM.AddNodeData(value, "Vertex");
                            }
                            structureM.AddNodeData(nowNameNode, start: value); // ? TO DO
                            namesChild.Add(value);
                            break;
                        case "vE":
                            if (!structureM.IsExistNode(value))
                            {
                                structureM.AddNode(value);
                                structureM.AddNodeData(value, "Vertex");
                            }
                            structureM.AddNodeData(nowNameNode, end: value); // ? TO DO
                            namesChild.Add(value);
                            break;
                    }
                }
                else
                {
                    switch (array[nowSector])
                    {
                        case ",":
                            break;
                        case "(":
                            nowLevel++; // Повышаем уровень скобок.
                            break;
                        case ")":
                            nowLevel--; // Понижаем уровень скобок.
                            structureM.AddNodeData(nowNameNode, nowType);
                            structureM.AddEnvironment(nowNameNode, childNames: namesChild);
                            // Если это рёбра, то это статические объекты. Другими словами, самостоятельные.
                            if (nowType == "Edge" || nowType == "Metaedge")
                            {
                                structureM.structure[nowNameNode].Static = true;
                            }
                            return new KeyValuePair<string, int>(nowNameNode, nowSector + 1);
                        case "Vertex":
                        case "Metavertex":
                        case "Edge":
                        case "Metaedge":
                        case "Graph":
                        case "Metagraph":
                        case "Attribute":
                            if (nowLevel == 0)
                            {
                                nowType = array[nowSector];
                            }
                            else
                            {
                                resultCall = ReadNowSector(array, nowSector);
                                nowSector = resultCall.Value;
                                namesChild.Add(resultCall.Key);
                            }
                            break;
                        default:
                            structureM.AddNode(array[nowSector]);
                            structureM.AddNodeData(array[nowSector], "Vertex");
                            namesChild.Add(array[nowSector]);
                            break;
                    }
                }
                nowSector++;
            }
            return new KeyValuePair<string, int>(nowNameNode, nowSector); // !
        }
    }
}
