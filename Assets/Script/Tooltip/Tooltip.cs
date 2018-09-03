using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

        FreeCamera fc;
        EditorMenu em;

        void Awake()
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
            fc = GameObject.Find("Camera").GetComponent<FreeCamera>();
            em = GameObject.Find("Menu").GetComponent<EditorMenu>();
            //boxText.alignment = TextAnchor.MiddleCenter;
        }

        void LateUpdate()
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
                    //if (tooltiptext.active)
                    show = true;
                }
            }

            boxText.text = text;
            //box.SetActive(active);
            arrow.SetActive(arrayShow);
            float width = maxWidth;
            // Если ширина текста соотвествует максимальной ширине.
            if (boxText.preferredWidth <= maxWidth - borderAround) width = boxText.preferredWidth + borderAround;
            // Если финальная ширина меньше минимальной ширины.
            if (width < minWidth) width = minWidth;
            boxRT.sizeDelta = new Vector2(width, boxText.preferredHeight + borderAround);

            float arrowShift = width / 4; // сдвиг позиции стрелки по Х

            if ((show || isUI) && !fc.m_inputCaptured && !em.menuActive)
            {
                float arrowPositionY = -(arrowRT.sizeDelta.y / 2 - 1); // позиция стрелки по умолчанию (внизу)
                float arrowPositionX = arrowShift;

                float curY = Input.mousePosition.y + boxRT.sizeDelta.y / 2 + arrowRT.sizeDelta.y;
                Vector3 arrowScale = new Vector3(1, 1, 1);
                if (curY + boxRT.sizeDelta.y / 2 > Screen.height) // если Tooltip выходит за рамки экрана, в данном случаи по высоте
                {
                    curY = Input.mousePosition.y - boxRT.sizeDelta.y / 2 - arrowRT.sizeDelta.y;
                    arrowPositionY = boxRT.sizeDelta.y + (arrowRT.sizeDelta.y / 2 - 1);
                    arrowScale = new Vector3(1, -1, 1); // отражение по вертикале
                }

                float curX = Input.mousePosition.x + arrowShift;
                if (curX + boxRT.sizeDelta.x / 2 > Screen.width)
                {
                    curX = Input.mousePosition.x - arrowShift;
                    arrowPositionX = width - arrowShift;
                }

                boxRT.anchoredPosition = new Vector2(curX, curY);

                arrowRT.anchoredPosition = new Vector2(arrowPositionX, arrowPositionY);
                arrowRT.localScale = arrowScale;

                foreach (Image bg in img)
                {
                    bg.color = Color.Lerp(bg.color, BGColor, speed * Time.deltaTime);
                }
                boxText.color = Color.Lerp(boxText.color, textColor, speed * Time.deltaTime);
            }
            else
            {
                foreach (Image bg in img)
                {
                    bg.color = Color.Lerp(bg.color, BGColorFade, speed * Time.deltaTime);
                }
                boxText.color = Color.Lerp(boxText.color, textColorFade, speed * Time.deltaTime);
            }
        }
    }
}