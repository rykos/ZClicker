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
    private GameObject bossUI;
    [SerializeField]
    private GameObject TapValueTextGO;
    [SerializeField]
    private GameObject TapValueTextBossGO;
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
        item.transform.Find("Cost/CostTMP").GetComponent<TextMeshProUGUI>().text = string.Format(_Desc, cost.Value, cost.Exponent);
        item.transform.Find("Level/LevelTMP").GetComponent<TextMeshProUGUI>().text = upgrade.Level.ToString();
        item.transform.Find("Description/DescriptionTMP").GetComponent<TextMeshProUGUI>().text = string.Format(description, val.Value, val.Exponent);
    }
    public void UpdateBuildingUpgrade(UpgradeControllerData upgrade, GameObject item)//Null upgrade
    {
        var cost = upgrade.Cost.GetString();
        var val = upgrade.Value.GetString();
        item.transform.Find("Cost/CostTMP").GetComponent<TextMeshProUGUI>().text = string.Format(_Desc, cost.Value, cost.Exponent);
        item.transform.Find("Level/LevelTMP").GetComponent<TextMeshProUGUI>().text = "0";
        item.transform.Find("Description/DescriptionTMP").GetComponent<TextMeshProUGUI>().text = 
            string.Format(upgrade.Description, val.Value, val.Exponent);
    }
    public void UpdateBuildingUI(BuildingMemory building)
    {
        GameObject.Find("/UI/BuildingLevel/LevelTMP").GetComponent<TextMeshProUGUI>().text = 
            string.Format("{0}{1}", building.Level.baseNumber, building.Level.exponentChar);
    }

    public void ShowTapValue(GameObject canvas, Vector2 position, Tap tap)
    {
        GameObject usedGO = (canvas.CompareTag("BossInterface") ? TapValueTextBossGO : TapValueTextGO);
        var tapValueText = Instantiate(usedGO, canvas.transform);
        position = Camera.main.ScreenToWorldPoint(position);
        tapValueText.transform.position = new Vector3(position.x, position.y, canvas.transform.position.z - 1);
        //tapValueText.transform.position = new Vector3(tapValueText.transform.position.x, tapValueText.transform.position.y, -8);
        var tapstring = tap.amount.GetString();
        tapValueText.GetComponent<TextMeshProUGUI>().text = string.Format(_Desc, tapstring.Value, tapstring.Exponent);
        tapValueText.GetComponent<TextMeshProUGUI>().color = (tap.critical) ? Color.red : new Color(0.9411765f, 0.7686275f, 0.5450981f);
    }
    public void ShowTapString(GameObject canvas, Vector2 position, string text)
    {
        GameObject usedGO = (canvas.CompareTag("BossInterface") ? TapValueTextBossGO : TapValueTextGO);
        var tapText = Instantiate(usedGO, canvas.transform);
        position = Camera.main.ScreenToWorldPoint(position);
        tapText.transform.position = new Vector3(position.x, position.y, canvas.transform.position.z - 1);
        tapText.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void SwitchBossUI()
    {
        bossUI.SetActive(!bossUI.activeSelf);
        gameObject.SetActive(!bossUI.activeSelf);
    }
}