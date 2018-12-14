using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Responsible for keeping ui up to date
/// </summary>
public class UIController : MonoBehaviour
{
    public List<GameObject> buildingUIs = new List<GameObject>();
    private Resources playerResources;
    private TextMeshProUGUI gold;

    private void Awake()
    {
        gold = GameObject.Find("Gold_TMP").GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        playerResources = MapManager.player.Resources;
    }
    private void FixedUpdate()
    {
        SetUiGold(playerResources.Gold);
    }

    public void SetUiGold(BigFloat amount)
    {
        gold.text = amount.ToString();
    }

    public void UpdateBuildingUpgrade(UpgradeMemory upgrade, GameObject item)
    {
        Debug.Log("Typing cost " + upgrade.Cost);
        item.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = upgrade.Cost.ToString();
    }
}