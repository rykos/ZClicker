using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Resources Resources;

    private void Awake()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "Resources.bin");
        if (File.Exists(savePath))
        {
            this.Resources = SaveManagment.Deserialize<Resources>(savePath);
        }
        else
        {
            this.Resources = new Resources();
        }
    }

    public void AddGold(int amount)
    {
        this.Resources.Gold += amount;
    }
}

[System.Serializable]
public class Resources
{
    public int Gold;
}