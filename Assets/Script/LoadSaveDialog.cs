using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using System;


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
        //private LogicModule logicM;
        //private PredicateModule predicateM;
        private ChangeModule changeM;

        string fileName = "";

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
            //if (OpenFilesBtn != null)
            //    OpenFilesBtn.interactable = FileBrowser.canOpenMultipleFiles;

            //if (OpenFoldersBtn != null)
            //    OpenFoldersBtn.interactable = FileBrowser.canOpenMultipleFolders;

            //readerM = Reader.GetInit();
            //logicM = LogicModule.GetInit();
            //predicateM = PredicateModule.GetInit();
            changeM = ChangeModule.GetInit();

            structureM = StructureModule.GetInit();
        }

        public void Clear()
        {
            fileName = "";
        }

        public void SaveFile()
        {
            if (fileName.Length != 0)
            {
                structureM.UnloadingJson(fileName);
            }
            else
            {
                SaveFileAs();
            }
        }

        // КОСССССТЫЫЫЫЫЫЫЛЬЬЬЬ
        //[DllImport("user32.dll")]
        //public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        //[DllImport("user32.dll")]
        //public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        //public const int WM_SYSCOMMAND = 0x0112;
        //public const int SC_CLOSE = 0xF060;
        //public static void FindAndCloseWindow()
        //{
        //    IntPtr lHwnd = FindWindow(null, "Oops");
        //    if (lHwnd != IntPtr.Zero)
        //    {
        //        SendMessage(lHwnd, WM_SYSCOMMAND, SC_CLOSE, 0);
        //    }
        //}
        // КОСССССТЫЫЫЫЫЫЫЛЬЬЬЬ

        public void SaveFileAs()
        {
            //SaveFileDialog SaveFile = new SaveFileDialog();

            //SaveFile.AddExtension = true;
            //SaveFile.OverwritePrompt = true;
            //SaveFile.RestoreDirectory = true;
            //SaveFile.InitialDirectory = @"C:\\";
            //SaveFile.Title = "Save the Metagraph json file";
            //SaveFile.Filter = "JSON Files (*.json) | *.json |All Files | *.* ";

            //if (SaveFile.ShowDialog() == DialogResult.OK)
            //{
            //    fileName = SaveFile.FileName;
            //    if (fileName.Length != 0)
            //    {
            //        structureM.UnloadingJson(fileName);
            //    }
            //}

            //#if UNITY_EDITOR
            //fileName = EditorUtility.SaveFilePanel("Save the Metagraph json file", "", "", "json");
            //if (fileName.Length != 0)
            //{
            //    structureM.UnloadingJson(fileName);
            //}
            //#endif
        }

        public void OpenFile ()
        {
            //OpenFileDialog OpenFile = new OpenFileDialog();

            //OpenFile.RestoreDirectory = true;
            //OpenFile.InitialDirectory = @"C:\\";
            //OpenFile.Title = "Please select the Metafile file";
            //OpenFile.Filter = "JSON Files (*.json) | *.json |All Files | *.* ";

            //if (OpenFile.ShowDialog() == DialogResult.OK)
            //{
            //    fileName = OpenFile.FileName;

            //    if (fileName.Length != 0)
            //    {
            //        changeM.ResetChange();
            //        structureM.LoadingJson(fileName);
            //    }
            //}

            //#if UNITY_EDITOR
            //            fileName = EditorUtility.OpenFilePanel("Please select the Metafile file", "", "json");
            //            if (fileName.Length != 0)
            //            {
            //                changeM.ResetChange();
            //                structureM.LoadingJson(fileName);
            //            }
            //#endif
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
