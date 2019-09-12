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
        private static GUIChangeModule init;


        public Text nameCurrentTarget;
        public Text typeTarget;

        //public InputField nameStart;
        //public InputField nameEnd;
        //public Dropdown typeEnvironment;
        //public InputField nameEnvironment;
        public Transform scrollViewParent;
        public Transform scrollViewChild;

        private void Awake()
        {
            init = this;
        }

        private void Start()
        {
            structureM = StructureModule.GetInit();
            changeM = ChangeModule.GetInit();
        }

        //public void AddEnvironment()
        //{
        //    if (nameEnvironment.text != null && structureM.IsExistNode(nameEnvironment.text) && nameEnvironment.text != changeM.saveSelectName)
        //    {
        //        if (typeEnvironment.value == 0)
        //        {
        //            var parentStructuresDict = structureM.structure[changeM.saveSelectName].ParentStructures;
        //            if (!parentStructuresDict.ContainsKey(nameEnvironment.text))
        //            {
        //                parentStructuresDict.Add(nameEnvironment.text, structureM.structure[nameEnvironment.text]);
        //            }
        //        }
        //        if (typeEnvironment.value == 1)
        //        {
        //            var childStructuresDict = structureM.structure[changeM.saveSelectName].ChildStructures;
        //            if (!childStructuresDict.ContainsKey(nameEnvironment.text))
        //            {
        //                childStructuresDict.Add(nameEnvironment.text, structureM.structure[nameEnvironment.text]);
        //            }
        //        }
        //    }
        //    changeM.RebuildObject("rebuild", changeM.saveSelectName);
        //}

        //public void DeleteEnvironment()
        //{
        //    Structure selectStructure = structureM.structure[changeM.saveSelectName];
        //    if (nameEnvironment.text != null)
        //    {
        //        string[] array = null;
        //        if (typeEnvironment.value == 0)
        //            array = selectStructure.ParentStructuresKeys;
        //        if (typeEnvironment.value == 1)
        //            array = selectStructure.ChildStructuresKeys;

        //        int i = 0;
        //        foreach (var part in array)
        //        {
        //            if (part == nameEnvironment.text)
        //            {
        //                array[i] = null;
        //            }
        //            i++;
        //        }

        //        array = array.Where(x => x != null).ToArray();
        //        if (typeEnvironment.value == 0)
        //        {
        //            selectStructure.ParentStructuresKeys = array;
        //            selectStructure.ParentStructures.Remove(nameEnvironment.text);
        //        }
        //        if (typeEnvironment.value == 1)
        //        {
        //            selectStructure.ChildStructuresKeys = array;
        //            selectStructure.ChildStructures.Remove(nameEnvironment.text);
        //        }
        //        changeM.RebuildObject("rebuild", changeM.saveSelectName);
        //    }
        //}

        //public void OutLog(string message)
        //{
        //    Debug.Log(message);
        //}

        public void OpenInformation()
        {
            nameCurrentTarget.text = changeM.saveSelectName;
            Structure structure = structureM.structure[changeM.saveSelectName];
            typeTarget.text = structure.ObjectType;
            ScrollViewHelper viewHelperParent = scrollViewParent.GetComponent<ScrollViewHelper>();
            ScrollViewHelper viewHelperChild = scrollViewChild.GetComponent<ScrollViewHelper>();
            viewHelperParent.ResetList();
            viewHelperChild.ResetList();
            viewHelperParent.ShowList(structureM.structure[changeM.saveSelectName].ParentStructuresKeys);
            viewHelperChild.ShowList(structureM.structure[changeM.saveSelectName].ChildStructuresKeys);
        }

        public void CheckChange()
        {
            //if (startName != null && startName != nameCurrentTarget.text)
            //{
            //    structureM.EditNodeData(startName, nameCurrentTarget.text);
            //}
        }

        // Добавить выбранное окружение.
        public void AddEnvironment()
        {
            Debug.Log("Добавить в папку");
        }

        //Не могу поставить на префаб данную функцию.
        //// Удалить выбранное окружение.
        //public void DeleteEnvironment()
        //{
        //    Debug.Log("Удалить из папки");
        //}

        //public void UpdateInformation()
        //{

        //}

        public static GUIChangeModule GetInit()
        {
            return init;
        }
    }
}
