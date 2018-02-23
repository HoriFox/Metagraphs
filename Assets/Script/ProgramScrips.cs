using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

public class ProgramScrips : MonoBehaviour
{
    Texture2D screenCap;

    public static List<Serialization> savedGames = new List<Serialization>();

    public static void Save() // Экспорт (сохранение)
    {
        savedGames.Add(Serialization.current);
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath это строка; выведите ее в логах и вы увидите расположение файла сохранений
        FileStream file = File.Create(Application.persistentDataPath + "/saved.gd");
        bf.Serialize(file, savedGames);
        file.Close();
    }

    public static void Load() // Импорт (загрузка)
    {
        if (File.Exists(Application.persistentDataPath + "/saved.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/saved.gd", FileMode.Open);
            savedGames = (List<Serialization>)bf.Deserialize(file);
            file.Close();
            //savedGames мучаем данную переменную и достаём данные
        }
    }

    public void MakeScreenShot() // Сделать скриншот
    {
        StartCoroutine("Capture");
    }

    public void Quit() // Выйти из приложения
    {
        Application.Quit();
    }

    IEnumerator Capture()
    {
        screenCap = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        yield return new WaitForEndOfFrame();
        screenCap.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenCap.Apply();

        byte[] bytes = screenCap.EncodeToPNG();
        string timeAndData = System.DateTime.Now.ToString("hh-mm-ss MM-dd-yyyy");
        File.WriteAllBytes(Application.dataPath + "/Screenshot/" + timeAndData + ".png", bytes);
    }
}
