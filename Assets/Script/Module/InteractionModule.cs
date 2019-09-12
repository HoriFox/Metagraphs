using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class InteractionModule : MonoBehaviour
    {
        private static InteractionModule Instance;

        private FreeCamera freeCamera;
        private PredicateModule predicateM;
        private StructureModule structureM;
        private ChangeModule changeM;

        public bool isConnection = false;
        public string startConnectionObject = null;
        //public bool isPanelActive = false;

        public Structure targetObject = new Structure();
        public bool isNowSelected = false;

        public bool arcConnection = false;

        public bool ArcConnection
        {
            get { return arcConnection; }
            set { arcConnection = value; }
        }

        public string connectionType = "normal";
                                    //"directed"
                                    //"nesting"

        public string ConnectionType
        {
            get { return connectionType; }
            set { connectionType = value; }
        }

        private void Awake()
        {
            Instance = this;
            freeCamera = Camera.main.GetComponent<FreeCamera>();
        }

        private void Start()
        {
            predicateM = PredicateModule.GetInit();
            structureM = StructureModule.GetInit();
            changeM = ChangeModule.GetInit();
        }

        public static InteractionModule GetInit()
        {
            return Instance;
        }

        private float doubleTapDelay = 0.4f;
        private float tapCount = 0;
        public void DoubleTap(string methodLuck)
        {
            tapCount++;
            if (tapCount == 2)
            {
                CancelInvoke("FailDoubleTap");
                tapCount = 0;
                Invoke(methodLuck, 0f);
                return;
            }

            Invoke("FailDoubleTap", doubleTapDelay);
        }

        private void FailDoubleTap()
        {
            tapCount = 0;
        }

        // Если сделали двойной клик в пустоту.
        public void DoubleTapToNull()
        {
            //Debug.Log("Я кликнул два раза в пустоту");

            isConnection = false;
            SelectActive(startConnectionObject, false);
            startConnectionObject = null;

            string name = PredicateModule.NameSystem.GetName("Vertex");
            Vector3[] position = new Vector3[1];

            // 5f - расстояние создания.
            Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5f);
            position[0] = Camera.main.ScreenToWorldPoint(mousePosFar);
            structureM.AddNode(name);
            structureM.AddNodeData(name, position, "Vertex");
            predicateM.TactBuild(name, "Vertex");
            changeM.ResetChange();
            isNowSelected = true;
            freeCamera.selectedObject = name;
            changeM.ShowChangeMenu(name);
        }

        // Присоединение.
        public void EndConnection()
        {
            string typeStart = structureM.structure[startConnectionObject].ObjectType;
            string typeEnd = structureM.structure[freeCamera.selectedObject].ObjectType;
            if (typeStart != "Edge" && typeStart != "Metaedge" && typeEnd != "Edge" && typeEnd != "Metaedge")
            {
                //Debug.Log("Присоединение");

                Vector3[] position = null;
                List<string> child = null;

                switch (connectionType)
                {
                    case "nesting":
                        position = new Vector3[2];
                        position[0] = structureM.structure[startConnectionObject].GetPosition(0);
                        position[1] = structureM.structure[freeCamera.selectedObject].GetPosition(0);
                        child = new List<string>();
                        child.Add(freeCamera.selectedObject);
                        structureM.AddEnvironment(startConnectionObject, childNames: child);
                        changeM.RebuildObject("rebuild", startConnectionObject);
                        break;

                    case "directed":
                    case "normal":
                        string name = PredicateModule.NameSystem.GetName("Edge");
                        position = new Vector3[0];
                        structureM.AddNode(name);

                        if (connectionType == "directed")
                            structureM.AddNodeData(name, position, "Edge", true, startConnectionObject, freeCamera.selectedObject, isArc: arcConnection);
                        else
                            structureM.AddNodeData(name, position, "Edge", isArc: arcConnection);

                        child = new List<string>();
                        child.Add(startConnectionObject);
                        child.Add(freeCamera.selectedObject);
                        structureM.AddEnvironment(name, childNames: child);
                        predicateM.TactBuild(name, "Edge");
                        changeM.ResetChange();
                        freeCamera.selectedObject = name;
                        changeM.ShowChangeMenu(name);
                        break;
                }
            }
            else
            {
                changeM.ResetChange();
                Debug.Log("С Edge и Metaedge пока не соединяем");
            }
        }

        // Если сделали двойной клик по объекту.
        public void DoubleTapToObject()
        {
            //Debug.Log("Я кликнул два раза по объекту");

            startConnectionObject = freeCamera.selectedObject;

            string type = structureM.structure[startConnectionObject].ObjectType;
            if (type != "Edge" && type != "Metaedge")
            {
                isConnection = true;
                startConnectionObject = freeCamera.selectedObject;
                SelectActive(startConnectionObject, true);
            }
            else
            {
                Debug.Log("Edge и Metaedge пока не соединяем");
                startConnectionObject = null;
            }
        }

        public void SelectActive(string name, bool active)
        {
            if (name == null) return;

            if (structureM.structure.ContainsKey(name))
            {
                GameObject selected = structureM.structure[name].gameObject[0].GetComponent<TooltipText>().selectedContainer;
                if (selected != null)
                {
                    selected.SetActive(active);
                }
            }
        }

        //public void ConnectionActive(bool active, bool setSelectedObject)
        //{
        //    isConnection = active;
        //    if (setSelectedObject)
        //    {
        //        startConnectionObject = freeCamera.selectedObject;
        //    }
        //    SelectActive(startConnectionObject, active);
        //}

        public bool IsExitObjectInStructure(string name)
        {
            if (structureM.IsExistNode(name))
            {
                return true;
            }
            return false;
        }

        public void TargetObjectClean()
        {
            targetObject = null;
        }

        public void SelectObjectAndOpenMenu(string _selectedObject)
        {
            changeM.ResetChange();
            isNowSelected = true;
            freeCamera.selectedObject = _selectedObject;
            changeM.ShowChangeMenu(_selectedObject);
        }
    }
}
