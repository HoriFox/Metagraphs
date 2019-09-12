using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class DetailStructure : MonoBehaviour
    {
        private StructureModule structureM;
        private Text structureDetail;

        private void Start()
        {
            structureM = StructureModule.GetInit();
            structureDetail = GetComponent<Text>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                structureDetail.text = "Детализация структуры (обновить - J)\n";
                foreach (var part in structureM.structure)
                {
                    structureDetail.text += "( ";

                    if (part.Value.ParentStructures.Count > 0)
                    {
                        foreach (var parent in part.Value.ParentStructures)
                        {
                            structureDetail.text += parent.Key + " ";
                        }
                    }
                    else
                    {
                        structureDetail.text += "нет родителей ";
                    }

                    structureDetail.text += ") -- <b>" + part.Value.Name + "</b> -- ( ";

                    if (part.Value.ChildStructures.Count > 0)
                    {
                        foreach (var child in part.Value.ChildStructures)
                        {
                            structureDetail.text += child.Key + " ";
                        }
                    }
                    else
                    {
                        structureDetail.text += "нет детей ";
                    }

                structureDetail.text += ")\n";
                }
            }
        }

    }
}
