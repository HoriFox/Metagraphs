using UnityEngine;

namespace nm
{
    public class Engine : MonoBehaviour
    {
        //public bool CustomCursor;
        //public Texture2D cursorTexture;
        public GameObject boxtooltip;
        //public string lastLoadType = "";

        private static Engine init;

        public int selectedAxis = -1;

        void Awake()
        {
            init = this;
            //if (CustomCursor)
            //{
                //Cursor.SetCursor(cursorTexture, new Vector2(14, 14), CursorMode.Auto);
            //}
            boxtooltip.SetActive(true);
        }

        public static Engine GetInit()
        {
            return init;
        }
    }
}