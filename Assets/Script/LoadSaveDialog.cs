using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;


using UnityEditor;

namespace nm
{
    public class LoadSaveDialog : MonoBehaviour {

        public static LoadSaveDialog Instance;

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

        public string fileName = "";

        //System.Windows.Forms.OpenFileDialog lDialog;
        //System.Windows.Forms.OpenFileDialog lcDialog;
        //System.Windows.Forms.SaveFileDialog sDialog;
        //Engine engineM;

        private StructureModule structureM;

        // Awake()
        //{
        //engineM = Engine.GetInit();

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
        //}

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
            Instance = this;
            //readerM = Reader.GetInit();
            //logicM = LogicModule.GetInit();
            //predicateM = PredicateModule.GetInit();
            changeM = ChangeModule.GetInit();

            structureM = StructureModule.GetInit();
        }

        public static LoadSaveDialog GetInstance()
        {
            return Instance;
        }

        public void Clear()
        {
            fileName = "";
            ClearTemp();
        }

        public void SaveFile()
        {
            if (fileName.Length != 0 && File.Exists(fileName))
            {
                structureM.UnloadingJson(fileName);
            }
            else
            {
                SaveFileAs();
            }
        }

        public void SaveFileAs()
        {
#if UNITY_EDITOR
            fileName = EditorUtility.SaveFilePanel("Save the JSON json file", "", "", "json");
#else
            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.AddExtension = true;
            SaveFile.OverwritePrompt = true;
            SaveFile.RestoreDirectory = true;
            SaveFile.InitialDirectory = @"C:\\";
            SaveFile.Title = "Save the JSON json file";
            SaveFile.Filter = "JSON Files (*.json) | *.json |All Files | *.* ";

            if (SaveFile.ShowDialog() == DialogResult.OK)
            {
                fileName = SaveFile.FileName;
            }
            else
            {
                fileName = "";
            }
#endif
            if (fileName.Length != 0)
            {
                structureM.UnloadingJson(fileName);

                string path = UnityEngine.Application.dataPath + "/MetagraphEditorTemp";
                string[] files = Directory.GetFiles(path);
                string toLocation = Path.GetDirectoryName(fileName);

                foreach (var file in files)
                {
                    File.Copy(file, toLocation + "/" + Path.GetFileName(file), true);
                }
            }
        }

        public void OpenFile ()
        {
#if UNITY_EDITOR
            fileName = EditorUtility.OpenFilePanel("Please select the JSON file", "", "json");
#else

            OpenFileDialog OpenFile = new OpenFileDialog();

            OpenFile.RestoreDirectory = true;
            OpenFile.InitialDirectory = @"C:\\";
            OpenFile.Title = "Please select the JSON file";
            OpenFile.Filter = "JSON Files (*.json) | *.json |All Files | *.* ";

            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                fileName = OpenFile.FileName;
            }
            else
            {
                fileName = "";
            }
#endif
            if (fileName.Length != 0)
            {
                ClearTemp();
                changeM.ResetChange();
                structureM.LoadingJson(fileName);
            }

        }

        public void OpenModel()
        {
            string fileNameModel = "";
#if UNITY_EDITOR
            fileNameModel = EditorUtility.OpenFilePanel("Please select the OBJ file", "", "obj");
#else

            OpenFileDialog OpenFile = new OpenFileDialog();

            OpenFile.RestoreDirectory = true;
            OpenFile.InitialDirectory = @"C:\\";
            OpenFile.Title = "Please select the OBJ file";
            OpenFile.Filter = "OBJ Files (*.obj) | *.obj |All Files | *.* ";

            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                fileNameModel = OpenFile.FileName;
            }
#endif

            if (fileNameModel.Length != 0)
            {
                string path = UnityEngine.Application.dataPath + "/MetagraphEditorTemp";
                string name = Path.GetFileName(fileNameModel);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                File.Copy(fileNameModel, path + "/" + name, true);
                changeM.LoadModel(path + "/" + name);
            }

        }

        public void ClearTemp()
        {
            string path = UnityEngine.Application.dataPath + "/MetagraphEditorTemp";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }

        void OnApplicationQuit()
        {
            string path = UnityEngine.Application.dataPath + "/MetagraphEditorTemp";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
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
