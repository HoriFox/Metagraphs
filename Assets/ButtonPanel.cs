using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPanel : MonoBehaviour
{
    public GameObject[] panels;

    public void HideAllBesides(int besidesNumber)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i != besidesNumber)
            {
                panels[i].SetActive(false);
            }
        }
    }

    public void HideAll()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
    }
}
