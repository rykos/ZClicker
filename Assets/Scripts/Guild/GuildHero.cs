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
    public Level Level;
    public string Name;
    public List<Item> Items;
    public HeroStats Stats;

    public Hero(string name, Level level)
    {
        this.Name = name;
        this.Level = level;
    }
}

[System.Serializable]
public class HeroStats
{
    public BigFloat DPS;
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