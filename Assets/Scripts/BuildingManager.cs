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
        SetUI();
    }

    public void Interact(Vector2 TappedPosition)
    {
        building.BuildingInteract(TappedPosition);
    }

    public void SwitchMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }

    private void Update()
    {
        building.TimedValue();
    }

    private void SetUI()
    {
        GameObject specificUI = GameObject.Find("UI").GetComponent<UIController>().
            buildingUIs.Find(x => x.gameObject.name == "BuildingInterface_" + buildingName).gameObject;
        var specificMenu = Instantiate(specificUI, menu.transform);
        specificMenu.GetComponent<Canvas>().worldCamera = Camera.main;
        building.Init();
    }
}

[System.Serializable]
public abstract class Building
{
    public List<UpgradeMemory> upgradeMemories;
    public string Name;
    public string Description;
    public int Level;
    public abstract void BuildingInteract(Vector2 TappedPosition);//Building tapped
    public abstract void Init();//Load
    public abstract void OnUpgrade();
    public abstract void TimedValue();
}

[System.Serializable]
public class Goldmine : Building
{
    private BigFloat idleValue = BigFloat.BuildNumber(0);//Value earned per second
    private BigFloat tapPower = BigFloat.BuildNumber(0);//Value earned per tap
    private float critical = 0f;
    public override void BuildingInteract(Vector2 TappedPosition)
    {
        Tap tap = ExecuteTap();
        MapManager.player.AddGold(tap.amount);
        //Show tap value on world UI
        GameObject.Find("UI").GetComponent<UIController>().ShowTapValue(MapManager.SelectedBuildingGameObject.transform.Find("Building_Canvas").gameObject, TappedPosition, tap);
    }

    //Collect tap upgrade variables
    private void CalculateTaps()
    {
        BigFloat newTapPower = CollectValues<BigFloat>(UpgradeType.ResourceOnTap);
        BigFloat ukValue = BigFloat.BuildNumber((float)CollectValues<BigFloat>(UpgradeType.ResourceOnTapPercent) + 1f);
        float newCriticalTapChance = CollectValues(UpgradeType.CriticalTap);
        this.tapPower = newTapPower;
        this.critical = newCriticalTapChance;
    }
    private void CalculateValueInTime()
    {
        idleValue = CollectValues<BigFloat>(UpgradeType.ValueInTime);
    }

    public override void Init()
    {
        if (upgradeMemories == null)
        {
            InitUpgrades();
        }
        CalculateTaps();
    }

    public override void OnUpgrade()
    {
        CalculateTaps();
        CalculateValueInTime();
    }

    private void InitUpgrades()
    {
        Debug.Log("InitUpgrades()");
        //Only one predefined upgrade
        upgradeMemories = new List<UpgradeMemory>();
        UpgradeMemory um = new UpgradeMemory("Pickaxe", 1, BigFloat.BuildNumber(1f), BigFloat.BuildNumber(10f), UpgradeType.ResourceOnTap);
        upgradeMemories.Add(um);
    }

    private Tap ExecuteTap()
    {
        float critB = UnityEngine.Random.Range(0f, 100f);
        if (critical > critB)
        {
            return new Tap(tapPower * BigFloat.BuildNumber(2), true);
        }
        else
        {
            return new Tap(tapPower, false);
        }
    }

    private float CollectValues(UpgradeType upgradeType)
    {
        float returnValue = 0;
        List<UpgradeMemory> ValidUpgrades = upgradeMemories.FindAll(x => x.UpgradeType == upgradeType);
        ValidUpgrades.ForEach(x =>
        {
            returnValue += (float)x.Value;
        });
        return returnValue;
    }
    private BigFloat CollectValues<T>(UpgradeType upgradeType)
    {
        BigFloat returnValue = BigFloat.BuildNumber(0);
        List<UpgradeMemory> ValidUpgrades = upgradeMemories.FindAll(x => x.UpgradeType == upgradeType);
        ValidUpgrades.ForEach(x =>
        {
            returnValue += x.Value;
        });
        return returnValue;
    }

    private float _updateTime = 0;
    public override void TimedValue()
    {
        _updateTime += Time.deltaTime;
        if (_updateTime > 1)
        {
            MapManager.player.AddGold(idleValue);
            _updateTime = 0;
        }
    }
}

[System.Serializable]
public class Alchemist : Building
{
    public override void BuildingInteract(Vector2 TappedPosition)
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
    public override void TimedValue()
    {
        //
    }
}

[System.Serializable]
public class Blacksmith : Building
{
    public override void BuildingInteract(Vector2 TappedPosition)
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

    public override void TimedValue()
    {
        //
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

public enum UpgradeStyle//How upgrade values are calculated
{
    Multiply,
    Add
}
public enum UpgradeType
{
    ResourceOnTapPercent,//Percentage per tap increase
    ResourceOnTap,//Raw value per tap increase
    CriticalTap,//Raw critical value increase
    ValueInTime//Raw value increase
}

public struct Tap
{
    public BigFloat amount;//Tapped amount
    public bool critical;//Was critical tap

    public Tap(BigFloat amount, bool critical)
    {
        this.amount = amount;
        this.critical = critical;
    }
}