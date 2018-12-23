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
        Debug.Log("Upgrade cost cost " + upgrade.Cost);
        item.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = upgrade.Cost.ToString();
        item.transform.Find("Level").GetComponent<TextMeshProUGUI>().text = upgrade.Level.ToString();
        item.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = string.Format("{0} gold per tap", upgrade.Value.ToString());
    }

    public void ShowTapValue(GameObject canvas, Vector2 position, string value)
    {
        var tapValueText = Instantiate(TapValueTextGO, canvas.transform);
        tapValueText.transform.localPosition = position;
        tapValueText.GetComponent<TextMeshProUGUI>().text = value;
    }
}