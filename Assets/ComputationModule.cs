using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nm
{
    public class ComputationModule : MonoBehaviour
    {
        private StructureModule structureM;

        void Start()
        {
            structureM = StructureModule.GetInit();
        }



    }
}
