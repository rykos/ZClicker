using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityManager : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI Description;

    private readonly string desc_title = "<style=\"Title\"><color=\"green\">{0}</color></style>\n";

    public void AbilityUpdate(Ability ability)
    {
        this.Description.text = string.Format(desc_title + ability.Description, ability.Name, ((BigFloatString)ability.Value).ShortString());
    }
}

[System.Serializable]
public class Ability
{
    public string Name;
    public string Description;
    public BigFloat Value;
    public float Cooldown;

    public virtual void Cast()
    {
        Debug.Log("Ability Cast");
    }
}

#region priest
[System.Serializable]
public class PartyHeal : Ability
{
    public PartyHeal(Hero hero)
    {
        this.Name = "Party Heal";
        this.Description = "heal party for {1}";
        this.Cooldown = 30;
        this.Value = 100;
    }

    public override void Cast()
    {
        Debug.Log($"Heal party for {this.Value*0.9f}");
    }
}

#endregion