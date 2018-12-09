using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
    public string buildingName;
    public string buildingDescription;
    public int buildingLevel;
    public object x;
    private Building building;

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

public abstract class Building
{
    public string Name;
    public string Description;
    public int Level;
    public abstract void BuildingInteract();//Building tapped
}

public class Goldmine : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Mining gold");
    }
}

public class Alchemist : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Alchemist tapped");
    }
}

public class Blacksmith : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Blacksmith tapped");
    }
}