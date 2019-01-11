using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManagment : MonoBehaviour
{
    public static void Serialize<T>(T objectToSerialize, string path)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, objectToSerialize);
        stream.Close();
    }

    public static T Deserialize<T>(string path)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        T deserializedObject = (T)(object)formatter.Deserialize(stream);
        stream.Close();
        return deserializedObject;
    }

    public void OnApplicationQuit()
    {
        SaveMap();
        SaveResources();
    }
    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            SaveMap();
            SaveResources();
        }
    }

    private void SaveMap()
    {
        string mapPath = Path.Combine(Application.persistentDataPath, "Map.bin");
        Serialize<List<Building>>(GameObject.Find("Map").GetComponent<MapManager>().map.Buildings, mapPath);
    }
    private void SaveResources()
    {
        string ResourcesPath = Path.Combine(Application.persistentDataPath, "Resources.bin");
        Serialize<Resources>(GameObject.FindGameObjectWithTag("Player")
            .GetComponent<Player>().Resources, ResourcesPath);
    }
}
