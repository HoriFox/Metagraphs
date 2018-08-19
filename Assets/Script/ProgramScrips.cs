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

    // Экспорт (сохранение).
    public void Save()
    {
        Debug.Log("Сохранение");
        savedGames.Add(Serialization.current);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saved.gd");
        bf.Serialize(file, savedGames);
        file.Close();
    }

    //C:\Users\Администратор\Documents\Metagraphs

    // Импорт (загрузка).
    public void Load() 
    {
        Debug.Log("Загрузка");
        if (File.Exists(Application.persistentDataPath + "/saved.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/saved.gd", FileMode.Open);
            savedGames = (List<Serialization>)bf.Deserialize(file);
            file.Close();
            //savedGames // Мучаем данную переменную и достаём данные
        }
    }
    // Назад.
    public void Backward()
    {
        Debug.Log("Назад");
        //StartCoroutine("Capture");
    }
    // Вперёд.
    public void Forward()
    {
        Debug.Log("Вперёд");
        //StartCoroutine("Capture");
    }
    // Сделать скриншот.
    public void MakeScreenShot()
    {
        Debug.Log("Скриншот");
        StartCoroutine("Capture");
    }
    // Куратина скриншота.
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
    // Показать информацию.
    public void ShowInfo()
    {
        Debug.Log("Информация");
        //StartCoroutine("Capture");
    }
    // Показать настройки.
    public void ShowSetting()
    {
        Debug.Log("Настройки");
        //StartCoroutine("Capture");
    }
    // Выйти из приложения.
    public void Quit()
    {
        Application.Quit();
    }
}
