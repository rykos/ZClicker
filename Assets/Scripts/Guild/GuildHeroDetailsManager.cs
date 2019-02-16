using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuildHeroDetailsManager : MonoBehaviour
{
    #region
    public GameObject Avatar;
    public TextMeshProUGUI Name_TMP;
    public TextMeshProUGUI Level_TMP;
    public GuildBuildingManager guildBuildingManager;
    #endregion
    private Hero activeHero;
    private void OnDisable()
    {
        activeHero = null;
        guildBuildingManager.IState = GuildInterfaceState.main;
    }
    private void OnEnable()
    {
        guildBuildingManager.IState = GuildInterfaceState.heroDetails;
    }

    public void FillDetails(Hero hero)
    {
        activeHero = hero;
        Name_TMP.text = hero.Name;
        Level_TMP.text = hero.Level.level.ToString();
    }

    public void ExitHeroDetials()
    {
        guildBuildingManager.FreezeTouch();
        gameObject.SetActive(false);
    }

    public void SellHero(Button sender)
    {
        this.guildBuildingManager.SellHero(this.activeHero);
    }
}
