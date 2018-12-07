using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingManager : MonoBehaviour
{
    [SerializeField]
    private string name;
    [SerializeField]
    private string description;
    private Building x = new TestBuilding();

    //Building collider clicked
    private void OnMouseDown()
    {
        x.BuildingInteract();
    }
}

public abstract class Building
{
    public string Name;
    public string Description;
    public int level;
    public abstract void BuildingInteract();
}

public class TestBuilding : Building
{
    public override void BuildingInteract()
    {
        Debug.Log("Interacted");
    }
}