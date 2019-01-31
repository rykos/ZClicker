using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Physical container for Hero
/// </summary>
public class GuildHero : MonoBehaviour
{
    public Hero hero;
}

[System.Serializable]
public class Hero
{
    public Level Level;
    public string Name;
    public List<Item> Items;
    public HeroStats Stats;
}

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
}