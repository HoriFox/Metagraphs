using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace nm
{
    public class ChangeModule : MonoBehaviour
    {
        public static ChangeModule Instance;

        private FreeCamera freeCamera;
        private StructureModule structureM;
        private PredicateModule predicateM;

        private InteractionModule interactionM;

        public GameObject changePanel;
        //public GameObject changeTransform;
        //public GameObject changeInformation;

        public InputField nameCurrentTarget;
        //public Text typeTarget;
        public GameObject nameCurrentWarning;

        public InputField descriptionCurrentTarget;

        public InputField inputFieldX;
        public InputField inputFieldY;
        public InputField inputFieldZ;

        public Text redValue;
        public Text greenValue;
        public Text blueValue;

        public Slider redSlider;
        public Slider greenSlider;
        public Slider blueSlider;

        public Toggle direction;
        public Toggle typeLine;

        public Toggle transformChangeButton;
        public Toggle graphChangeButton;
        public Toggle edgeChangeButton;

        public Text arcAngleValue;
        public Text factorBendingValue;

        public Slider factorBendingSlider;
        public Slider arcAngleSlider;

        public Text modelValue;
        public Text scaleModelValue;
        public Text scaleSelectorMarkerValue;

        public Slider sliderScaleModel;
        public Slider sliderScaleSelectorMarker;

        //public Transform visualStyleToggle;

        private GUIChangeModule guiChangeM;
        private ResourceManager resourceM;
        [HideInInspector] public Vector3 positionSelected = new Vector3(0f, 0f, 0f);
        [HideInInspector] public string saveSelectName = null;

        private Vector3 SavePosition = new Vector3(0f, 0f, 0f);
        public GameObject markerObject;
        //public GameObject markerObjectCenter;
        //private float timeToHideMarker = 0f;
        //private bool momentHideMarkerCenter = false;

        [HideInInspector] public bool isSelectStage = false;


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

            //markerObject = Instantiate(resourceM.GetPrefab("Marker"), positionSelected, Quaternion.Euler(0f, 0f, 0f));
            //markerObject.SetActive(false);
        }

        // ПОДТВЕРЖДЕНИЕ ИЗМЕНЕНИЯ ГРАФА.
        public void UpdateChange()
        {
            // Если позиции действительно сменились, то задаём их.
            if (!interactionM.targetObject.Static)
            {
                if (saveSelectName != null && interactionM.targetObject.GetPosition(0) != positionSelected && positionSelected != new Vector3(0f, 0f, 0f))
                {
                    interactionM.targetObject.Position[0] = positionSelected;
                    RebuildObject("rebuild");
                }
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
            //Debug.Log(name);
            nameCurrentTarget.text = name;
            descriptionCurrentTarget.text = structureM.structure[name].Description;

            changePanel.SetActive(true);
            transformChangeButton.isOn = true;

            //Если элемент статический (Edge и Metaedge).
            if (structureM.structure[name].Static)
            {
                inputFieldX.interactable = false;
                inputFieldY.interactable = false;
                inputFieldZ.interactable = false;
                edgeChangeButton.interactable = true;
                graphChangeButton.interactable = false;
            }
            else
            {
                inputFieldX.interactable = true;
                inputFieldY.interactable = true;
                inputFieldZ.interactable = true;
                edgeChangeButton.interactable = false;
                graphChangeButton.interactable = true;
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
            changePanel.SetActive(false);
            nameCurrentWarning.SetActive(false);
            //guiChangeM.CheckChange();
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

            // Если сейчас что-то выделено и на нём висит красный Маркер.
            if (markerObject.activeInHierarchy)
            {
                //if (momentHideMarkerCenter)
                //{
                //    if (Time.time > timeToHideMarker)
                //    {
                //        markerObjectCenter.SetActive(true);
                //        momentHideMarkerCenter = false;
                //    }
                //    else
                //    {
                //        markerObjectCenter.SetActive(false);
                //    }
                //}

                Vector3 transformMarker = markerObject.transform.localPosition;
                inputFieldX.text = transformMarker.x.ToString("0.00");
                inputFieldY.text = transformMarker.y.ToString("0.00");
                inputFieldZ.text = transformMarker.z.ToString("0.00");

                // Если поменяли позиции маркера.
                if (transformMarker != SavePosition)
                {
                    // ОШИБКА ВЫДИЛЕНИЯ! TO DO (Я закоментировал, пока никаких неверностей)
                    // Сбиваем соединение, ибо сместили.
                    //interactionM.isConnection = false;
                    //interactionM.SelectActive(interactionM.startConnectionObject, false);
                    //interactionM.startConnectionObject = null;

                    SavePosition = transformMarker;
                    positionSelected = SavePosition;
                }

                // DO TO. KEY
                if (Input.GetKeyDown(KeyCode.Return) && interactionM.isNowSelected)
                {
                    UpdateChange();
                }
            }
            // Одноразовое включение заселекченного объекта.
            if (saveSelectName != freeCamera.selectedObject)
            {
                // Установка флага работы select.
                isSelectStage = true;

                saveSelectName = freeCamera.selectedObject;
                if (saveSelectName != null)
                {
                    interactionM.targetObject = structureM.structure[saveSelectName];
                    if (!interactionM.targetObject.Static)
                    {
                        markerObject.transform.position = interactionM.targetObject.GetPosition(0);
                        markerObject.SetActive(true);

                        float scale = interactionM.targetObject.gameObject[0].GetComponent<TooltipText>().sizeSelectMarker;
                        markerObject.transform.localScale = new Vector3(scale, scale, scale);
                    }
                    else
                    {
                        markerObject.transform.localPosition = Vector3.zero;

                        foreach (var part in interactionM.targetObject.gameObject)
                        {
                            part.gameObject.GetComponent<Outline>().enabled = true;
                        }
                    }
                    guiChangeM.OpenInformation();

                    redValue.text = interactionM.targetObject.color.r.ToString("0.");
                    redSlider.value = interactionM.targetObject.color.r;
                    greenValue.text = interactionM.targetObject.color.g.ToString("0.");
                    greenSlider.value = interactionM.targetObject.color.g;
                    blueValue.text = interactionM.targetObject.color.b.ToString("0.");
                    blueSlider.value = interactionM.targetObject.color.b;

                    direction.isOn = interactionM.targetObject.Eo;
                    typeLine.isOn = interactionM.targetObject.Arc;
                    factorBendingSlider.value = interactionM.targetObject.HeightArc;
                    factorBendingValue.text = interactionM.targetObject.HeightArc.ToString("0.0");
                    arcAngleSlider.value = interactionM.targetObject.AngleArc;
                    arcAngleValue.text = interactionM.targetObject.AngleArc.ToString("0.");

                    modelValue.text = interactionM.targetObject.NameModel;
                    scaleModelValue.text = interactionM.targetObject.ScaleModel.ToString("0.0");
                    sliderScaleModel.value = interactionM.targetObject.ScaleModel;
                    scaleSelectorMarkerValue.text = interactionM.targetObject.ScaleSelectMarker.ToString("0.0");
                    sliderScaleSelectorMarker.value = interactionM.targetObject.ScaleSelectMarker;
                }
                else
                {
                    // Если контейнер выбранного объекта пуст, то выключаем маркер.
                    markerObject.SetActive(false);
                }

                Invoke("ClearIgnoreBool", 0.1f);
            }
        }

        public void ClearIgnoreBool()
        {
            isSelectStage = false;
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
            float? valueY = float.Parse(value);
            if (valueY != null)
            {
                currentPostion.y = valueY.GetValueOrDefault();
                markerObject.transform.localPosition = currentPostion;
            }
        }

        public void UpdatePositionFieldZ(string value)
        {
            Vector3 currentPostion = markerObject.transform.localPosition;
            float? valueZ = float.Parse(value);
            if (valueZ != null)
            {
                currentPostion.z = valueZ.GetValueOrDefault();
                markerObject.transform.localPosition = currentPostion;
            }
        }

        public void UpdateFactorBending(float value)
        {
            // Защита от On Value Change при установке стандартных значений при открытии.
            if (!isSelectStage)
            {
                factorBendingValue.text = value.ToString("0.0");

                Structure updateObject = structureM.structure[freeCamera.selectedObject];
                structureM.structure[freeCamera.selectedObject].HeightArc = value;

                DeleteObject(updateObject.gameObject);
                predicateM.TactBuild(updateObject.Name, updateObject.ObjectType);
            }
        }

        public void UpdateArcAngle(float value)
        {
            // Защита от On Value Change при установке стандартных значений при открытии.
            if (!isSelectStage)
            {
                arcAngleValue.text = value.ToString("0.");

                Structure updateObject = structureM.structure[freeCamera.selectedObject];
                structureM.structure[freeCamera.selectedObject].AngleArc = value;

                DeleteObject(updateObject.gameObject);
                predicateM.TactBuild(updateObject.Name, updateObject.ObjectType);
            }
        }

        public void UpdateColorRed(float value)
        {
            // Защита от On Value Change при установке стандартных значений при открытии.
            if (!isSelectStage)
            {
                Structure structure = interactionM.targetObject;
                Color32 currentColor = structure.color;
                currentColor.r = (byte)value;
                redValue.text = value.ToString("0.");
                foreach (var part in structure.gameObject)
                {
                    part.GetComponent<Renderer>().material.color = currentColor;
                }
                interactionM.targetObject.color = currentColor;
            }
        }

        public void UpdateColorGreen(float value)
        {
            // Защита от On Value Change при установке стандартных значений при открытии.
            if (!isSelectStage)
            {
                Structure structure = interactionM.targetObject;
                Color32 currentColor = structure.color;
                currentColor.g = (byte)value;
                greenValue.text = value.ToString("0.");
                foreach (var part in structure.gameObject)
                {
                    part.GetComponent<Renderer>().material.color = currentColor;
                }
                interactionM.targetObject.color = currentColor;
            }
        }

        public void UpdateColorBlue(float value)
        {
            // Защита от On Value Change при установке стандартных значений при открытии.
            if (!isSelectStage)
            {
                Structure structure = interactionM.targetObject;
                Color32 currentColor = structure.color;
                currentColor.b = (byte)value;
                blueValue.text = value.ToString("0.");
                foreach (var part in structure.gameObject)
                {
                    part.GetComponent<Renderer>().material.color = currentColor;
                }
                interactionM.targetObject.color = currentColor;
            }
        }

        public void UpdateNameObject(string value)
        {
            if (value != interactionM.targetObject.Name && value != "")
            {
                if (structureM.structure.ContainsKey(value))
                {
                    // Имя уже существует. Ошибка.
                    nameCurrentWarning.SetActive(true);
                    return;
                }
                else
                {
                    Structure intermediate = interactionM.targetObject;

                    foreach (var part in intermediate.ChildStructures)
                    {
                        // Для корректного отображения в редакторе связей.
                        if (part.Value.ParentStructuresKeys != null)
                        {
                            for (int i = 0; i < part.Value.ParentStructuresKeys.Length; i++)
                            {
                                if (part.Value.ParentStructuresKeys[i] == intermediate.Name)
                                {
                                    part.Value.ParentStructuresKeys[i] = value;
                                }
                            }
                        }

                        part.Value.ParentStructures.Remove(intermediate.Name);
                        part.Value.ParentStructures.Add(value, intermediate);
                    }

                    foreach (var part in intermediate.ParentStructures)
                    {
                        // Для корректного отображения в редакторе связей.
                        if (part.Value.ChildStructuresKeys != null)
                        {
                            for (int i = 0; i < part.Value.ChildStructuresKeys.Length; i++)
                            {
                                if (part.Value.ChildStructuresKeys[i] == intermediate.Name)
                                {
                                    part.Value.ChildStructuresKeys[i] = value;
                                }
                            }
                        }

                        part.Value.ChildStructures.Remove(intermediate.Name);
                        part.Value.ChildStructures.Add(value, intermediate);
                    }

                    structureM.structure.Remove(intermediate.Name);

                    intermediate.Name = value;

                    freeCamera.selectedObject = value;
                    saveSelectName = value;

                    structureM.structure.Add(value, intermediate);

                    DeleteObject(intermediate.gameObject);
                    predicateM.TactBuild(intermediate.Name, intermediate.ObjectType);
                }
            }
            nameCurrentWarning.SetActive(false);
        }

        public void UpdateArc(bool value)
        {
            // Защита от On Value Change при установке стандартных значений при открытии.
            if (!isSelectStage)
            {
                Structure updateObject = structureM.structure[freeCamera.selectedObject];
                structureM.structure[freeCamera.selectedObject].Arc = value;

                DeleteObject(updateObject.gameObject);
                predicateM.TactBuild(updateObject.Name, updateObject.ObjectType);
            }
        }

        public void UpdateDirection(bool value)
        {
            // Защита от On Value Change при установке стандартных значений при открытии.
            if (!isSelectStage)
            {
                Structure updateObject = structureM.structure[freeCamera.selectedObject];
                structureM.structure[freeCamera.selectedObject].Eo = value;

                DeleteObject(updateObject.gameObject);
                predicateM.TactBuild(updateObject.Name, updateObject.ObjectType);
            }
        }

        public void UpdateDescription(string value)
        {
            Structure updateObject = structureM.structure[freeCamera.selectedObject];
            structureM.structure[freeCamera.selectedObject].Description = value;

            DeleteObject(updateObject.gameObject);
            predicateM.TactBuild(updateObject.Name, updateObject.ObjectType);
        }

        public void LoadModel(string filePath)
        {
            string _path = Path.GetDirectoryName(filePath);
            string _nameModel = Path.GetFileName(filePath);

            structureM.structure[freeCamera.selectedObject].NameModel = _nameModel;
            modelValue.text = _nameModel;

            List<GameObject> gameObjectList = structureM.structure[freeCamera.selectedObject].gameObject;
            if (gameObjectList.Count == 1)
            {
                gameObjectList[0].GetComponent<DemoLoadObj>().LoadModel(_path, _nameModel);
            }
        }

        public void ResetModel()
        {
            Structure updateObject = structureM.structure[freeCamera.selectedObject];
            structureM.structure[freeCamera.selectedObject].NameModel = null;
            modelValue.text = "";

            DeleteObject(updateObject.gameObject);
            predicateM.TactBuild(updateObject.Name, updateObject.ObjectType);
        }

        public void UpdateScaleModel(float value)
        {
            structureM.structure[freeCamera.selectedObject].ScaleModel = value;
            scaleModelValue.text = value.ToString("0.0");

            List<GameObject> gameObjectList = structureM.structure[freeCamera.selectedObject].gameObject;
            if (gameObjectList.Count == 1)
            {
                gameObjectList[0].transform.localScale = new Vector3(value, value, value);
            }
        }

        public void UpdateScaleSelectorMarker(float value)
        {
            structureM.structure[freeCamera.selectedObject].ScaleSelectMarker = value;
            structureM.structure[freeCamera.selectedObject].ScaleSelectMarker = value;

            scaleSelectorMarkerValue.text = value.ToString("0.0");
            markerObject.transform.localScale = new Vector3(value, value, value);
            GameObject graph = structureM.structure[freeCamera.selectedObject].gameObject[0];
            graph.GetComponent<TooltipText>().sizeSelectMarker = value;
        }

        public void ResetScaleSetting()
        {
            InitObject initObjectM = InitObject.GetInstance();
            UpdateScaleModel(initObjectM.defaultScaleGraph);
            UpdateScaleSelectorMarker(initObjectM.defaultScaleSelectorMarker);
        }

    }
}
