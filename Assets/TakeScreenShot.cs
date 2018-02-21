using UnityEngine;
using System.Collections;
using System.IO;

public class TakeScreenShot : MonoBehaviour
{
    Texture2D screenCap;
    //Texture2D border; //Для рамки
    bool shot = false;

    void Start()
    {
        screenCap = new Texture2D(300, 200, TextureFormat.RGB24, false); //Шаблон, на который будет наноситься скриншот | сделать размеры монитора
        //border = new Texture2D(2, 2, TextureFormat.ARGB32, false); //Для рамки
        //border.Apply(); //Для рамки
    }

    void Update()
    {
        //Если размеры изменились, то перегрузить шаблон заново !

        if (Input.GetKeyUp(KeyCode.N)) // Активация скриншота
        {
            StartCoroutine("Capture");
        }
    }

    void OnGUI()
    {
        //GUI.DrawTexture(new Rect(200, 100, 300, 2), border, ScaleMode.StretchToFill); //Полоска 1 //Рамка
        //GUI.DrawTexture(new Rect(200, 300, 300, 2), border, ScaleMode.StretchToFill); //Полоска 2
        //GUI.DrawTexture(new Rect(200, 100, 2, 200), border, ScaleMode.StretchToFill); //Полоска 3
        //GUI.DrawTexture(new Rect(500, 100, 2, 200), border, ScaleMode.StretchToFill); //Полоска 4

        if (shot)
        {
            GUI.DrawTexture(new Rect(10, 10, 60, 40), screenCap, ScaleMode.StretchToFill);
        }
    }

    IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();
        screenCap.ReadPixels(new Rect(198, 98, 80, 198), 0, 0); // Сделать размеры приложения!
        screenCap.Apply();

        byte[] bytes = screenCap.EncodeToPNG();
        string timeAndData = System.DateTime.Now.ToString("hh-mm-ss MM-dd-yyyy");
        File.WriteAllBytes(Application.dataPath + "/ScreenShot/" + timeAndData + ".png", bytes);

        shot = true;
    }
}
