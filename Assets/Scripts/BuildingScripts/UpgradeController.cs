using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Single building upgrade
/// </summary>
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
            Debug.Log("Already exist, upgrade");
            Building building = FindParentBuilding().GetComponent<BuildingManager>().Building;
            int index = building.upgradeMemories.IndexOf(building.upgradeMemories.Find(bm => bm.Name == this.upgrade.Name));
            UpgradeMemory um = building.upgradeMemories[index];
            updatedMemory = NextUpgradeMemory(um);
            building.upgradeMemories[index] = updatedMemory;
        }
        else//Create new upgrade 
        {
            Debug.Log("Create new upgrade ");
            UpgradeMemory newUpgradeMemory = new UpgradeMemory
                (this.upgrade.Name, upgrade.Level, this.upgrade.Value,
                this.upgrade.Cost, this.upgrade.upgradeType);
            updatedMemory = NextUpgradeMemory(newUpgradeMemory);
            FindParentBuilding().gameObject.GetComponent<BuildingManager>().Building.upgradeMemories.Add(updatedMemory);
        }
        UpgradeAction(upgrade);
        GameObject.Find("UI").GetComponent<UIController>().UpdateBuildingUpgrade(updatedMemory, this.upgrade.Description, transform.gameObject);
    }

    private bool CollectPay(UpgradeControllerData upgrade)
    {
        lock (player.Resources.GoldLock)
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
            Debug.Log("Pay: (" + amount.ToString() + ") from (" + player.Resources.Gold.ToString() + ")");
            if (player.Resources.Gold >= amount)
            {
                player.Resources.Gold -= amount;
                Debug.Log("Finalized with " + player.Resources.Gold);
                return true;
            }
            else
            {
                Debug.Log("Not enough gold");
                return false;
            }
        }
    }

    private void UpgradeAction(UpgradeControllerData upgrade)
    {
        Building building = FindParentBuilding().gameObject.GetComponent<BuildingManager>().Building;
        Interact(building);
    }

    private UpgradeMemory NextUpgradeMemory(UpgradeMemory upgrade)
    {
        UpgradeMemory newMemory;
        if (upgrade.Level == 0)//Not bought yet
        {
            BigFloat newValue = (this.upgrade.upgradeStyle == UpgradeStyle.Multiply) ?
            upgrade.Value * BigFloat.BuildNumber(this.upgrade.ValueMultiplier) :
            upgrade.Value + BigFloat.BuildNumber(this.upgrade.ValueMultiplier);
            newMemory = new UpgradeMemory(upgrade.Name, upgrade.Level + 1, upgrade.Value, upgrade.Cost, upgrade.UpgradeType);
        }
        else//Already bought
        {
            BigFloat newValue = (this.upgrade.upgradeStyle == UpgradeStyle.Multiply) ?
            upgrade.Value * BigFloat.BuildNumber(this.upgrade.ValueMultiplier) :
            upgrade.Value + BigFloat.BuildNumber(this.upgrade.ValueMultiplier);
            newMemory = new UpgradeMemory(upgrade.Name, upgrade.Level + 1,
                newValue,
                (upgrade.Cost * BigFloat.BuildNumber(this.upgrade.CostMultiplier)),
                upgrade.UpgradeType);
        }
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
            Debug.Log("Memory found.");
            Debug.Log(this.upgrade.Description);
            GameObject.Find("UI").GetComponent<UIController>().UpdateBuildingUpgrade(um, this.upgrade.Description, transform.gameObject);
        }
        else
        {
            Debug.Log("Memory not found.");
            GameObject.Find("UI").GetComponent<UIController>().UpdateBuildingUpgrade(this.upgrade, transform.gameObject);
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
public struct UpgradeControllerData//Hand tweaked data inside Editor
{
    public string Name;
    public string Description;
    public int Level;
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

//Struct handling large numbers
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
            newBaseNumber = (float)((double)a.baseNumber - (double)b.baseNumber / Math.Pow(10, exponentDifference));
            newExponent = a.exponent;
        }
        else//B
        {
            newBaseNumber = a.baseNumber / (float)Math.Pow(10, exponentDifference) - b.baseNumber;
            newExponent = b.exponent;
        }
        if (newBaseNumber >= 1000)
        {
            int exponent = (int)Math.Floor(Math.Log10(newBaseNumber));
            newBaseNumber = newBaseNumber / (float)Math.Pow(10, exponent);
            newExponent += exponent;
        }
        if (newBaseNumber < 1 && newBaseNumber > 0)
        {
            int exponent = (int)Math.Floor(Math.Log10(newBaseNumber));
            newBaseNumber = newBaseNumber / (float)Math.Pow(10, exponent);
            newExponent += exponent;
        }
        else if (newBaseNumber <= 0)
        {
            newExponent = 0;
        }
        //return new BigFloat(newBaseNumber, newExponent, GetShortName(newExponent));
        return ReBuild(newBaseNumber, newExponent, GetExponent(newExponent));
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
    public static BigFloat operator /(BigFloat a, BigFloat b)
    {
        if (a.baseNumber <= 0 || b.baseNumber <= 0)
        {
            return BuildNumber(0);
        }
        float newBaseNumber = (a.baseNumber / b.baseNumber);
        int newExponent = a.exponent - b.exponent;
        if (newBaseNumber < 1)
        {
            int expo = PullLowerExponent(newExponent);
            int expoDifference = newExponent - expo;
            newExponent = expo;
            newBaseNumber = newBaseNumber * (float)Math.Pow(10, expoDifference);
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

    private static int PullLowerExponent(int activeExpo)
    {
        int[] expos = expoChars.Keys.Where(x => x < activeExpo).ToArray();
        if (expos.Length == 0)
        {
            return 0;
        }
        return expoChars.Keys.Where(x => x < activeExpo).ToArray().Last();
    }
    private static BigFloat ReBuild(float baseNumber, int exponent, int targetExponent)
    {
        //t=3 e=5 d=2 | t=0 e=1 d=1
        baseNumber = baseNumber * (float)Math.Pow(10, exponent - targetExponent);
        return new BigFloat(baseNumber, targetExponent, GetShortName(targetExponent));
    }
    private static int GetExponent(int expo)
    {
        if (expo < 0)
        {
            return 0;
        }
        return expoChars.Keys.Where(x => x <= expo).Max();
    }
    private static char GetShortName(int exponent)
    {
        var expos = expoChars.Keys.Where(x => x <= exponent);
        if (expos.Count() > 0)
        {
            return expoChars[expos.Max()];
        }
        else
        {
            return ' ';
        }
    }
    public static explicit operator float(BigFloat bigFloat)
    {
        return bigFloat.baseNumber * (float)(Math.Pow(10, bigFloat.exponent));
    }
    public static implicit operator BigFloat(float number)
    {
        return BigFloat.BuildNumber(number);
    }

    public override string ToString()
    {
        if (exponent > 0)
        {
            return string.Format("{0}{1}", baseNumber.ToString("N1"), exponentChar);
        }
        else
        {
            return string.Format(baseNumber.ToString("N3"));
        }
    }

    public BigFloatString GetString()
    {
        BigFloatString returnValue;
        if (exponent > 0)
        {
            return new BigFloatString(baseNumber, exponentChar.ToString());
        }
        else
        {
            return new BigFloatString(baseNumber, "");
        }
    }
}

public struct BigFloatString
{
    public float Value;
    public string Exponent;

    public BigFloatString(float value, string expo)
    {
        this.Value = value;
        this.Exponent = expo;
    }

    public static implicit operator BigFloatString(BigFloat bigFloat)
    {
        return new BigFloatString(bigFloat.baseNumber, bigFloat.exponentChar.ToString());
    }

    public string ShortString()
    {
        return $"{Value:0.}{Exponent}";
    }
}