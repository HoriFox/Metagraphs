using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class ChangeTransform : MonoBehaviour
    {
        public static ChangeTransform Instance;

        private FreeCamera freeCamera;
        private StructureModule structureM;
        private PredicateModule predicateM;

        public Text coordinates;
        public Transform visualStyleToggle;
        private Toggle visualStyle;
        private bool isChangedStyle = false;

        [HideInInspector] public LogicModule logicM;
        [HideInInspector] public ResourceManager resourceM;
        [HideInInspector] public Vector3 positionSelected = new Vector3(0f, 0f, 0f);
        [HideInInspector] public string saveSelectName = null;

        private Vector3 SavePosition = new Vector3(0f, 0f, 0f);

        private GameObject markerObject = null;
        private Structure targetObject = new Structure();

        private void Awake()
        {
            Instance = this;
            freeCamera = Camera.main.GetComponent<FreeCamera>();
        }

        public static ChangeTransform GetInit()
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

            markerObject = Instantiate(resourceM.GetPrefab("LabelPrefab"), positionSelected, Quaternion.Euler(0f, 0f, 0f));
            markerObject.SetActive(false);
        }

        public void UpdateInformation() // TO DO
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

            if (saveSelectName != null && targetObject.position != positionSelected)
            {
                targetObject.position = positionSelected;
                rebuild = true;
            }

            if (rebuild)
            {
                RebuildObject("rebuild");
            }
        }

        private void RebuildObject(string typeRebuild, string name = null)
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

        public void ResetChangeTransform()
        {
            saveSelectName = null;
            freeCamera.selectedObject = null;
            freeCamera.changeTransformMenu.SetActive(false);
            markerObject.SetActive(false);
            positionSelected = new Vector3(0f, 0f, 0f);
        }

        public void DeleteObject(string name = null)
        {
            RebuildObject("delete", name);
            ResetChangeTransform();
        }

        void Update()
        {
            if (markerObject != null)
            {
                Transform transformMarker = markerObject.transform;
                coordinates.text = transformMarker.localPosition.ToString();

                if (transformMarker.localPosition != SavePosition)
                {
                    SavePosition = transformMarker.localPosition;
                    positionSelected = SavePosition;
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    DeleteObject();
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ResetChangeTransform();
                }
            }
            // Одноразовое включение заселекченного объекта.
            if (saveSelectName != freeCamera.selectedObject)
            {
                saveSelectName = freeCamera.selectedObject;
                if (saveSelectName != null)
                {
                    targetObject = structureM.structure[saveSelectName];
                    markerObject.transform.position = targetObject.GetPosition();
                    markerObject.SetActive(true);
                    visualStyle.isOn = (targetObject.StyleVisualization == "3D") ? true : false;
                    isChangedStyle = false;
                }
                else
                {
                    markerObject.SetActive(false);
                }
            }
        }
    }
}
