using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Engine : MonoBehaviour
{
    public SetObject setObject;

    void Start()
    {
        setObject.Create();
    }
}