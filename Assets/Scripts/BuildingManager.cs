using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System;

public class BuildingManager : MonoBehaviour
{
    #region Building specific predefined
    public GameObject menu;
    public string buildingName;
    public string buildingDescription;
    public int buildingLevel;
    #endregion
    private Building building;
    public Building Building
    {
        get
        {
            return this.building;
        }
    }

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
        building.Init();
        //Load UpgradeMemory to Upgrades
        //foreach (UpgradeMemory um in this.building.upgradeMemories)
        //{

        //}
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
    public List<UpgradeMemory> upgradeMemories;
    public string Name;
    public string Description;
    public int Level;
    public abstract void BuildingInteract();//Building tapped
    public abstract void Init();//Load
    public abstract void OnUpgrade();
}

[System.Serializable]
public class Goldmine : Building
{
    private BigFloat tapPower = BigFloat.BuildNumber(0);
    public override void BuildingInteract()
    {
        MapManager.player.AddGold(tapPower);
    }

    public void CalculateTapPower()
    {
        BigFloat newTapPower = upgradeMemories.Find(x => x.Name == "Pickaxe").Value;
        this.tapPower = newTapPower;
    }

    public override void Init()
    {
        if (upgradeMemories == null)
        {
            InitUpgrades();
        }
        else
        {
            Debug.Log("List not empty" + upgradeMemories.Count);
        }
        CalculateTapPower();
    }

    public override void OnUpgrade()
    {
        CalculateTapPower();
    }

    private void InitUpgrades()
    {
        Debug.Log("InitUpgrades()");
        //Only one predefined upgrade
        upgradeMemories = new List<UpgradeMemory>();
        UpgradeMemory um = new UpgradeMemory("Pickaxe", 1, BigFloat.BuildNumber(1f), BigFloat.BuildNumber(10f), UpgradeType.ResourceOnTap);
        upgradeMemories.Add(um);
    }
}

[System.Serializable]
public class Alchemist : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Alchemist tapped");
    }

    public override void Init()
    {
        Debug.Log("Init");
    }

    public override void OnUpgrade()
    {
        throw new NotImplementedException();
    }
}

[System.Serializable]
public class Blacksmith : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Blacksmith tapped");
    }

    public override void Init()
    {
        Debug.Log("Init");
    }

    public override void OnUpgrade()
    {
        throw new NotImplementedException();
    }
}

//Represents each upgrade object memory
[System.Serializable]
public struct UpgradeMemory
{
    public string Name;//Building Name
    public int Level;//Building level
    public BigFloat Value;//Building use value
    public BigFloat Cost;//Cost to upgrade
    public DateTime? FinishTime;//Time at which action will be completed
    public UpgradeType UpgradeType;

    public UpgradeMemory(string name, int level, BigFloat value, BigFloat cost, UpgradeType upgradeType, DateTime? finishTime = null)
    {
        this.Name = name;
        this.Level = level;
        this.Value = value;
        this.Cost = cost;
        this.UpgradeType = upgradeType;
        this.FinishTime = finishTime;
    }
    public override string ToString()
    {
        return string.Format("Name:{0} Level:{1} Value:{2}", this.Name, this.Level, this.Value);
    }
}

public enum UpgradeType
{
    ResourceOnTap,
}