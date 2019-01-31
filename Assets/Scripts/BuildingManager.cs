using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Generic unit. Active on every building
/// </summary>
public class BuildingManager : MonoBehaviour
{
    #region Building specific predefined
    public GameObject menu;
    public GameObject specificMenu;
    [SerializeField]
    private SelectedBuilding selectedBuilding;//Informs editor what class it should hold
    #endregion
    private Building building;
    public Building Building
    {
        get
        {
            return this.building;
        }
    }
    private Dictionary<SelectedBuilding, Type> sbToType = new Dictionary<SelectedBuilding, Type>
    {
        { SelectedBuilding.Goldmine, typeof(Goldmine) },
        { SelectedBuilding.Alchemist, typeof(Alchemist) },
        { SelectedBuilding.Blacksmith, typeof(Blacksmith) },
        { SelectedBuilding.Guild, typeof(Guild) }
    };

    //Builds this building with given type
    public void Build()
    {
        building = (Building)(object)Activator.CreateInstance(sbToType[this.selectedBuilding]);
        building.Position = (Vector2)transform.position;
        building.Name = transform.name;
        PersonalizeBuilding();
    }

    //Personalize building setting up specific items
    private void PersonalizeBuilding()
    {
        //SetUI();
    }

    public void Interact(Vector2 TappedPosition)
    {
        building.BuildingInteract(TappedPosition);
    }

    public void SwitchMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }

    private void Awake()
    {
        FetchSave();
        if (building == null)
        {
            Build();
            GameObject.Find("/Map").GetComponent<MapManager>().map.AddBuilding(this.building);
        }
    }

    private void Update()
    {
        building.TimedValue();
    }

    private void SetUI()
    {
        //Loads predefined ui to generic building gameobject
        GameObject specificUI = GameObject.Find("UI").GetComponent<UIController>().
            buildingUIs.Find(x => x.gameObject.name == "BuildingInterface_" + this.building.Name).gameObject;
        var specificMenu = Instantiate(specificUI, menu.transform);
        specificMenu.GetComponent<Canvas>().worldCamera = Camera.main;
        building.Init();
    }

    private void FetchSave()
    {
        building = GameObject.Find("/Map").GetComponent<MapManager>().map.FetchBuilding(transform.name);
    }

    //Sync building with its memory
    public void Sync(BuildingMemory bm)
    {
        this.building.Level = bm.Level;
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

[System.Serializable]
public struct Vector2Serialize
{
    public float X;
    public float Y;
    public Vector2Serialize(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }

    public static implicit operator Vector2(Vector2Serialize vec)
    {
        return new Vector2(vec.X, vec.Y);
    }
    public static implicit operator Vector2Serialize(Vector2 vec)
    {
        return new Vector2Serialize(vec.x, vec.y);
    }
}

public enum SelectedBuilding
{
    Goldmine,
    Blacksmith,
    Alchemist,
    Guild
}