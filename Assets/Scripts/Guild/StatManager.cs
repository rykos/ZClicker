using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatManager : MonoBehaviour
{
    public TextMeshProUGUI _value;
    public StatType statType;
    //Sync display data
    public void StatUpdate(Hero hero)
    {
        if (statType == StatType.health)
        {
            BigFloatString bfs = hero.Stats.Health;
            _value.text = bfs.ShortString();
        }
        else if (statType == StatType.dps)
        {
            BigFloatString bfs = hero.Stats.DPS;
            _value.text = bfs.ShortString();
        }
        else if (statType == StatType.spellPower)
        {
            BigFloatString bfs = hero.Stats.SpellPower;
            _value.text = bfs.ShortString();
        }
    }
}
