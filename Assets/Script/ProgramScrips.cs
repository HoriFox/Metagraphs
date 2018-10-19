using System.Collections;
using UnityEngine;
using System.IO;

namespace nm
{
    public class ProgramScrips : MonoBehaviour
    {
        EditorMenu editorMenu;
        FreeCamera freeCamera;
        public GameObject GUIObject;

        private StructureModule structureM;

        private void Start()
        {
            structureM = StructureModule.GetInit();
        }

        void Awake()
        {
            editorMenu = GameObject.Find("Menu").GetComponent<EditorMenu>();
            freeCamera = Camera.main.GetComponent<FreeCamera>();
        }

        // Сохранение.
        public void Save()
        {
            GetComponent<LoadSaveDialog>().showDialogSave = true;
        }

        // Загрузка mgpl файла.
        public void LoadMGPL()
        {
            GetComponent<LoadSaveDialog>().showDialogLoadMGPL = true;
        }
        // Загрузка конфигурации.
        public void LoadConfiguration()
        {
            GetComponent<LoadSaveDialog>().showDialogLoadJSON = true;
        }
        // Очистка.
        public void Clear()
        {
            SceneCleaning.Instance.Clean();
            ChangeTransform.Instance.ResetChangeTransform();
            structureM.NewStructure();
        }
        // Назад.
        public void Backward()
        {
            Debug.Log("Назад");
            //StartCoroutine("Capture");
        }
        // Вперёд.
        public void Forward()
        {
            Debug.Log("Вперёд");
            //StartCoroutine("Capture");
        }
        // Сделать скриншот.
        public void MakeScreenShot()
        {
            Debug.Log("Скриншот");
            StartCoroutine("CaptureScreen");
        }

        // TO DO Не успевает обработать скриншот и оставляет GUI
        public int screenshotQuality = 1;

        public IEnumerator CaptureScreen()
        {
            yield return null;
            GUIObject.SetActive(false);
            yield return new WaitForEndOfFrame();
            string timeAndData = System.DateTime.Now.ToString("hh-mm-ss MM-dd-yyyy");
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshot/" + timeAndData + ".png"/*, screenshotQuality*/);
            GUIObject.SetActive(true);
        }

        // Показать настройки.
        public void ShowAbout()
        {
            if (!editorMenu.menuActive && !freeCamera.m_inputCaptured)
            {
                editorMenu.ShowAbout();
            }
        }
        // Показать настройки.
        public void ShowSetting()
        {
            if (!editorMenu.fail && !editorMenu.menuActive && !freeCamera.m_inputCaptured)
            {
                editorMenu.Show();
            }
        }
        // Выйти из приложения.
        public void Quit()
        {
            Application.Quit();
        }
    }
}
