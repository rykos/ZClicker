using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingManager : MonoBehaviour
{
    [SerializeField]
    private string buildingName;
    [SerializeField]
    private string buildingDescription;
    private Building building = new TestBuilding();

    private void Start()
    {
        building.Name = buildingName;
        building.Description = buildingDescription;
        building.Level = 1;
    }

    //Building collider clicked
    private void OnMouseDown()
    {
        building.BuildingInteract();
    }
}

public abstract class Building
{
    public string Name;
    public string Description;
    public int Level;
    public abstract void BuildingInteract();//Building tapped
}

public class TestBuilding : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Building interaction");
    }
}