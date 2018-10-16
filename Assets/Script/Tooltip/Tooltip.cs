using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class Tooltip : MonoBehaviour
    {

        public static string text;
        //public static bool active;
        public static bool arrayShow;
        public static bool isUI;

        public Color BGColor = Color.white;
        public Color textColor = Color.black;
        // Размер шрифта.
        public int fontSize = 14;
        // Максимальная ширина Tooltip.
        public int maxWidth = 250;
        // Минимальная ширина Tooltip.
        public int minWidth = 70;
        // Ширина обводки.
        public int border = 10;
        // Скорость плавного затухания и проявления.
        public float speed = 10;
        public GameObject box;
        public GameObject arrow;
        public Text boxText;
        public Camera _camera;

        private int borderAround;
        private Image[] img;
        private Color BGColorFade;
        private Color textColorFade;
        private RectTransform arrowRT;
        private RectTransform boxRT;

        private float arrowPositionY;
        private float arrowPositionX;
        private float currentZ;
        private float curY;
        private float curX;
        private bool right;

        FreeCamera freeCamera;
        EditorMenu editorMenu;

        private void Awake()
        {
            borderAround = border * 2;
            img = new Image[2];
            img[0] = box.GetComponent<Image>();
            boxRT = box.GetComponent<RectTransform>();
            arrowRT = arrow.GetComponent<RectTransform>();
            img[1] = arrowRT.GetComponent<Image>();
            boxRT.sizeDelta = new Vector2(maxWidth, boxRT.sizeDelta.y);
            BGColorFade = BGColor;
            BGColorFade.a = 0;
            textColorFade = textColor;
            textColorFade.a = 0;
            isUI = false;
            foreach (Image bg in img)
            {
                bg.color = BGColorFade;
            }
            boxText.color = textColorFade;
            freeCamera = Camera.main.GetComponent<FreeCamera>();
            editorMenu = GameObject.Find("Menu").GetComponent<EditorMenu>();
        }

        private void LateUpdate()
        {
            bool show = false;
            boxText.fontSize = fontSize;

            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.GetComponent<TooltipText>())
                {
                    TooltipText tooltiptext = hit.transform.GetComponent<TooltipText>();
                    text = tooltiptext.text;
                    arrayShow = tooltiptext.arrayShow;
                    show = true;
                }
            }

            boxText.text = text;
            arrow.SetActive(arrayShow);
            float width = maxWidth;

            if (boxText.preferredWidth <= maxWidth - borderAround)
            {
                width = boxText.preferredWidth + borderAround;
            }

            // Ограничиватель. 
            width = Mathf.Clamp(width, minWidth, maxWidth);

            boxRT.sizeDelta = new Vector2(width, boxText.preferredHeight + borderAround);

            if ((show || isUI) && !freeCamera.m_inputCaptured && !editorMenu.menuActive)
            {
                arrowPositionY = 0;
                arrowPositionX = 0;
                currentZ = 0;
                right = false;
                curY = Input.mousePosition.y + boxRT.sizeDelta.y + arrowRT.sizeDelta.y / 4f;
                curX = Input.mousePosition.x + boxRT.sizeDelta.x + arrowRT.sizeDelta.x / 4f;

                // Если Tooltip выходит за рамки экрана по ширине.
                if (curX > Screen.width)
                {
                    right = true;
                    arrowPositionX = boxRT.sizeDelta.x;
                }
                // Если Tooltip выходит за рамки экрана по высоте.
                if (curY > Screen.height)
                {
                    arrowPositionY = boxRT.sizeDelta.y;
                    if (right)
                    {
                        // Верхний правый
                        currentZ = 315f;
                        curX -= (boxRT.sizeDelta.x + boxRT.sizeDelta.x / 2f + arrowRT.sizeDelta.x / 2f);
                        curY -= (boxRT.sizeDelta.y + arrowRT.sizeDelta.y + arrowRT.sizeDelta.y / 2f);
                    }
                    else
                    {
                        // Верхний левый
                        currentZ = 45f;
                        curX -= boxRT.sizeDelta.x / 2f;
                        curY -= (boxRT.sizeDelta.y + boxRT.sizeDelta.y / 2f + arrowRT.sizeDelta.y / 2f);
                    }
                }
                else
                {
                    if (right)
                    {
                        // Нижний правый
                        currentZ = 225f;
                        curX -= (boxRT.sizeDelta.x + boxRT.sizeDelta.x / 2f + arrowRT.sizeDelta.x / 2f);
                        curY -= boxRT.sizeDelta.y / 2f;

                    }
                    else
                    {
                        // Нижний левый
                        currentZ = 135f;
                        curX -= boxRT.sizeDelta.x / 2f;
                        curY -= boxRT.sizeDelta.y / 2f;
                    }
                }

                boxRT.anchoredPosition = new Vector2(curX , curY);
                arrowRT.anchoredPosition = new Vector2(arrowPositionX, arrowPositionY);
                arrowRT.localRotation = Quaternion.Euler(0f, 0f, currentZ);

                // Появиться.
                foreach (Image bg in img)
                {
                    bg.color = Color.Lerp(bg.color, BGColor, speed * Time.deltaTime);
                }
                boxText.color = Color.Lerp(boxText.color, textColor, speed * Time.deltaTime);
            }
            else
            {
                // Исчезнуть.
                foreach (Image bg in img)
                {
                    bg.color = Color.Lerp(bg.color, BGColorFade, speed * Time.deltaTime);
                }
                boxText.color = Color.Lerp(boxText.color, textColorFade, speed * Time.deltaTime);
            }
        }
    }
}