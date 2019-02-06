using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public Item item;
}

/// <summary>
/// Item logic information
/// </summary>
[System.Serializable]
public struct Item
{
    public ItemType itemType;
    public string Name;
    public string Description;
    public Item(string name, string desc, ItemType itemType)
    {
        this.Name = name;
        this.Description = desc;
        this.itemType = itemType;
    }
}

[System.Serializable]
public enum ItemType
{
    Potion,
    Equipment,
    Everything
}