using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Engine : MonoBehaviour
{
    [SerializeField]
    private SetObject setObject;

    void Start()
    {
        setObject.Create();
    }
}