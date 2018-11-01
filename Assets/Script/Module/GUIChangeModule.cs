using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class GUIChangeModule : MonoBehaviour
    {
        private StructureModule structureM;
        private ChangeModule changeM;
        //private OutlogModule outlogM;
        private static GUIChangeModule init;

        public Text nameTarget;
        public Text typeTarget;
        public Toggle oeToggle;
        public InputField nameStart;
        public InputField nameEnd;
        public Dropdown typeEnvironment;
        public InputField nameEnvironment;
        public Transform scrollViewParent;
        public Transform scrollViewChild;

        private void Awake()
        {
            init = this;
        }

        private void Start()
        {
            structureM = StructureModule.GetInit();
            //outlogM = OutlogModule.GetInit();
            changeM = ChangeModule.GetInit();
        }

        public void AddEnvironment()
        {
            if (nameEnvironment.text != null && structureM.IsExistNode(nameEnvironment.text) && nameEnvironment.text != changeM.saveSelectName)
            {
                if (typeEnvironment.value == 0)
                {
                    if (!structureM.structure[changeM.saveSelectName].ParentStructures.ContainsKey(nameEnvironment.text))
                    {
                        structureM.structure[changeM.saveSelectName].ParentStructures.Add(nameEnvironment.text, structureM.structure[nameEnvironment.text]);
                    }
                }
                if (typeEnvironment.value == 1)
                {
                    if (!structureM.structure[changeM.saveSelectName].ChildStructures.ContainsKey(nameEnvironment.text))
                    {
                        structureM.structure[changeM.saveSelectName].ChildStructures.Add(nameEnvironment.text, structureM.structure[nameEnvironment.text]);
                    }
                }
            }
            //outlogM.OutTooltip(currentName:changeM.saveSelectName);
            changeM.RebuildObject("rebuild", changeM.saveSelectName);
        }

        public void DeleteEnvironment()
        {
            Debug.Log("Edit +" + nameEnvironment.text);
            if (nameEnvironment.text != null)
            {
                string[] array = null;
                if (typeEnvironment.value == 0)
                    array = structureM.structure[changeM.saveSelectName].ParentStructuresKeys;
                if (typeEnvironment.value == 1)
                    array = structureM.structure[changeM.saveSelectName].ChildStructuresKeys;

                int i = 0;
                foreach (var part in array)
                {
                    if (part == nameEnvironment.text)
                    {
                        array[i] = null;
                    }
                    i++;
                }

                array = array.Where(x => x != null).ToArray();
                if (typeEnvironment.value == 0)
                {
                    structureM.structure[changeM.saveSelectName].ParentStructuresKeys = array;
                    structureM.structure[changeM.saveSelectName].ParentStructures.Remove(nameEnvironment.text);
                }
                if (typeEnvironment.value == 1)
                {
                    structureM.structure[changeM.saveSelectName].ChildStructuresKeys = array;
                    structureM.structure[changeM.saveSelectName].ChildStructures.Remove(nameEnvironment.text);
                }
                //outlogM.OutTooltip(currentName: changeM.saveSelectName);
                changeM.RebuildObject("rebuild", changeM.saveSelectName);
            }
        }

        public void OpenInformation()
        {
            nameTarget.text = changeM.saveSelectName;
            Structure structure = structureM.structure[changeM.saveSelectName];
            typeTarget.text = structure.ObjectType;
            ScrollViewHelper viewHelperParent = scrollViewParent.GetComponent<ScrollViewHelper>();
            ScrollViewHelper viewHelperChild = scrollViewChild.GetComponent<ScrollViewHelper>();
            viewHelperParent.ResetList();
            viewHelperChild.ResetList();
            viewHelperParent.ShowList(structureM.structure[changeM.saveSelectName].ParentStructuresKeys);
            viewHelperChild.ShowList(structureM.structure[changeM.saveSelectName].ChildStructuresKeys);
        }

        public void UpdateInformation()
        {
            //structureM.structure[saveSelectName];
        }

        public static GUIChangeModule GetInit()
        {
            return init;
        }
    }
}
