using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class ChangeTransformInterface : MonoBehaviour
    {
        public Transform _camera;
        private FreeCamera freeC;
        private StructureModule structureM;
        private PredicateModule predicateM;

        [HideInInspector] public ResourceManager resourceM;
        [HideInInspector] public Vector3 positionSelected = new Vector3(0f, 0f, 0f);
        [HideInInspector] public string saveSelectName = null;

        public Transform inputXPosition;
        private InputField inputXPositionOut;

        public Transform inputYPosition;
        private InputField inputYPositionOut;

        public Transform inputZPosition;
        private InputField inputZPositionOut;

        private string SaveXPosition = null;
        private string SaveYPosition = null;
        private string SaveZPosition = null;

        private GameObject markerObject = null;
        private Vector3 newPosition;
        private Structure targetObject = null;

        private void Awake()
        {
            freeC = _camera.GetComponent<FreeCamera>();
        }

        private void Start()
        {
            structureM = StructureModule.GetInit();
            predicateM = PredicateModule.GetInit();
            resourceM = ResourceManager.GetInstance();

            inputXPositionOut = inputXPosition.GetComponent<InputField>();
            inputYPositionOut = inputYPosition.GetComponent<InputField>();
            inputZPositionOut = inputZPosition.GetComponent<InputField>();
        }

        // Если честно, то мне самому страшно на это смотреть. 
        // Слава богу, что эта операция происходит не в Update.
        public void UpdatePosition()
        {
            if (saveSelectName != null && targetObject.position != positionSelected)
            {
                // Говорим, что теперь выделенный объект использует свои координаты.
                //targetObject.isUsingCustomPosition = true;
                targetObject.position = positionSelected;

                // Пересоздаём всех детей.
                foreach (var part in targetObject.ChildStructures)
                {
                    DeleteObject(part.Value.gameObject);
                    predicateM.TactBuild(part.Value.Name, part.Value.ObjectType);
                }

                // Пересоздаём всех родителей.
                foreach (var part in targetObject.ParentStructures)
                {
                    DeleteObject(part.Value.gameObject);
                    predicateM.TactBuild(part.Value.Name, part.Value.ObjectType);
                }

                // Пересоздаём себя.
                DeleteObject(targetObject.gameObject);
                predicateM.TactBuild(saveSelectName, targetObject.ObjectType);
            }
        }

        private void DeleteObject(List<GameObject> gameObject)
        {
            int k = 0;
            foreach (var part in gameObject)
            {
                Destroy(part);
                Debug.Log("Удалили объект с локальным индексом: " + k);
                k++;
            }
        }

        public void ResetChangeTransform()
        {
            saveSelectName = null;
            freeC.selectedObject = null;
            freeC.changeTransformMenu.SetActive(false);
            Destroy(markerObject);
            positionSelected = new Vector3(0f, 0f, 0f);
        }

        void Update()
        {
            if (markerObject != null)
            {
                if (inputXPositionOut.text != SaveXPosition)
                {
                    SaveXPosition = inputXPositionOut.text;
                    newPosition.x = float.Parse(SaveXPosition);
                }
                if (inputYPositionOut.text != SaveYPosition)
                {
                    SaveYPosition = inputYPositionOut.text;
                    newPosition.y = float.Parse(SaveYPosition);
                }
                if (inputZPositionOut.text != SaveZPosition)
                {
                    SaveZPosition = inputZPositionOut.text;
                    newPosition.z = float.Parse(SaveZPosition);
                }

                if (newPosition.x != positionSelected.x || newPosition.y != positionSelected.y || newPosition.z != positionSelected.z)
                {
                    positionSelected = newPosition;
                    markerObject.transform.position = new Vector3(newPosition.x, newPosition.y, newPosition.z);
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ResetChangeTransform();
                }
            }
            // Одноразовое включение заселекченного объекта.
            if (saveSelectName != freeC.selectedObject)
            {
                saveSelectName = freeC.selectedObject;
                if (saveSelectName != null)
                {
                    targetObject = structureM.structure[saveSelectName];
                    positionSelected = targetObject.GetPosition();

                    newPosition.x = positionSelected.x;
                    inputXPositionOut.text = positionSelected.x.ToString();

                    newPosition.y = positionSelected.y;
                    inputYPositionOut.text = positionSelected.y.ToString();

                    newPosition.z = positionSelected.z;
                    inputZPositionOut.text = positionSelected.z.ToString();

                    Destroy(markerObject);
                    markerObject = Instantiate(resourceM.GetPrefab("LabelPrefab"), positionSelected, Quaternion.Euler(0f, 0f, 0f));
                }
                else
                {
                    Destroy(markerObject);
                }
            }
        }
    }
}
