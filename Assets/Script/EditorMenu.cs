using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;
namespace nm
{
    public class EditorMenu : MonoBehaviour
    {
        [HideInInspector]
        public bool menuActive;
        [HideInInspector]
        public bool fail;
        [HideInInspector]
        public float mouseSensitivity;
        [HideInInspector]
        public float smoothingMotion;
        public static KeyCode[] keys;

        public string saveFileName = "EditorSettings";
        public GameObject[] allUI;
        public GameObject startMenu;
        public GameObject aboutMenu;
        public GameObject errorMenu;
        public Transform hidePanel;
        public float defaultMouseSensitivity = 0.5f;
        public float defaultSmoothingMotion = 0.36f;
        public KeyCode[] defaultKeys = { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S, KeyCode.E, KeyCode.Q, KeyCode.LeftShift };
        public Scrollbar SensitivityScrollbar;
        public Scrollbar SmoothingScrollbar;
        public Text[] keyText;
        public GameObject keyMenu;
        public GameObject keyInfo;
        public Button applyButton;
        public Toggle screenMode;
        public Text resolutionsText;
        public Text qualityText;

        private Resolution[] resolutionsList;
        private string[] qualityList;
        private int curButtonId;
        private bool updateKey;
        private bool isFullScreen;
        private int resolutions_id;
        private int quality_id;

        void Start()
        {
            // Определяем разрешение и качество.
            // Будут доступны все разрешения, которые поддерживает монитор (в полноэкранном режиме).
            qualityList = QualitySettings.names;
            resolutionsList = Screen.resolutions;
            quality_id = QualitySettings.GetQualityLevel();
            for (int i = 0; i < resolutionsList.Length; i++)
            {
                if (resolutionsList[i].height == Screen.currentResolution.height && resolutionsList[i].width == Screen.currentResolution.width) resolutions_id = i;
            }

            // Стартовые значения.
            fail = false;
            menuActive = false;
            updateKey = false;
            mouseSensitivity = defaultMouseSensitivity;
            smoothingMotion = defaultSmoothingMotion;
            if (!Screen.fullScreen) isFullScreen = false; else isFullScreen = true;
            keys = new KeyCode[defaultKeys.Length];
            for (int i = 0; i < defaultKeys.Length; i++)
            {
                keys[i] = defaultKeys[i];
            }
            foreach (GameObject obj in allUI)
            {
                obj.SetActive(false);
            }

            // Проверка наличия файла сохранения.
            // Файл создается в папке ИМЯ_Data.
            // В редакторе файл будет создан в папке Assets.
            if (File.Exists(Application.dataPath + "/" + saveFileName + ".xml") && !PlayerPrefs.HasKey("Error"))
            {
                LoadSettings();
            }
            else
            {
                SaveDefaultSettings();
            }
        }

        void LoadSettings()
        {
            // Пробуем читать файл с сохранениями.
            try
            {
                XmlTextReader keyReader = new XmlTextReader(Application.dataPath + "/" + saveFileName + ".xml");
                for (int i = 0; i < keys.Length; i++)
                {
                    // Пробуем читать текст как int.
                    keyReader.ReadToFollowing("KeyCode_" + i);
                    int k;
                    if (int.TryParse(keyReader.ReadString(), out k)) keys[i] = (KeyCode)k; else fail = true;
                }
                keyReader.Close();

                XmlTextReader reader = new XmlTextReader(Application.dataPath + "/" + saveFileName + ".xml");
                while (reader.Read())
                {
                    if (reader.IsStartElement("SmoothingMotionValue"))
                    {
                        float k;
                        if (float.TryParse(reader.ReadString(), out k)) smoothingMotion = k; else fail = true;
                    }
                    if (reader.IsStartElement("MouseSensitivityValue"))
                    {
                        float k;
                        if (float.TryParse(reader.ReadString(), out k)) mouseSensitivity = k; else fail = true;
                    }
                }
                reader.Close();

                // Ловим ошибки чтения текста.
                if (fail)
                {
                    PlayerPrefs.SetInt("Error", 1);
                    errorMenu.SetActive(true);
                }
                else
                {
                    ReadyToStart();
                }
            }

            // Ловим ошибки чтения файла.
            catch (System.Exception)
            {
                PlayerPrefs.SetInt("Error", 1);
                errorMenu.SetActive(true);
            }
        }

        // Сохраняем значения по умолчанию.
        public void SaveDefaultSettings()
        {
            if (PlayerPrefs.HasKey("Error")) PlayerPrefs.DeleteKey("Error");

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("SaveData");
            xmlDoc.AppendChild(rootNode);

            XmlNode userNode;

            userNode = xmlDoc.CreateElement("SmoothingMotionValue");
            userNode.InnerText = "0.36";
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("MouseSensitivityValue");
            userNode.InnerText = "0.5";
            rootNode.AppendChild(userNode);

            for (int i = 0; i < defaultKeys.Length; i++)
            {
                int k = (int)defaultKeys[i];
                userNode = xmlDoc.CreateElement("KeyCode_" + i);
                userNode.InnerText = k.ToString();
                rootNode.AppendChild(userNode);
            }

            xmlDoc.Save(Application.dataPath + "/" + saveFileName + ".xml");

            ReadyToStart();
        }

