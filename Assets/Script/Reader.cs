using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace nm
{
    public class Reader : MonoBehaviour
    {
        public Dictionary<string, PredicateList.Predicate> predicate = null;
        public void ReadCode(string content)
        {
            predicate = new Dictionary<string, PredicateList.Predicate>();
            string input = content.Replace(" ", string.Empty);
            //Debug.Log(input);
            var delimiters = new List<string> { ",", "(", ")" };
            string pattern = "(" + String.Join("|", delimiters.Select(d => Regex.Escape(d)).ToArray()) + ")";
            string[] result = Regex.Split(input, pattern);

            ReadNowSector(result, 0, result[0]);
        }

        public KeyValuePair<PredicateList.Predicate, int> ReadNowSector(string[] array, int nowSector, string predicateType)
        {
            // Массив тех частей кода, которые нужны на текущий момент.
            string[] result = null;
            // Словарь, который содержит вершины для объединения ребром.
            Dictionary<string, PredicateList.Vertex> bondsDict = new Dictionary<string, PredicateList.Vertex>();
            // Словарь, который содержит подчинённые предикаты, для дальнейшего вкладывания в предикат высшего порядка.
            Dictionary<string, PredicateList.Predicate> predicateChild = new Dictionary<string, PredicateList.Predicate>();
            // Имя текущего предиката.
            string predicateName = null;
            // Нужен только для Attribute. Содержит тип значения атрибута.
            string predicateValueType = null;
            // Нужен только для Attribute. Содержит значение атрибута.
            string predicateValue = null;
            // Нужен только для Edge. Признак нправленности ребра.
            bool edgeDirection = false;
            PredicateList.Predicate currentPredicate = null;
            // Результат текущего вызова ReadNowSector, который нужен для завершения вызова высшего порядка.
            KeyValuePair<PredicateList.Predicate, int> resultCall;
            while (nowSector != array.Length)
            {
                nowSector++;
                // Если мы нашли тип данных вида "name=value".
                if (array[nowSector].Contains("="))
                {
                    string pattern = "(=)";
                    result = Regex.Split(array[nowSector], pattern);

                    string key = result[2];
                    switch (result[0])
                    {
                        case "Name":
                            predicateName = key;
                            break;
                        case "Type":
                            predicateValueType = key;
                            break;
                        case "Value":
                            predicateValue = key;
                            break;
                        case "eo":
                            edgeDirection = bool.Parse(key);
                            break;
                        case "vS":
                            if (predicate.ContainsKey(key) == false)
                            {
                                predicate[key] = new PredicateList.Vertex(key);
                            }
                            bondsDict["start"] = (PredicateList.Vertex)predicate[key];
                            break;
                        case "vE":
                            if (predicate.ContainsKey(key) == false)
                            {
                                predicate[key] = new PredicateList.Vertex(key);
                            }
                            bondsDict["end"] = (PredicateList.Vertex)predicate[key];
                            break;
                    }
                }
                else
                {
                    switch (array[nowSector])
                    {
                        case ",":
                        case "(":
                            break;
                        case ")":
                            goto ENDRETURN;
                        case "Vertex":
                            resultCall = ReadNowSector(array, nowSector, "Vertex");
                            nowSector = resultCall.Value;
                            predicateChild[resultCall.Key.Name] = resultCall.Key;
                            break;
                        case "Metavertex":
                            resultCall = ReadNowSector(array, nowSector, "Metavertex");
                            nowSector = resultCall.Value;
                            predicateChild[resultCall.Key.Name] = resultCall.Key;
                            break;
                        case "Edge":
                            resultCall = ReadNowSector(array, nowSector, "Edge");
                            nowSector = resultCall.Value;
                            predicateChild[resultCall.Key.Name] = resultCall.Key;
                            break;
                        case "Metaedge":
                            resultCall = ReadNowSector(array, nowSector, "Metaedge");
                            nowSector = resultCall.Value;
                            predicateChild[resultCall.Key.Name] = resultCall.Key;
                            break;
                        case "Metagraph":
                            resultCall = ReadNowSector(array, nowSector, "Metagraph");
                            nowSector = resultCall.Value;
                            predicateChild[resultCall.Key.Name] = resultCall.Key;
                            break;
                        case "Attribute":
                            resultCall = ReadNowSector(array, nowSector, "Attribute");
                            nowSector = resultCall.Value;
                            predicateChild[resultCall.Key.Name] = resultCall.Key;
                            break;
                        default:
                            // Если это неопределённый тип, то вероятнее всего это примитив Vertex.
                            if (predicate.ContainsKey(array[nowSector]))
                            {
                                // Если мы ранее инициализировали это примитив.
                                predicateChild[array[nowSector]] = predicate[array[nowSector]];
                            }
                            else
                            {
                                // Если мы ранее НЕ инициализировали это примитив.
                                PredicateList.Vertex vertex = new PredicateList.Vertex(array[nowSector]);
                                predicate[vertex.Name] = vertex;
                                predicateChild[vertex.Name] = vertex;
                            }
                            break;
                    }
                }
            }
            ENDRETURN:
            switch (predicateType)
            {
                case "Vertex":
                    currentPredicate = new PredicateList.Vertex(predicateName, predicateChild);
                    break;
                case "Metavertex":
                    currentPredicate = new PredicateList.Vertex(predicateName, predicateChild, metatype: true);
                    break;
                case "Edge":
                    currentPredicate = new PredicateList.Edge(predicateName, bondsDict, predicateChild, edgeDirection);
                    break;
                case "Metaedge":
                    currentPredicate = new PredicateList.Edge(predicateName, bondsDict, predicateChild, edgeDirection, metatype: true);
                    break;
                case "Metagraph":
                    currentPredicate = new PredicateList.Graph(predicateName, predicateChild, metatype: true);
                    break;
                case "Attribute":
                    currentPredicate = new PredicateList.Attribute(predicateName, predicateValueType, predicateValue);
                    break;
            }
            predicate[predicateName] = currentPredicate;
            return new KeyValuePair<PredicateList.Predicate, int>(currentPredicate, nowSector + 1);
        }
    }
}
