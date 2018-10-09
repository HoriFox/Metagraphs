using System.Collections;
using UnityEngine;
using System.IO;

namespace nm
{
    public class ProgramScrips : MonoBehaviour
    {
        EditorMenu editorMenu;
        FreeCamera freeCamera;

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
            StartCoroutine("Capture");
        }
        Texture2D screenCap;
        // Куратина скриншота.
        IEnumerator Capture()
        {
            screenCap = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            yield return new WaitForEndOfFrame();
            screenCap.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenCap.Apply();

            byte[] bytes = screenCap.EncodeToPNG();
            string timeAndData = System.DateTime.Now.ToString("hh-mm-ss MM-dd-yyyy");
            File.WriteAllBytes(Application.dataPath + "/Screenshot/" + timeAndData + ".png", bytes);
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
