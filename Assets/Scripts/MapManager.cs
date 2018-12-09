using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Contains access to all buildings in town
/// </summary>
public class MapManager : MonoBehaviour
{
    #region Predefined variables
    [SerializeField]
    private GameObject buildingPrefab;
    #endregion

    private BuildingMemory selectedBuilding;
    public Map map = new Map();

    private void Start()
    {
        foreach (BuildingMemory bm in map.Buildings)
        {
            var newBuilding = Instantiate(buildingPrefab, this.transform);
            newBuilding.transform.localPosition = new Vector2(bm.PositionX, 0);
            BuildingManager buildingManager = newBuilding.GetComponent<BuildingManager>();
            LoadIntoBuildingManager(buildingManager, bm);
        }
        selectedBuilding = map.Buildings.Find(x => x.Name == "Goldmine");
    }

    private void LoadIntoBuildingManager(BuildingManager buildingManager, BuildingMemory bm)
    {
        buildingManager.buildingName = bm.Name;
        buildingManager.buildingDescription = bm.Description;
        buildingManager.buildingLevel = bm.Level;
        buildingManager.Build(bm.Type);
    }

    public BuildingMemory NextBuilding(Vector2 direction)
    {
        int index = map.Buildings.IndexOf(selectedBuilding) + (int)direction.x;
        selectedBuilding = (index >= 0 && index < map.Buildings.Count) ? map.Buildings[index] : selectedBuilding;
        return selectedBuilding;
    }
}

[System.Serializable]
public class Map
{
    public List<BuildingMemory> Buildings = new List<BuildingMemory>();

    public Map()
    {
        BuildingMemory Goldmine = new BuildingMemory("Goldmine", "You can mine gold in here", 1, 
            0, 0, new Goldmine());
        BuildingMemory Blacksmith = new BuildingMemory("Blacksmith", "You can upgrade your hero in here", 1, 
            6, 0, new Blacksmith());
        BuildingMemory Alchemist = new BuildingMemory("Alchemist", "Create potions", 1, 
            -6, 0, new Alchemist());
        Buildings.Add(Goldmine);
        Buildings.Add(Blacksmith);
        Buildings.Add(Alchemist);
        Buildings.Sort((x, y) => x.PositionX.CompareTo(y.PositionX));
        //
    }
}

/// <summary>
/// Holds specific information about building
/// </summary>
[System.Serializable]
public struct BuildingMemory
{
    public string Name;
    public string Description;
    public int Level;
    public float TimeLeft;
    public int PositionX;
    public object Type;

    public BuildingMemory(string name, string description, int level, int position, float timeLeft, object type)
    {
        this.Name = name;
        this.Description = description;
        this.Level = level;
        this.TimeLeft = timeLeft;
        this.PositionX = position;
        this.Type = type;
    }
}