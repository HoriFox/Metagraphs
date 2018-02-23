using UnityEngine;
using System.Collections;
using System.IO;

public class TakeScreenShot : MonoBehaviour
{
    Texture2D screenCap;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.N)) StartCoroutine("Capture");
    }

    IEnumerator Capture()
    {
        screenCap = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        yield return new WaitForEndOfFrame();
        screenCap.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenCap.Apply();

        byte[] bytes = screenCap.EncodeToPNG();
        string timeAndData = System.DateTime.Now.ToString("hh-mm-ss MM-dd-yyyy");
        File.WriteAllBytes(Application.dataPath + "/ScreenShot/" + timeAndData + ".png", bytes);
    }
}
