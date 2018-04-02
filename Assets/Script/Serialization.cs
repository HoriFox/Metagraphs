using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Object
{
    public string name;
    public Object()
    {
        this.name = "TestName";
    }
}

[System.Serializable]
public class Serialization : MonoBehaviour {

    public static Serialization current;
    public Object name1;
    public Object name2;
    public Object name3;

    public Serialization()
    {
        name1 = new Object();
        name2 = new Object();
        name3 = new Object();

        //Данные для сериализации
    }
}
