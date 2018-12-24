using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class UpgradeController : MonoBehaviour
{
    [SerializeField]
    private UpgradeControllerData upgrade;
    private static Player player;

    public void Upgrade()
    {
        UpgradeMemory updatedMemory;
        if (!CollectPay(upgrade))
        {
            return;
        }

        if (MemoryExists(upgrade))//Already exist, upgrade
        {
            Building building = FindParentBuilding().GetComponent<BuildingManager>().Building;
            int index = building.upgradeMemories.IndexOf(building.upgradeMemories.Find(bm => bm.Name == this.upgrade.Name));
            UpgradeMemory um = building.upgradeMemories[index];
            updatedMemory = NextUpgradeMemory(um);
            building.upgradeMemories[index] = updatedMemory;
        }
        else//Create new upgrade 
        {
            UpgradeMemory newUpgradeMemory = new UpgradeMemory
                (this.upgrade.Name, 1, this.upgrade.Value,
                this.upgrade.Cost, this.upgrade.upgradeType);
            updatedMemory = newUpgradeMemory;
            FindParentBuilding().gameObject.GetComponent<BuildingManager>().Building.upgradeMemories.Add(updatedMemory);
        }
        UpgradeAction(upgrade);
        GameObject.Find("UI").GetComponent<UIController>().UpdateBuildingUpgrade(updatedMemory, transform.gameObject);
    }

    private bool CollectPay(UpgradeControllerData upgrade)
    {
        BigFloat amount;
        if (MemoryExists(upgrade))
        {
            Building building = FindParentBuilding().GetComponent<BuildingManager>().Building;
            int index = building.upgradeMemories.IndexOf(building.upgradeMemories.Find(bm => bm.Name == this.upgrade.Name));
            UpgradeMemory um = building.upgradeMemories[index];
            amount = um.Cost;
        }
        else
        {
            amount = (this.upgrade.Cost);
        }
        if (player.Resources.Gold >= amount)
        {
            player.Resources.Gold -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpgradeAction(UpgradeControllerData upgrade)
    {
        Building building = FindParentBuilding().gameObject.GetComponent<BuildingManager>().Building;
        Interact(building);
    }
    
    private UpgradeMemory NextUpgradeMemory(UpgradeMemory upgrade)
    {
        BigFloat newValue = (this.upgrade.upgradeStyle == UpgradeStyle.Multiply) ?
            upgrade.Value * BigFloat.BuildNumber(this.upgrade.ValueMultiplier) :
            BigFloat.BuildNumber(this.upgrade.ValueMultiplier);
        UpgradeMemory newMemory = new UpgradeMemory(upgrade.Name, upgrade.Level++,
            newValue,
            (upgrade.Cost * BigFloat.BuildNumber(this.upgrade.CostMultiplier)),
            upgrade.UpgradeType);
        Debug.Log("Created new upgrade: " + newMemory.Level + " " + newMemory.Name);
        return newMemory;
    }

    private bool MemoryExists(UpgradeControllerData upgrade)
    {
        var result = FindParentBuilding().gameObject.GetComponent<BuildingManager>().Building.upgradeMemories.
            Find(bm => bm.Name == upgrade.Name);
        if (result.Equals(default(UpgradeMemory)))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private Transform FindParentBuilding()
    {
        Transform parentBuilding = transform.parent;
        while(parentBuilding.GetComponent<BuildingManager>() == null)
        {
            parentBuilding = parentBuilding.parent;
            if (parentBuilding.GetComponent<MapManager>() != null)
            {
                throw new System.Exception();
            }
        }
        return parentBuilding;
    }

    private void Interact(Building building)
    {
        building.OnUpgrade();
    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        //Pulling data for app startup
        UpgradeMemory um = FindParentBuilding().GetComponent<BuildingManager>().Building.upgradeMemories
            .Find(x => x.Name == this.upgrade.Name);
        if (!um.Equals(default(UpgradeMemory)))
        {
            GameObject.Find("UI").GetComponent<UIController>().UpdateBuildingUpgrade(um, transform.gameObject);
        }
    }
}

interface IUpgrade
{
    string Name
    { get; set; }
    void Interact(Building building);
}

[Serializable]
struct UpgradeControllerData//Hand tweaked data inside Editor
{
    public string Name;
    public string Description;
    public BigFloat Value;
    public BigFloat Cost;
    public float BasicUpgradeTime;
    public UpgradeType upgradeType;
    public UpgradeStyle upgradeStyle;
    //
    public float ValueMultiplier;
    public float CostMultiplier;
    public float TimeMultiplier;
}

//Struct handling big numbers
[System.Serializable]
public struct BigFloat
{
    public float baseNumber;//base number
    public int exponent;//e
    public char? exponentChar;//short name
    public static Dictionary<int, char> expoChars = new Dictionary<int, char>()
        {
            { 0, ' '},
            { 3, 'K'},
            { 6, 'M'},
            { 9, 'B'},
            { 12, 't'},
            { 15, 'q'},
            { 18, 'Q'},
            { 21, 's'},
            { 24, 'S'},
            { 27, 'O'},
            { 30, 'N'},
            { 33, 'd'},
            { 36, 'U'},
            { 39, 'D'},
            { 42, 'T'},
            { 45, 'X'},
        };
    public static BigFloat BuildNumber(float amount)
    {
        if (amount < 1000)
        {
            return new BigFloat(amount, 0);
        }
        int exponent = (int)Math.Floor(Math.Log10((double)amount));
        int newExponent = expoChars.Keys.Where(x => x <= exponent).Max();
        return new BigFloat(amount / (float)Math.Pow(10, newExponent), newExponent, expoChars[(int)newExponent]);
    }

    public BigFloat(float number, int exponent, char? expochar = null)
    {
        this.baseNumber = number;
        this.exponent = exponent;
        this.exponentChar = expochar;
    }

    public static BigFloat operator +(BigFloat a, BigFloat b)
    {
        int commonExponent = Math.Max(a.exponent, b.exponent);
        int exponentDifference = Math.Abs(a.exponent - b.exponent);
        int newExponent;
        float newBaseNumber;
        if (a.exponent > b.exponent)//A
        {
            newBaseNumber = b.baseNumber / (float)Math.Pow(10, exponentDifference) + a.baseNumber;
            newExponent = a.exponent;
        }
        else//B
        {
            newBaseNumber = a.baseNumber / (float)Math.Pow(10, exponentDifference) + b.baseNumber;
            newExponent = b.exponent;
        }
        if (newBaseNumber >= 1000)
        {
            int exponent = (int)Math.Floor(Math.Log10((double)newBaseNumber));
            newBaseNumber = newBaseNumber / (float)Math.Pow(10, exponent);
            newExponent += exponent;
        }
        return new BigFloat(newBaseNumber, newExponent, GetShortName(newExponent));
    }
    public static BigFloat operator -(BigFloat a, BigFloat b)
    {
        int commonExponent = Math.Max(a.exponent, b.exponent);
        int exponentDifference = Math.Abs(a.exponent - b.exponent);
        int newExponent;
        float newBaseNumber;
        if (a.exponent > b.exponent)//A
        {
            newBaseNumber = b.baseNumber / (float)Math.Pow(10, exponentDifference) - a.baseNumber;
            newExponent = a.exponent;
        }
        else//B
        {
            newBaseNumber = a.baseNumber / (float)Math.Pow(10, exponentDifference) - b.baseNumber;
            newExponent = b.exponent;
        }
        if (newBaseNumber >= 1000)
        {
            int exponent = (int)Math.Floor(Math.Log10((double)newBaseNumber));
            newBaseNumber = newBaseNumber / (float)Math.Pow(10, exponent);
            newExponent += exponent;
        }
        return new BigFloat(newBaseNumber, newExponent, GetShortName(newExponent));
    }
    public static BigFloat operator *(BigFloat a, BigFloat b)
    {
        float newBaseNumber = (a.baseNumber * b.baseNumber);
        int newExponent = a.exponent + b.exponent;
        if (newBaseNumber >= 1000)
        {
            int exponent = (int)Math.Floor(Math.Log10(newBaseNumber));
            newBaseNumber = newBaseNumber / (float)Math.Pow(10, exponent);
            newExponent += exponent;
        }
        return new BigFloat(newBaseNumber, newExponent, GetShortName(newExponent));
    }
    public static bool operator >(BigFloat a, BigFloat b)
    {
        if (a.exponent > b.exponent)
        {
            return true;
        }
        else if (a.exponent == b.exponent)
        {
            if (a.baseNumber > b.baseNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    public static bool operator <(BigFloat b, BigFloat a)
    {
        if (a.exponent > b.exponent)
        {
            return true;
        }
        else if (a.exponent == b.exponent)
        {
            if (a.baseNumber > b.baseNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    public static bool operator >=(BigFloat a, BigFloat b)
    {
        if (a.exponent > b.exponent)
        {
            return true;
        }
        else if (a.exponent == b.exponent)
        {
            if (a.baseNumber >= b.baseNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    public static bool operator <=(BigFloat b, BigFloat a)
    {
        if (a.exponent > b.exponent)
        {
            return true;
        }
        else if (a.exponent == b.exponent)
        {
            if (a.baseNumber >= b.baseNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private static char GetShortName(int exponent)
    {
        return expoChars[expoChars.Keys.Where(x => x <= exponent).Max()];
    }

    public override string ToString()
    {
        if (exponent > 0)
        {
            return string.Format("{0}{1}", baseNumber.ToString("N1"), exponentChar);
        }
        else
        {
            return string.Format(baseNumber.ToString("N1"));
        }
    }
}