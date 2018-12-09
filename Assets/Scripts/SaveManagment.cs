using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManagment
{
    public static void Serialize<T>(T serialize, string name)
    {
        IFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "name");
        Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, serialize);
        stream.Close();
    }

    public static T Deserialize<T>(string name)
    {
        IFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "name");
        Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        T deserializedObject = (T)(object)formatter.Deserialize(stream);
        stream.Close();
        return deserializedObject;
    }
}
