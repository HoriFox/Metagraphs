using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

namespace nm
{
    public class LoadSaveDialog : MonoBehaviour {

        [DllImport("user32.dll")]
        private static extern void OpenFileDialog();
        [HideInInspector]
        public bool showDialogLoad = false;
        [HideInInspector]
        public bool showDialogSave = false;

        string fileName;

        System.Windows.Forms.OpenFileDialog lDialog;
        System.Windows.Forms.SaveFileDialog sDialog;
        Reader re;

        void Awake()
        {
            re = GetComponent<Reader>();

            lDialog = new System.Windows.Forms.OpenFileDialog();
            sDialog = new System.Windows.Forms.SaveFileDialog();

            lDialog.RestoreDirectory = true;
            lDialog.InitialDirectory = @"C:\\";
            lDialog.Title = "Please select the Metafile file. ";
            lDialog.Filter = "MGPL Files (*.MGPL) | *.mgpl |All Files | *.* ";

            sDialog.AddExtension = true;
            sDialog.OverwritePrompt = true;
            sDialog.RestoreDirectory = true;
            sDialog.InitialDirectory = @"C:\\";
            sDialog.Title = "Save the Metafile file. ";
            sDialog.Filter = "MGPL Files (*.MGPL) | *.mgpl |All Files | *.* ";
        }

        void Update()
        {
            if (showDialogLoad == true)
            {

                if (lDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = lDialog.FileName;
                    string content = File.ReadAllText(fileName);
                    re.ReadCode(content);
                }
                showDialogLoad = false;
            }
            if (showDialogSave == true)
            {

                if (sDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = sDialog.FileName;
                    System.IO.File.WriteAllText(fileName, "TestCodeText");
                }
                showDialogSave = false;
            }
        }
    }
}
