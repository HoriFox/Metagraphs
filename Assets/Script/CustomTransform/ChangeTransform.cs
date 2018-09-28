using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class ChangeTransform : MonoBehaviour
    {
        private FreeCamera freeC;
        private StructureModule structureM;
        private PredicateModule predicateM;

        [HideInInspector] public ResourceManager resourceM;
        [HideInInspector] public Vector3 positionSelected = new Vector3(0f, 0f, 0f);
        [HideInInspector] public string saveSelectName = null;

        private Vector3 SavePosition = new Vector3(0f, 0f, 0f);

        private GameObject markerObject = null;
        private Structure targetObject = new Structure();

        private void Awake()
        {
            freeC = Camera.main.GetComponent<FreeCamera>();
        }

        private void Start()
        {
            structureM = StructureModule.GetInit();
            predicateM = PredicateModule.GetInit();
            resourceM = ResourceManager.GetInstance();

            markerObject = Instantiate(resourceM.GetPrefab("LabelPrefab"), positionSelected, Quaternion.Euler(0f, 0f, 0f));
            markerObject.SetActive(false);
        }

        public void UpdatePosition()
        {
            if (saveSelectName != null && targetObject.position != positionSelected)
            {
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
                k++;
            }
            gameObject.Clear();
        }

        public void ResetChangeTransform()
        {
            saveSelectName = null;
            freeC.selectedObject = null;
            freeC.changeTransformMenu.SetActive(false);
            markerObject.SetActive(false);
            positionSelected = new Vector3(0f, 0f, 0f);
        }

        void Update()
        {
            if (markerObject != null)
            {
                if (markerObject.transform.localPosition.x != SavePosition.x)
                {
                    SavePosition.x = markerObject.transform.localPosition.x;
                    positionSelected.x = SavePosition.x;
                }
                if (markerObject.transform.localPosition.y != SavePosition.y)
                {
                    SavePosition.y = markerObject.transform.localPosition.y;
                    positionSelected.y = SavePosition.y;
                }
                if (markerObject.transform.localPosition.z != SavePosition.z)
                {
                    SavePosition.z = markerObject.transform.localPosition.z;
                    positionSelected.z = SavePosition.z;
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
                    markerObject.transform.position = targetObject.GetPosition();
                    markerObject.SetActive(true);
                }
                else
                {
                    markerObject.SetActive(false);
                }
            }
        }
    }
}
