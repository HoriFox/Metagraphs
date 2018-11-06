using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class ChangeModule : MonoBehaviour
    {
        public static ChangeModule Instance;

        public float doubleTapDelay = 0.4f;
        private FreeCamera freeCamera;
        private StructureModule structureM;
        private PredicateModule predicateM;

        public GameObject changeTransform;
        public GameObject changeInformation;

        public Text coordinates;
        public Transform visualStyleToggle;
        private Toggle visualStyle;
        private bool isChangedStyle = false;

        private GUIChangeModule guiChangeM;
        private LogicModule logicM;
        private ResourceManager resourceM;
        [HideInInspector] public Vector3 positionSelected = new Vector3(0f, 0f, 0f);
        [HideInInspector] public string saveSelectName = null;

        private Vector3 SavePosition = new Vector3(0f, 0f, 0f);
        private float tapCount = 0;
        private GameObject markerObject = null;
        private Structure targetObject = new Structure();

        private void Awake()
        {
            Instance = this;
            freeCamera = Camera.main.GetComponent<FreeCamera>();
        }

        public static ChangeModule GetInit()
        {
            return Instance;
        }

        private void Start()
        {
            visualStyle = visualStyleToggle.GetComponent<Toggle>();

            logicM = LogicModule.GetInit();
            structureM = StructureModule.GetInit();
            predicateM = PredicateModule.GetInit();
            resourceM = ResourceManager.GetInstance();
            guiChangeM = GUIChangeModule.GetInit();

            markerObject = Instantiate(resourceM.GetPrefab("LabelPrefab"), positionSelected, Quaternion.Euler(0f, 0f, 0f));
            markerObject.SetActive(false);
        }

        private void DoubleTap(string methodLuck)
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

        public void UpdateChange() // TO DO
        {
            bool rebuild = false;
            string visualStyleStr = (visualStyle.isOn) ? "3D" : "2D";

            if (targetObject.StyleVisualization != visualStyleStr)
            {
                isChangedStyle = true;
            }

            if (isChangedStyle)
            {
                if (visualStyleStr == "3D")
                {
                    targetObject.StyleVisualization = visualStyleStr;
                    logicM.LogicAdd(targetObject.Name);
                    rebuild = true;
                }

                if (visualStyleStr == "2D")
                {
                    targetObject.StyleVisualization = visualStyleStr;
                    logicM.LogicAdd2D(targetObject.Name);
                    rebuild = true;
                }
                isChangedStyle = false;
            }

            if (saveSelectName != null && targetObject.GetPosition(0) != positionSelected)
            {
                targetObject.position[0] = positionSelected;
                rebuild = true;
            }

            if (rebuild)
            {
                RebuildObject("rebuild");
            }
        }

        public void RebuildObject(string typeRebuild, string name = null)
        {
            Structure target;
            if (name != null)
            {
                target = structureM.structure[name];
            }
            else
            {
                target = targetObject;
            }

            // Пересоздаём всех детей.
            foreach (var part in target.ChildStructures)
            {
                DeleteObject(part.Value.gameObject);
                if (typeRebuild == "rebuild")
                {
                    predicateM.TactBuild(part.Value.Name, part.Value.ObjectType);
                }
            }

            // Пересоздаём всех родителей.
            foreach (var part in target.ParentStructures)
            {
                DeleteObject(part.Value.gameObject);
                if (typeRebuild == "delete")
                {
                    part.Value.ChildStructures.Remove(target.Name);
                }
                predicateM.TactBuild(part.Value.Name, part.Value.ObjectType);
            }

            DeleteObject(target.gameObject);
            if (typeRebuild == "delete")
            {
                structureM.structure.Remove(target.Name);
            }
            if (typeRebuild == "rebuild")
            {
                predicateM.TactBuild(saveSelectName, target.ObjectType);
            }
        }

        private void DeleteObject(List<GameObject> gameObject)
        {
            int k = 0;
            foreach (var part in gameObject)
            {
                Destroy(part);
                k++;
            }
            gameObject.Clear();
        }

        public void ShowChangeMenu(string name)
        {
            changeInformation.SetActive(true);
            //Если элемент не статический (не Edge и Metaedge).
            if (!structureM.structure[name].Static)
            {
                changeTransform.SetActive(true);
            }
        }

        public void ResetChange()
        {
            saveSelectName = null;
            freeCamera.selectedObject = null;
            changeTransform.SetActive(false);
            changeInformation.SetActive(false);
            markerObject.SetActive(false);
            positionSelected = new Vector3(0f, 0f, 0f);
            if (targetObject.Static)
            {
                foreach (var part in targetObject.gameObject)
                {
                    part.GetComponent<Outline>().enabled = false;
                }
            }
        }

        public void DeleteObject(string name = null)
        {
            RebuildObject("delete", name);
            ResetChange();
        }

        void Update()
        {
            if (markerObject != null)
            {
                // DO TO. KEY
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    UpdateChange();
                }

                Transform transformMarker = markerObject.transform;
                coordinates.text = transformMarker.localPosition.ToString();

                if (transformMarker.localPosition != SavePosition)
                {
                    SavePosition = transformMarker.localPosition;
                    positionSelected = SavePosition;
                }
                // DO TO. KEY
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    DeleteObject();
                }
                // DO TO. KEY
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ResetChange();
                }
            }
            // Одноразовое включение заселекченного объекта.
            if (saveSelectName != freeCamera.selectedObject)
            {
                saveSelectName = freeCamera.selectedObject;
                if (saveSelectName != null)
                {
                    targetObject = structureM.structure[saveSelectName];
                    if (!targetObject.Static)
                    {
                        markerObject.transform.position = targetObject.GetPosition(0);
                        markerObject.SetActive(true);
                        visualStyle.isOn = (targetObject.StyleVisualization == "3D") ? true : false;
                        isChangedStyle = false;
                    }
                    else
                    {
                        foreach (var part in targetObject.gameObject)
                        {
                            part.GetComponent<Outline>().enabled = true;
                        }
                    }
                    guiChangeM.OpenInformation();
                }
                else
                {
                    markerObject.SetActive(false);
                }
            }
        }
    }
}
