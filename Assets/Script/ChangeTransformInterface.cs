using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class ChangeTransformInterface : MonoBehaviour
    {
        public Transform _camera;
        private FreeCamera freeC;
        private StructureModule structureM;
        private PredicateModule predicateM;

        private void Awake()
        {
            freeC = _camera.GetComponent<FreeCamera>();
        }

        private void Start()
        {
            structureM = StructureModule.GetInit();
            predicateM = PredicateModule.GetInit();
        }

        public Vector3 positionSelectedObject = new Vector3(0f, 0f, 0f);

        public string saveSelectName = null;

        public bool update = false;

        Structure targetObject = null;

        void Update()
        {
            if (saveSelectName != freeC.selectedObject)
            {
                saveSelectName = freeC.selectedObject;
                if (saveSelectName != null)
                {
                    targetObject = structureM.structure[saveSelectName];
                    positionSelectedObject = targetObject.GetPosition();
                }
            }
            if (update)
            {
                if (saveSelectName != null && targetObject.position != positionSelectedObject)
                {
                    //foreach(var part in targetObject.transform)
                    //{
                    //    Destroy(part);
                    //}

                    targetObject.isUsingCustomPosition = true;
                    targetObject.customPosition = positionSelectedObject;
                    predicateM.TactBuild(saveSelectName, targetObject.ObjectType);
                }

                update = false;
            }
        }
    }
}
