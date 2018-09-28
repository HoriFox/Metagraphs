using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

namespace nm
{
    public class LoadSaveDialog : MonoBehaviour {

        [DllImport("user32.dll")]
        private static extern void OpenFileDialog();
        [HideInInspector]
        public bool showDialogLoadMGPL = false;
        [HideInInspector]
        public bool showDialogLoadJSON = false;
        [HideInInspector]
        public bool showDialogSave = false;

        string fileName;

        System.Windows.Forms.OpenFileDialog lDialog;
        System.Windows.Forms.OpenFileDialog lcDialog;
        System.Windows.Forms.SaveFileDialog sDialog;
        Engine engineM;

        private StructureModule structureM;

        void Awake()
        {
            engineM = Engine.GetInit();

            lDialog = new System.Windows.Forms.OpenFileDialog();
            lcDialog = new System.Windows.Forms.OpenFileDialog();
            sDialog = new System.Windows.Forms.SaveFileDialog();

            lDialog.RestoreDirectory = true;
            lDialog.InitialDirectory = @"C:\\";
            lDialog.Title = "Please select the Metafile file. ";
            lDialog.Filter = "MGPL Files (*.mgpl) | *.mgpl |All Files | *.* ";

            lcDialog.RestoreDirectory = true;
            lcDialog.InitialDirectory = @"C:\\";
            lcDialog.Title = "Please select the Metagraph json file. ";
            lcDialog.Filter = "JSON Files (*.json) | *.json |All Files | *.* ";

            sDialog.AddExtension = true;
            sDialog.OverwritePrompt = true;
            sDialog.RestoreDirectory = true;
            sDialog.InitialDirectory = @"C:\\";
            sDialog.Title = "Save the Metagraph json file. ";
            sDialog.Filter = "JSON Files (*.json) | *.json |All Files | *.* ";
        }


        private void Start()
        {
            structureM = StructureModule.GetInit();
        }

        void Update()
        {
            if (showDialogLoadMGPL == true)
            {
                if (lDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = lDialog.FileName;
                    string content = File.ReadAllText(fileName);
                    engineM.ReadAndBuild(content);
                }
                showDialogLoadMGPL = false;
            }
            if (showDialogLoadJSON == true)
            {
                if (lcDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = lcDialog.FileName;
                    structureM.LoadingJson(fileName);
                }
                showDialogLoadJSON = false;
            }
            if (showDialogSave == true)
            {
                if (sDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = sDialog.FileName;
                    structureM.UnloadingJson(fileName);
                }
                showDialogSave = false;
            }
        }
    }
}
