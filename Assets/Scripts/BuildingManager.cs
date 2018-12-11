using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
    #region Building specific predefined
    public GameObject menu;
    public string buildingName;
    public string buildingDescription;
    public int buildingLevel;
    #endregion
    private Building building;

    public void Build<T>(T type)
    {
        building = (Building)(object)type;
        building.Name = buildingName;
        building.Description = buildingDescription;
        building.Level = buildingLevel;
        PersonalizeBuilding();
    }

    private void PersonalizeBuilding()
    {
        GameObject specificUI = GameObject.Find("UI").GetComponent<UIController>().
            buildingUIs.Find(x => x.gameObject.name == buildingName).gameObject;
        var specificMenu = Instantiate(specificUI, menu.transform);
    }

    public void Interact()
    {
        building.BuildingInteract();
    }

    public void SwitchMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }
}

[System.Serializable]
public abstract class Building
{
    public string Name;
    public string Description;
    public int Level;
    public abstract void BuildingInteract();//Building tapped
    public abstract void OpenMenu();
}

[System.Serializable]
public class Goldmine : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Mining gold +1");
        MapManager.player.AddGold(1);
    }

    public override void OpenMenu()
    {
        Debug.Log("Mining menu opened");
    }
}

[System.Serializable]
public class Alchemist : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Alchemist tapped");
    }

    public override void OpenMenu()
    {
        Debug.Log("Alchemist menu opened");
    }
}

[System.Serializable]
public class Blacksmith : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Blacksmith tapped");
    }

    public override void OpenMenu()
    {
        Debug.Log("Blacksmith menu opened");
    }
}