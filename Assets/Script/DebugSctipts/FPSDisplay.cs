using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private GUIStyle style = new GUIStyle();
    private Rect rect;

    public void Start()
    {
        int w = Screen.width;
        int h = Screen.height;
        int heightGui = h * 2 / 100;
        rect = new Rect(10f, h - heightGui - 10f, w, heightGui);

        style.alignment = TextAnchor.MiddleLeft;
        style.fontSize = 14;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} fps ({1:0.0} ms)", fps, msec);
        GUI.Label(rect, text, style);
    }
}
