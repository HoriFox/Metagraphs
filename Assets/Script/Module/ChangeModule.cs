using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class ChangeModule : MonoBehaviour
    {
        public static ChangeModule Instance;

        private FreeCamera freeCamera;
        private StructureModule structureM;
        private PredicateModule predicateM;

        private InteractionModule interactionM;

        public GameObject changeTransform;
        public GameObject changeInformation;

        public InputField inputFieldX;
        public InputField inputFieldY;
        public InputField inputFieldZ;

        public Transform visualStyleToggle;

        private GUIChangeModule guiChangeM;
        private ResourceManager resourceM;
        [HideInInspector] public Vector3 positionSelected = new Vector3(0f, 0f, 0f);
        [HideInInspector] public string saveSelectName = null;

        private Vector3 SavePosition = new Vector3(0f, 0f, 0f);
        private GameObject markerObject = null;

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
            interactionM = InteractionModule.GetInit();
            structureM = StructureModule.GetInit();
            predicateM = PredicateModule.GetInit();
            resourceM = ResourceManager.GetInstance();
            guiChangeM = GUIChangeModule.GetInit();

            markerObject = Instantiate(resourceM.GetPrefab("LabelPrefab"), positionSelected, Quaternion.Euler(0f, 0f, 0f));
            markerObject.SetActive(false);
        }

        // ПОДТВЕРЖДЕНИЕ ИЗМЕНЕНИЯ ГРАФА.
        public void UpdateChange()
        {
            // Если позиции действительно сменились, то задаём их.
            if (saveSelectName != null && interactionM.targetObject.GetPosition(0) != positionSelected && positionSelected != new Vector3(0f, 0f, 0f))
            {
                interactionM.targetObject.Position[0] = positionSelected;
                RebuildObject("rebuild");
            }
        }

        // Пересобирает окружение в зависимости от режима (удаление части или перестройка), требуется имя части.
        public void RebuildObject(string typeRebuild, string name = null)
        {
            Structure target = (name != null) ? structureM.structure[name] : interactionM.targetObject;

            // Пересоздаём всех детей.
            //foreach (var part in target.ChildStructures)
            //{
            //    DeleteObject(part.Value.gameObject);
            //    if (typeRebuild == "rebuild")
            //    {
            //        predicateM.TactBuild(part.Value.Name, part.Value.ObjectType);
            //    }
            //}

            // Пересоздаём всех детей.
            foreach (var part in target.ChildStructures)
            {
                DeleteObject(part.Value.gameObject);
                if (typeRebuild == "delete")
                {
                    part.Value.ParentStructures.Remove(target.Name);
                }
                predicateM.TactBuild(part.Value.Name, part.Value.ObjectType);
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

        // Удаляет поданный список gameObject.
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
            //interactionM.isPanelActive = true;
            changeInformation.SetActive(true);
            //Если элемент не статический (не Edge и Metaedge).
            if (!structureM.structure[name].Static)
            {
                changeTransform.SetActive(true);
            }
        }

        // Если нажали Esc или переключились на другой объект.
        public void ResetChange()
        {
            interactionM.isNowSelected = false;

            interactionM.isConnection = false;
            interactionM.SelectActive(interactionM.startConnectionObject, false);
            interactionM.startConnectionObject = null;

            //interactionM.isPanelActive = false;
            saveSelectName = null;
            freeCamera.selectedObject = null;
            changeTransform.SetActive(false);
            guiChangeM.CheckChange();
            changeInformation.SetActive(false);
            markerObject.SetActive(false);
            positionSelected = new Vector3(0f, 0f, 0f);
            if (interactionM.targetObject != null && interactionM.targetObject.Static)
            {
                foreach (var part in interactionM.targetObject.gameObject)
                {
                    part.GetComponent<Outline>().enabled = false;
                }
            }
            interactionM.targetObject = null;
        }

        void Update()
        {
            // Если сейчас что-то выделено и на нём висит красный Маркер.
            if (markerObject != null)
            {
                Vector3 transformMarker = markerObject.transform.localPosition;
                inputFieldX.text = transformMarker.x.ToString("0.00");
                inputFieldY.text = transformMarker.y.ToString("0.00");
                inputFieldZ.text = transformMarker.z.ToString("0.00");

                // Если поменяли позиции маркера.
                if (transformMarker != SavePosition)
                {
                    // Сбиваем соединение, ибо сместили.
                    interactionM.isConnection = false;
                    interactionM.SelectActive(interactionM.startConnectionObject, false);
                    interactionM.startConnectionObject = null;

                    SavePosition = transformMarker;
                    positionSelected = SavePosition;
                }
                // DO TO. KEY
                if (Input.GetKeyDown(KeyCode.Return) && interactionM.isNowSelected)
                {
                    UpdateChange();
                }
                // DO TO. KEY
                if (Input.GetKeyDown(KeyCode.Delete) && interactionM.isNowSelected)
                {
                    RebuildObject("delete");
                    ResetChange();
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
                    interactionM.targetObject = structureM.structure[saveSelectName];
                    if (!interactionM.targetObject.Static)
                    {
                        markerObject.transform.position = interactionM.targetObject.GetPosition(0);
                        markerObject.SetActive(true);
                    }
                    else
                    {
                        foreach (var part in interactionM.targetObject.gameObject)
                        {
                            part.GetComponent<Outline>().enabled = true;
                        }
                    }
                    guiChangeM.OpenInformation();
                }
                else
                {
                    // Если контейнер выбранного объекта пуст, то выключаем маркер.
                    markerObject.SetActive(false);
                }
            }
        }

        public void UpdatePositionFieldX(string value)
        {
            Vector3 currentPostion = markerObject.transform.localPosition;
            float? valueX = float.Parse(value);
            if (valueX != null)
            {
                currentPostion.x = valueX.GetValueOrDefault();
                markerObject.transform.localPosition = currentPostion;
            }
        }

        public void UpdatePositionFieldY(string value)
        {
            Vector3 currentPostion = markerObject.transform.localPosition;
            currentPostion.y = float.Parse(value);
            markerObject.transform.localPosition = currentPostion;
        }

        public void UpdatePositionFieldZ(string value)
        {
            Vector3 currentPostion = markerObject.transform.localPosition;
            currentPostion.z = float.Parse(value);
            markerObject.transform.localPosition = currentPostion;
        }

    }
}
