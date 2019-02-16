using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Physical container for Hero
/// </summary>
public class GuildHero : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Level;
    public GameObject Avatar;

    public Hero Hero
    {
        get
        {
            return this.hero;
        }
        set
        {
            this.hero = value;
            LoadDisplay();
        }
    }
    private Hero hero;

    private void LoadDisplay()
    {
        Name.text = hero.Name;
        Level.text = hero.Level.level.ToString();
    }

    public void BuyHero()
    {
        (GameObject.Find("BuildingInterface_Guild").GetComponent<GuildBuildingManager>().building as Guild).Heroes.Add(this.hero);
        Destroy(this.gameObject);
    }
}

[System.Serializable]
public class Hero
{
    public Level Level;//Hero level
    public string Name;//Hero name
    public List<Item> Items;//Equipped items
    public HeroStats Stats;//Base stats

    public Hero(string name, Level level)
    {
        this.Name = name;
        this.Level = level;
        this.Items = new List<Item>();
        this.Stats = new HeroStats();
    }
}

[System.Serializable]
public class HeroStats//Can be alternated by external sources
{
    public BigFloat DPS;//Base damage dealt
    public BigFloat Health;//Base health
    public BigFloat SpellPower;//Base spell multiplier
    private List<Buff> buffs;
    private HeroStats basicStats;//Unchanged values

    public HeroStats(bool basicStats = false)
    {
        this.DPS = 0;
        this.Health = 100;
        this.SpellPower = 0;
        if (basicStats == false)
        {
            this.basicStats = new HeroStats(true);
            buffs = new List<Buff>();
        }
    }

    public void Recalculate()//Based of basicStats
    {
        BigFloat rawDps = basicStats.DPS;
        rawDps += CollectBuffValues(StatType.dps, BuffAlternationType.AddValue);
        rawDps *= CollectBuffValues(StatType.dps, BuffAlternationType.MultiplyValue);
        //
        BigFloat rawHealth = basicStats.Health;
        rawHealth += CollectBuffValues(StatType.health, BuffAlternationType.AddValue);
        rawHealth *= CollectBuffValues(StatType.health, BuffAlternationType.MultiplyValue);
        //
        BigFloat rawSpellPower = basicStats.SpellPower;
        rawSpellPower += CollectBuffValues(StatType.spellPower, BuffAlternationType.AddValue);
        rawSpellPower *= CollectBuffValues(StatType.spellPower, BuffAlternationType.MultiplyValue);

        this.DPS = rawDps;
        this.Health = rawHealth;
        this.SpellPower = rawSpellPower;
    }

    public void AddBuff(Buff[] buffs)
    {
        foreach (Buff buff in buffs)
        {
            this.buffs.Add(buff);
        }
        Recalculate();
    }

    public void RemoveBuff(Buff[] buffs)
    {
        foreach (Buff buff in buffs)
        {
            this.buffs.Remove(buff);
        }
        Recalculate();
    }

    public BigFloat CollectBuffValues(StatType statType, BuffAlternationType bat)
    {
        BigFloat fetchAmount = 0;
        foreach (Buff buff in this.buffs)
        {
            if (buff.statType == statType && buff.alterType == bat)
            {
                fetchAmount += buff.Value;
            }
        }
        return fetchAmount;
    }
}

/// <summary>
/// Alternates hero stats
/// </summary>
[System.Serializable]
public struct Buff
{
    public bool Fixed;//Buff is fixed, should not be removed
    public StatType statType;//Stat type this buff target
    public BigFloat Value;//Value for this buff
    public BuffAlternationType alterType;

    public Buff(StatType st, BigFloat value, BuffAlternationType alterType)
    {
        this.statType = st;
        this.Value = value;
        this.alterType = alterType;
        this.Fixed = false;
    }
}

public enum StatType
{
    dps,
    health,
    spellPower
}

public enum BuffAlternationType
{
    AddValue, //Adds raw value
    MultiplyValue, //Multiply by float
}


[System.Serializable]
public class Level
{
    public uint level;//Hero level should be capped
    public BigFloat Exp;
    public BigFloat ExpToLevel;

    public Level(uint level, BigFloat exp)
    {
        this.level = level;
        this.Exp = exp;
        //=100*(L3^1.5)*2
        this.ExpToLevel = 200*(Mathf.Pow(level, 2));
    }
}