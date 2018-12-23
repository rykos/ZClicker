using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

/// <summary>
/// Contains access to all buildings in town
/// </summary>
public class MapManager : MonoBehaviour
{
    #region Predefined variables
    [SerializeField]
    private GameObject buildingPrefab;
    #endregion
    public static Player player;//Reference to player
    private BuildingMemory selectedBuilding;//building the player is currently looking at
    public static GameObject SelectedBuildingGameObject;
    public Map map;

    private void Awake()
    {
        map = new Map(Application.persistentDataPath);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
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
        FindBuildingGameobject();
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
        FindBuildingGameobject();
        return selectedBuilding;
    }

    private void FindBuildingGameobject()
    {
        foreach (Transform child in transform)
        {
            if (child.position.x == selectedBuilding.PositionX)
            {
                SelectedBuildingGameObject = child.gameObject;
                break;
            }
        }
    }
}

[System.Serializable]
public class Map
{
    public List<BuildingMemory> Buildings = new List<BuildingMemory>();

    public Map(string persistentDataPath)
    {
        //Try to load save, if there is no file then create new
        string savePath = Path.Combine(persistentDataPath, "Map.bin");
        if(File.Exists(savePath))
        {
            Debug.Log("SaveLoaded");
            this.Buildings = SaveManagment.Deserialize<List<BuildingMemory>>(savePath);
        }
        else
        {
            FirstInit();
            Debug.Log("FirstInit");
        }
    }

    //Needs refactorization
    private void FirstInit()
    {
        Buildings.Add(new BuildingMemory("Goldmine", "You can mine gold in here", 1,
            0, 0, new Goldmine()));
        Buildings.Add(new BuildingMemory("Blacksmith", "You can upgrade your hero in here", 1,
            6, 0, new Blacksmith()));
        Buildings.Add(new BuildingMemory("Alchemist", "Create potions", 1,
            -6, 0, new Alchemist()));
        Buildings.Sort((x, y) => x.PositionX.CompareTo(y.PositionX));
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