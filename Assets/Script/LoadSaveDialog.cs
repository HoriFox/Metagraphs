using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

using UnityEditor;

namespace nm
{
    public class LoadSaveDialog : MonoBehaviour {

        //[DllImport("user32.dll")]
        //private static extern void OpenFileDialog();
        //[HideInInspector]
        //public bool showDialogLoadMGPL = false;
        //[HideInInspector]
        //public bool showDialogLoadJSON = false;
        //[HideInInspector]
        //public bool showDialogSave = false;

        //private Reader readerM;
        private LogicModule logicM;
        private PredicateModule predicateM;
        private ChangeModule changeM;

        string fileName;

        //System.Windows.Forms.OpenFileDialog lDialog;
        //System.Windows.Forms.OpenFileDialog lcDialog;
        //System.Windows.Forms.SaveFileDialog sDialog;
        Engine engineM;

        private StructureModule structureM;

        void Awake()
        {
            engineM = Engine.GetInit();

            //lDialog = new System.Windows.Forms.OpenFileDialog();
            //lcDialog = new System.Windows.Forms.OpenFileDialog();
            //sDialog = new System.Windows.Forms.SaveFileDialog();

            //    lDialog.RestoreDirectory = true;
            //    lDialog.InitialDirectory = @"C:\\";
            //    lDialog.Title = "Please select the Metafile file. ";
            //    lDialog.Filter = "MGPL Files (*.mgpl) | *.mgpl |All Files | *.* ";

            //    lcDialog.RestoreDirectory = true;
            //    lcDialog.InitialDirectory = @"C:\\";
            //    lcDialog.Title = "Please select the Metagraph json file. ";
            //    lcDialog.Filter = "JSON Files (*.json) | *.json |All Files | *.* ";

            //    sDialog.AddExtension = true;
            //    sDialog.OverwritePrompt = true;
            //    sDialog.RestoreDirectory = true;
            //    sDialog.InitialDirectory = @"C:\\";
            //    sDialog.Title = "Save the Metagraph json file. ";
            //    sDialog.Filter = "JSON Files (*.json) | *.json |All Files | *.* ";
        }

        //public void ReloadFile()
        //{
        //    if (engineM.lastLoadType == "MGPL")
        //    {
        //        string content = File.ReadAllText(fileName);
        //        readerM.ReadCode(content);
        //        logicM.LogicAdd();
        //        predicateM.BuildGraphs();
        //    }
        //    else if (engineM.lastLoadType == "JSON")
        //    {
        //        structureM.LoadingJson(fileName);
        //    }
        //}

        private void Start()
        {
            //readerM = Reader.GetInit();
            logicM = LogicModule.GetInit();
            predicateM = PredicateModule.GetInit();
            changeM = ChangeModule.GetInit();

            structureM = StructureModule.GetInit();
        }

        public void SaveFile ()
        {
            fileName = EditorUtility.SaveFilePanel("Save the Metagraph json file", "", "", "json");
            if (fileName.Length != 0)
            {
                structureM.UnloadingJson(fileName);
            }
        }

        public void OpenFile ()
        {
            fileName = EditorUtility.OpenFilePanel("Please select the Metafile file", "", "json");
            if (fileName.Length != 0)
            {
                changeM.ResetChange();
                structureM.LoadingJson(fileName);
            }
        }


        //    void Update()
        //    {
        //        if (showDialogLoadMGPL == true)
        //        {
        //            if (lDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //            {
        //                changeM.ResetChange();
        //                fileName = lDialog.FileName;
        //                string content = File.ReadAllText(fileName);
        //                readerM.ReadCode(content);
        //                logicM.LogicAdd();
        //                predicateM.BuildGraphs();
        //                engineM.lastLoadType = "MGPL";
        //            }
        //            showDialogLoadMGPL = false;
        //        }
        //        if (showDialogLoadJSON == true)
        //        {
        //            if (lcDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //            {
        //                changeM.ResetChange();
        //                fileName = lcDialog.FileName;
        //                structureM.LoadingJson(fileName);
        //                engineM.lastLoadType = "JSON";
        //            }
        //            showDialogLoadJSON = false;
        //        }
        //        if (showDialogSave == true)
        //        {
        //            if (sDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //            {
        //                fileName = sDialog.FileName;
        //                structureM.UnloadingJson(fileName);
        //            }
        //            showDialogSave = false;
        //        }
        //    }
        //}

        //void Update()
        //{
        //    if (showDialogLoadMGPL == true)
        //    {
        //        if (lDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            changeM.ResetChange();
        //            fileName = lDialog.FileName;
        //            string content = File.ReadAllText(fileName);
        //            readerM.ReadCode(content);
        //            logicM.LogicAdd();
        //            predicateM.BuildGraphs();
        //            engineM.lastLoadType = "MGPL";
        //        }
        //        showDialogLoadMGPL = false;
        //    }
        //    if (showDialogLoadJSON == true)
        //    {
        //        if (lcDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            changeM.ResetChange();
        //            fileName = lcDialog.FileName;
        //            structureM.LoadingJson(fileName);
        //            engineM.lastLoadType = "JSON";
        //        }
        //        showDialogLoadJSON = false;
        //    }
        //    if (showDialogSave == true)
        //    {
        //        if (sDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            fileName = sDialog.FileName;
        //            structureM.UnloadingJson(fileName);
        //        }
        //        showDialogSave = false;
        //    }
        //}
    }
}