        // Автосохранение, перед выходом.
        void OnApplicationQuit()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("SaveData");
            xmlDoc.AppendChild(rootNode);

            XmlNode userNode;

            userNode = xmlDoc.CreateElement("SmoothingMotionValue");
            userNode.InnerText = smoothingMotion.ToString();
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("MouseSensitivityValue");
            userNode.InnerText = mouseSensitivity.ToString();
            rootNode.AppendChild(userNode);

            for (int i = 0; i < keys.Length; i++)
            {
                int k = (int)keys[i];
                userNode = xmlDoc.CreateElement("KeyCode_" + i);
                userNode.InnerText = k.ToString();
                rootNode.AppendChild(userNode);
            }

            xmlDoc.Save(Application.dataPath + "/" + saveFileName + ".xml");
        }

        // После определения переменных, применяем конечный результат.
        void ReadyToStart()
        {
            qualityText.text = qualityList[quality_id];
            resolutionsText.text = resolutionsList[resolutions_id].width + "x" + resolutionsList[resolutions_id].height;
            screenMode.isOn = isFullScreen;
            SensitivityScrollbar.value = mouseSensitivity;
            SmoothingScrollbar.value = smoothingMotion;
            applyButton.interactable = false;
            for (int i = 0; i < keys.Length; i++)
            {
                keyText[i].text = keys[i].ToString();
            }
        }

        public void SetApplyButton(bool i) // !
        {
            applyButton.interactable = i;
        }

        public void AppQuit()
        {
            Application.Quit();
        }

        public void SetScreenMode(bool mode)
        {
            isFullScreen = mode;
        }

        public void SensitivityScrollBar(float value)
        {
            mouseSensitivity = value;
        }

        public void SmoothingScrollBar(float value)
        {
            smoothingMotion = value;
        }

        public void DefaultKeyBoardSitting()
        {
            for (int i = 0; i < defaultKeys.Length; i++)
            {
                keys[i] = defaultKeys[i];
                keyText[i].text = keys[i].ToString();
            }
        }

        public void DefaultMouseSitting()
        {
            SensitivityScrollbar.value = defaultMouseSensitivity;
            SmoothingScrollbar.value = defaultSmoothingMotion;
        }

        // Определение id клавиши, которая будет изменена.
        public void KeyButtonID(int id)
        {
            curButtonId = id;
            updateKey = true;
        }

        // Ловим нажатие клавиши.
        void OnGUI()
        {
            if (updateKey)
            {
                Event e = Event.current;
                if (e.isKey)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        updateKey = false;
                    }
                    else
                    {
                        keyInfo.SetActive(false);
                        keyMenu.SetActive(true);
                        updateKey = false;
                        keys[curButtonId] = e.keyCode;
                        for (int i = 0; i < keys.Length; i++)
                        {
                            keyText[i].text = keys[i].ToString();
                        }
                    }
                }
            }
        }

        public void NextResolutionsID()
        {
            if (resolutions_id < resolutionsList.Length - 1) resolutions_id++; else resolutions_id = 0;
            resolutionsText.text = resolutionsList[resolutions_id].width + "x" + resolutionsList[resolutions_id].height;
        }

        public void NextQualityID()
        {
            if (quality_id < qualityList.Length - 1) quality_id++; else quality_id = 0;
            qualityText.text = qualityList[quality_id];
        }

        // Применение видео/аудио и сохранение.
        public void ApplySitting()
        {
            QualitySettings.SetQualityLevel(quality_id, true);
            Screen.SetResolution(resolutionsList[resolutions_id].width, resolutionsList[resolutions_id].height, isFullScreen);
            OnApplicationQuit();
        }

        // Вкл/Выкл меню клавишей Escape.
        void Update()
        {
            if (!fail)
            {
                if (Input.GetKeyDown(KeyCode.Escape) && menuActive) Hide();
            }
        }

        // Сброс активности меню.
        public void ResetMenuActive()
        {
            menuActive = false;
            Camera.main.GetComponent<FreeCamera>().UpdateMouseSetting();
            hidePanel.gameObject.SetActive(false);
        }

        public void ShowAbout()
        {
            hidePanel.gameObject.SetActive(true);
            aboutMenu.SetActive(true);
            menuActive = true;
        }

        public void Show()
        {
            hidePanel.gameObject.SetActive(true);
            startMenu.SetActive(true);
            menuActive = true;
        }

        public void Hide()
        {
            ResetMenuActive();
            foreach (GameObject obj in allUI)
            {
                obj.SetActive(false);
            }
        }
    }
}
