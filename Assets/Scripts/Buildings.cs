using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class Building
{
    public Vector2Serialize Position;
    public List<UpgradeMemory> upgradeMemories;
    public string Name;
    public string Description;
    public bool UpgradeState;
    public float TimeToBuild;//Overall Time
    public float TimeLeft;
    public BigFloat Level;
    public virtual void BuildingInteract(Vector2 TappedPosition)
    {
        if (UpgradeState == true)
        {
            TimeLeft -= 1;
            GameObject.Find("UI").GetComponent<UIController>().ShowTapString(MapManager.SelectedBuildingGameObject.transform.Find("Building_Canvas").gameObject, TappedPosition, "-1s");
        }
    }
    public abstract void Init();//Load
    public abstract void OnUpgrade();
    public abstract void TimedValue();//Executed 
    public virtual void LevelUP(BigFloat amount)
    {
        this.Level = this.Level + amount;
        Debug.Log("<color=red>LEVELUP</color>");
    }

    protected Building()
    {
        this.Level = BigFloat.BuildNumber(1);
        this.UpgradeState = false;
        if (upgradeMemories == null)
        {
            upgradeMemories = new List<UpgradeMemory>();
        }
    }
}

[System.Serializable]
public class Goldmine : Building
{
    private BigFloat idleValue = BigFloat.BuildNumber(0);//Value earned per second
    private BigFloat tapPower = BigFloat.BuildNumber(1);//Value earned per tap
    private float critical = 0f;
    public override void BuildingInteract(Vector2 TappedPosition)
    {
        base.BuildingInteract(TappedPosition);
        if (UpgradeState == false)
        {
            Tap tap = ExecuteTap();
            MapManager.player.AddGold(tap.amount);
            //Show tap value on world UI
            GameObject.Find("UI").GetComponent<UIController>().ShowTapValue(MapManager.SelectedBuildingGameObject.transform.Find("Building_Canvas").gameObject, TappedPosition, tap);
        }
    }

    //Collect tap upgrade variables
    private void CalculateTaps()
    {
        BigFloat newTapPower = CollectValues<BigFloat>(UpgradeType.ResourceOnTap);
        BigFloat ukValue = BigFloat.BuildNumber((float)CollectValues<BigFloat>(UpgradeType.ResourceOnTapPercent) + 1f);
        float newCriticalTapChance = CollectValues(UpgradeType.CriticalTap);
        this.tapPower = 1 + newTapPower;
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
        base.BuildingInteract(TappedPosition);
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
        base.BuildingInteract(TappedPosition);
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

/// <summary>
/// Guild of heroes
/// </summary>
[System.Serializable]
public class Guild : Building
{

    public override void Init()
    {
        throw new NotImplementedException();
    }

    public override void OnUpgrade()
    {
        throw new NotImplementedException();
    }

    public override void TimedValue()
    {
        
    }
}