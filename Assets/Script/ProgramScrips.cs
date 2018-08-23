using System.Collections;
using UnityEngine;
using System.IO;

namespace nm
{
    public class ProgramScrips : MonoBehaviour
    {
        EditorMenu em;
        FreeCamera fc;

        void Awake()
        {
            em = GameObject.Find("Menu").GetComponent<EditorMenu>();
            fc = GameObject.Find("Camera").GetComponent<FreeCamera>();
        }

        // Сохранение.
        public void Save()
        {
            GetComponent<LoadSaveDialog>().showDialogSave = true;
        }

        // Загрузка.
        public void Load()
        {
            GetComponent<LoadSaveDialog>().showDialogLoad = true;
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
        public void ShowInfo()
        {
            em.menuActive = true;
        }
        // Показать настройки.
        public void ShowSetting()
        {
            if (!em.fail && !em.menuActive && !fc.m_inputCaptured)
            {
                em.Show();
            }
        }
        // Выйти из приложения.
        public void Quit()
        {
            Application.Quit();
        }
    }
}
