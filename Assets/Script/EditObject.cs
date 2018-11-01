using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class EditObject : MonoBehaviour
    {
        public Text nameEditWindow;
        public Text actionText;
        public GameObject editMenu;
        public InputField nameObject;
        public Dropdown typebject;
        public Toggle metabject;

        private StructureModule structureModule;
        private PredicateModule predicateModule;

        private void Start()
        {
            structureModule = StructureModule.GetInit();
            predicateModule = PredicateModule.GetInit();
        }

        public void Action()
        {
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
                        typeObject = "Metagraph";
                    else
                        typeObject = "Graph";
                    break;
            }
            structureModule.AddNodeData(nameObject.text, typeObject);
            predicateModule.TactBuild(nameObject.text, typeObject);
        }
    }
}
