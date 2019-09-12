using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
    public GameObject panel;
    public void Switch()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
