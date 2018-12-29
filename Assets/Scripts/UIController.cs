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
    #region Editor
    [SerializeField]
    private GameObject TapValueTextGO;
    public List<GameObject> buildingUIs = new List<GameObject>();
    #endregion
    private static readonly string _Desc = "{0:0.0}{1}";
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
        BigFloatString bfs = amount.GetString();
        gold.text = string.Format("{0:0.0}{1}", bfs.Value, bfs.Exponent);
    }

    public void UpdateBuildingUpgrade(UpgradeMemory upgrade, string description, GameObject item)//Existing upgrade
    {
        var cost = upgrade.Cost.GetString();
        var val = upgrade.Value.GetString();
        item.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = string.Format(_Desc, cost.Value, cost.Exponent);
        item.transform.Find("Level").GetComponent<TextMeshProUGUI>().text = upgrade.Level.ToString();
        item.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = 
            string.Format(description, val.Value, val.Exponent);
    }
    public void UpdateBuildingUpgrade(UpgradeControllerData upgrade, GameObject item)//Null upgrade
    {
        //Loads Data from editor
        var cost = upgrade.Cost.GetString();
        var val = upgrade.Value.GetString();
        item.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = string.Format(_Desc, cost.Value, cost.Exponent);
        item.transform.Find("Level").GetComponent<TextMeshProUGUI>().text = "0";
        item.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = 
            string.Format(upgrade.Description, val.Value, val.Exponent);
    }

    public void ShowTapValue(GameObject canvas, Vector2 position, Tap tap)
    {
        var tapValueText = Instantiate(TapValueTextGO, canvas.transform);
        tapValueText.transform.localPosition = position;
        var tapstring = tap.amount.GetString();
        tapValueText.GetComponent<TextMeshProUGUI>().text = string.Format(_Desc, tapstring.Value, tapstring.Exponent);
        tapValueText.GetComponent<TextMeshProUGUI>().color = (tap.critical) ? Color.red : Color.white;
    }
}