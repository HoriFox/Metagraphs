using UnityEngine;

namespace nm
{
    public class Engine : MonoBehaviour
    {
        public bool CustomCursor;
        public Texture2D cursorTexture;
        public SetObject setObject;
        public GameObject boxtooltip;

        void Awake()
        {
            if (CustomCursor)
            {
                Cursor.SetCursor(cursorTexture, new Vector2(14, 14), CursorMode.Auto);
            }
            boxtooltip.SetActive(true);
        }

        void Start()
        {
            setObject.Create();
        }
    }
}