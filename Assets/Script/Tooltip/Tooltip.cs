using UnityEngine;
using UnityEngine.UI;

namespace nm
{
    public class Tooltip : MonoBehaviour
    {

        public static string text;
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
        public RectTransform box;
        public GameObject arrow;
        public Text boxText;
        public Camera _camera;


        private Image[] img;
        private Color BGColorFade;
        private Color textColorFade;
        private RectTransform arrowRT;

        FreeCamera fc;
        EditorMenu em;

        void Awake()
        {
            arrowRT = arrow.GetComponent<RectTransform>();
            img = new Image[2];
            img[0] = box.GetComponent<Image>();
            img[1] = arrowRT.GetComponent<Image>();
            box.sizeDelta = new Vector2(maxWidth, box.sizeDelta.y);
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
            boxText.alignment = TextAnchor.MiddleCenter;
            fc = GameObject.Find("Camera").GetComponent<FreeCamera>();
            em = GameObject.Find("Menu").GetComponent<EditorMenu>();
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
                    show = true;
                }
            }

            boxText.text = text;
            arrow.SetActive(arrayShow);
            float width = maxWidth;
            // Если ширина текста соотвествует максимальной ширине.
            if (boxText.preferredWidth <= maxWidth - border) width = boxText.preferredWidth + border;
            // Если финальная ширина меньше минимальной ширины.
            if (width < minWidth) width = minWidth;
            box.sizeDelta = new Vector2(width, boxText.preferredHeight + border);
            // Cдвиг позиции стрелки по Х.
            float arrowShift = width / 4;

            if ((show || isUI) && !fc.m_inputCaptured && !em.menuActive)
            {
                // Gозиция стрелки по умолчанию (внизу).
                float arrowPositionY = -(arrowRT.sizeDelta.y / 2 - 1); 
                float arrowPositionX = arrowShift;

                float curY = Input.mousePosition.y + box.sizeDelta.y / 2 + arrowRT.sizeDelta.y;
                Vector3 arrowScale = new Vector3(1, 1, 1);
                // Если Tooltip выходит за рамки экрана, в данном случаи по высоте.
                if (curY + box.sizeDelta.y / 2 > Screen.height) 
                {
                    curY = Input.mousePosition.y - box.sizeDelta.y / 2 - arrowRT.sizeDelta.y;
                    arrowPositionY = box.sizeDelta.y + (arrowRT.sizeDelta.y / 2 - 1);
                    // Отражение по вертикале.
                    arrowScale = new Vector3(1, -1, 1); 
                }

                float curX = Input.mousePosition.x + arrowShift;
                if (curX + box.sizeDelta.x / 2 > Screen.width)
                {
                    curX = Input.mousePosition.x - arrowShift;
                    arrowPositionX = width - arrowShift;
                }

                box.anchoredPosition = new Vector2(curX, curY);

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