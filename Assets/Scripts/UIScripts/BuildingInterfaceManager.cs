using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInterfaceManager : MonoBehaviour
{
    //Hides active building menu
    public void HideMenu()
    {
        Debug.Log("Hiding menu");
        transform.parent.gameObject.SetActive(false);
    }
}
