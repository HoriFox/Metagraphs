using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCameraView : MonoBehaviour
{
    public Camera cameraMain;
    public Image imageButton;

    public Sprite orthographic;
    public Sprite perspective;

    public void UpdateView()
    {
        if (cameraMain.orthographic)
        {
            imageButton.sprite = perspective;
            cameraMain.orthographic = false;
        }
        else
        {
            imageButton.sprite = orthographic;
            cameraMain.orthographic = true;
        }
    }
}
