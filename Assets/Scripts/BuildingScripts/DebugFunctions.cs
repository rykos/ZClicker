using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DebugFunctions : MonoBehaviour
{
    //[MenuItem("Tools/Flush Save")]
    private static void FlushSave()
    {
        string mapPath = Path.Combine(Application.persistentDataPath, "Map.bin");
        string ResourcesPath = Path.Combine(Application.persistentDataPath, "Resources.bin");

        RemoveFile(mapPath);
        RemoveFile(ResourcesPath);
    }

    private static void RemoveFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
