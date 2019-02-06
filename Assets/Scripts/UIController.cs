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
    private GameObject characterUI;
    [SerializeField]
    private GameObject TapValueTextGO;
    [SerializeField]
    private GameObject TapValueTextBossGO;
    public List<GameObject> buildingUIs = new List<GameObject>();
    #endregion
    public static UIType selectedUIType = UIType.village;
    private static readonly string _Desc = "{0:0.0}{1}";
    private Resources playerResources;
    private TextMeshProUGUI gold;
    private PlayerInputManager playerInputManager;


    private void Awake()
    {
        gold = GameObject.Find("Gold_TMP").GetComponent<TextMeshProUGUI>();
        playerInputManager = GameObject.Find("EventSystem").GetComponent<PlayerInputManager>();
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
        item.transform.Find("Level/LevelTMP").GetComponent<TextMeshProUGUI>().text = upgrade.Level.ToString();
        item.transform.Find("Description/DescriptionTMP").GetComponent<TextMeshProUGUI>().text = 
            string.Format(upgrade.Description, val.Value, val.Exponent);
    }
    public void UpdateBuildingUI(Building building)
    {
        GameObject.Find("/UI/BuildingLevel/LevelTMP").GetComponent<TextMeshProUGUI>().text = 
            string.Format("{0}{1}", building.Level.baseNumber, building.Level.exponentChar);
    }
    //Update inner building UI with sent parameter*
    public void UpdateBuildingInnerUI(GameObject target, GenericBuildingInnerUI gbi)
    {
        Debug.Log(target.gameObject);
        target.transform.Find("Interface/BuildingDescription/Description").GetComponent<TextMeshProUGUI>().text = gbi.ToString();
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

    public void SwitchUI(GameObject switchedUI)
    {
        switchedUI.SetActive(!switchedUI.activeSelf);
        if (switchedUI.activeSelf)
        {
            switch (switchedUI.tag)
            {
                case "VillageInterface":
                    selectedUIType = UIType.village;
                    break;

                case "BossInterface":
                    selectedUIType = UIType.boss;
                    break;

                case "CharacterInterface":
                    selectedUIType = UIType.character;
                    break;
                default:
                    break;
            }
        }
        playerInputManager.ChangePlayerInput(switchedUI);
    }
}

public enum UIType
{
    village,
    character,
    boss,
    buildingUpgrade
}

/// <summary>
/// Container for generic inner UI, 
/// </summary>
public struct GenericBuildingInnerUI
{
    public string MainDescription;
    public BigFloatString Cost;
    public string Time;

    public GenericBuildingInnerUI(string mainDescription, BigFloatString cost, string time)
    {
        this.MainDescription = mainDescription;
        this.Cost = cost;
        this.Time = time;
    }

    public override string ToString()
    {
        string result = string.Format("{0}\n{1:0}{2}G\n{3}", MainDescription, Cost.Value, Cost.Exponent, Time);
        return result;
    }
}