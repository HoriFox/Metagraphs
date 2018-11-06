using UnityEngine;

namespace nm
{
    public class Engine : MonoBehaviour
    {
        public bool CustomCursor;
        public Texture2D cursorTexture;
        public GameObject boxtooltip;

        private static Engine init;

        [HideInInspector] public Reader readerM;
        [HideInInspector] public LogicModule logicM;
        [HideInInspector] public PredicateModule predicateM;

        public int selectedAxis = -1;

        void Awake()
        {
            init = this;
            if (CustomCursor)
            {
                Cursor.SetCursor(cursorTexture, new Vector2(14, 14), CursorMode.Auto);
            }
            boxtooltip.SetActive(true);
        }

        void Start()
        {
            readerM = Reader.GetInit();
            logicM = LogicModule.GetInit();
            predicateM = PredicateModule.GetInit();

            //InitObject.Instance.Create();
        }

        public void ReadAndBuild(string content)
        {
            readerM.ReadCode(content);
            logicM.LogicAdd();
            predicateM.BuildGraphs();
        }

        public static Engine GetInit()
        {
            return init;
        }
    }
}