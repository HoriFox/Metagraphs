using UnityEngine;

namespace nm
{
    public class Engine : MonoBehaviour
    {
        public bool CustomCursor;
        public Texture2D cursorTexture;
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
            InitObject.Instance.Create();
        }
    }
    //public static T GetSafeComponent<T>(this GameObject obj) where T : MonoBehaviour
    //{
    //    T component = obj.GetComponent<T>();

    //    if (component == null)
    //    {
    //        Debug.LogError("Expected to find component of type "
    //           + typeof(T) + " but found none", obj);
    //    }

    //    return component;
    //}
}