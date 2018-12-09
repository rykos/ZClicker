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
    private Map map = new Map();

    private void Start()
    {
        foreach (BuildingMemory bm in map.Buildings)
        {
            var newBuilding = Instantiate(buildingPrefab, this.transform);
            newBuilding.transform.localPosition = bm.Position;
            BuildingManager buildingManager = newBuilding.GetComponent<BuildingManager>();
            buildingManager.buildingName = bm.Name;
            buildingManager.buildingDescription = bm.Description;
        }
        selectedBuilding = map.Buildings.Find(x => x.Position == new Vector2(0, 0));
    }

    public BuildingMemory NextBuilding(Vector2 direction)
    {
        int index = map.Buildings.IndexOf(selectedBuilding) + (int)direction.x;
        selectedBuilding = (index >= 0 && index < map.Buildings.Count) ? map.Buildings[index] : selectedBuilding;
        return selectedBuilding;
    }
}

public class Map
{
    public List<BuildingMemory> Buildings = new List<BuildingMemory>();

    public Map()
    {
        BuildingMemory firstBuilding = new BuildingMemory("Test Building 1", "None", 1, new Vector2(0, 0), 0);
        BuildingMemory rightBuilding = new BuildingMemory("Test Building 2", "None", 2, new Vector2(6, 0), 0);
        BuildingMemory leftBuilding = new BuildingMemory("Test Building 3", "None", 3, new Vector2(-6, 0), 0);
        Buildings.Add(firstBuilding);
        Buildings.Add(rightBuilding);
        Buildings.Add(leftBuilding);
        Buildings.Sort((x, y) => x.Position.x.CompareTo(y.Position.x));
    }
}

/// <summary>
/// Holds specific information about building
/// </summary>
public struct BuildingMemory
{
    public string Name;
    public string Description;
    public int Level;
    public float TimeLeft;
    public Vector2 Position;

    public BuildingMemory(string name, string description, int level, Vector2 position, float timeLeft)
    {
        this.Name = name;
        this.Description = description;
        this.Level = level;
        this.TimeLeft = timeLeft;
        this.Position = position;
    }
}