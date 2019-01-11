using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInterfaceManager : MonoBehaviour
{
    public BuildingUpgrade buildingUpgrade;
    //Hides active building menu
    public void HideMenu()
    {
        Debug.Log("Hiding menu");
        transform.parent.gameObject.SetActive(false);
    }

    //Executed on upgrade building button press
    public void UpgradeBuilding()
    {
        Debug.Log("Proceed to upgrade building");
        if (CollectPay(this.buildingUpgrade.GoldCost))
        {
            GameObject.Find("/Map").GetComponent<MapManager>().UpgradeBuilding(this.buildingUpgrade);
        }
        else
        {
            Debug.Log("<color=red>Not enough money</color>");
        }
    }

    //Returns success status
    private bool CollectPay(BigFloat payAmount)
    {
        lock (MapManager.player.Resources.GoldLock)
        {
            if (MapManager.player.Resources.Gold < payAmount)
            {
                return false;
            }
            else
            {
                MapManager.player.Resources.Gold -= payAmount;
            }
        }
        return true;
    }
}

//Upgrade Cost
[System.Serializable]
public struct BuildingUpgrade
{
    public BigFloat GoldCost;//
    public float Time;//
    public BigFloat CostMulti;
    public float TimeAdd;
    public bool Multiplication;

    public BuildingUpgrade(BigFloat cost, float time, BigFloat costMulti, float timeAdd, bool multiplication)
    {
        this.GoldCost = cost;
        this.Time = time;
        this.CostMulti = costMulti;
        this.TimeAdd = timeAdd;
        this.Multiplication = multiplication;
    }
}