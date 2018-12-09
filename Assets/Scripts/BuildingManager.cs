using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
    #region Building specific
    public string buildingName;
    public string buildingDescription;
    public int buildingLevel;
    private Building building;
    #endregion

    public void Build<T>(T type)
    {
        building = (Building)(object)type;
        building.Name = buildingName;
        building.Description = buildingDescription;
        building.Level = buildingLevel;
    }

    //Building collider clicked
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            building.BuildingInteract();
        }
    }
}

[System.Serializable]
public abstract class Building
{
    public string Name;
    public string Description;
    public int Level;
    public abstract void BuildingInteract();//Building tapped
}

[System.Serializable]
public class Goldmine : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Mining gold +1");
        MapManager.player.AddGold(1);
    }
}

[System.Serializable]
public class Alchemist : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Alchemist tapped");
    }
}

[System.Serializable]
public class Blacksmith : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Blacksmith tapped");
    }
}