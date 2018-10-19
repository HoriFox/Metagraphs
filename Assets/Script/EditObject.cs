using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class EditObject : MonoBehaviour
    {
        //private string currentNameObject = null;

        // Action раздел
        public Text nameEditWindow;
        public Text actionText;
        public GameObject editMenu;
        public InputField nameObject;
        public Dropdown typebject;
        public Toggle metabject;

        // Environment раздел
        public InputField environmentName;
        public Dropdown environmentType;

        private string currentState = null;
        private StructureModule structureModule;
        private PredicateModule predicateModule;
        //private ChangeTransform changeTransform;

        private void Start()
        {
            structureModule = StructureModule.GetInit();
            predicateModule = PredicateModule.GetInit();
            //changeTransform = ChangeTransform.GetInit();
        }

        public void AddEnvironment()
        {
            //predicateModule.TactBuild(nameObject.text, typeObject);
        }

        public void DeleteEnvironment()
        {
            //changeTransform.DeleteObject(environmentName.text);
        }

        public void Create()
        {
            currentState = "create";
            nameEditWindow.text = "Create";
            actionText.text = "Create";
            editMenu.SetActive(true);
            nameObject.gameObject.SetActive(true);
            typebject.gameObject.SetActive(true);
            metabject.gameObject.SetActive(true);
        }

        public void Edit()
        {
            currentState = "edit";
            nameEditWindow.text = "Edit";
            actionText.text = "Apply";
            editMenu.SetActive(true);
            // ...
        }

        public void Delete()
        {
            currentState = "delete";
            nameEditWindow.text = "Delete";
            actionText.text = "Delete";
            editMenu.SetActive(true);
            // ...
        }

        public void Action()
        {
            switch (currentState)
            {
                case "create":
                    structureModule.AddNode(nameObject.text);
                    string typeObject = null;
                    switch(typebject.value)
                    {
                        case 0:
                            if (metabject.isOn)
                                typeObject = "Metavertex";
                            else
                                typeObject = "Vertex";
                            break;
                        case 1:
                            if (metabject.isOn)
                                typeObject = "Metaedge";
                            else
                                typeObject = "Edge";
                            break;
                        case 2:
                            if (metabject.isOn)
                                typeObject = "Metagraph";
                            else
                                typeObject = "Graph";
                            break;
                        //case 3:
                        //    typeObject = "Attribute";
                        //    break;
                    }
                    structureModule.AddNodeData(nameObject.text, typeObject);
                    predicateModule.TactBuild(nameObject.text, typeObject);
                    break;
                case "edit":

                    break;
                case "delete":

                    break;
            }

        }
    }
}
