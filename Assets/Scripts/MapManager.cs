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
    public GameObject BuildingSlider;
    public static Player player;//Reference to player
    public static GameObject SelectedBuildingGameObject;
    public Map map;
    //
    private Building selectedBuilding;//building the player is currently looking at

    private void Awake()
    {
        map = new Map(Application.persistentDataPath);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    private void Start()
    {
        foreach (Building building in map.Buildings)
        {
            var newBuilding = Instantiate(buildingPrefab, this.transform);
            newBuilding.transform.localPosition = new Vector2(building.Position.X, -2);
            BuildingManager buildingManager = newBuilding.GetComponent<BuildingManager>();
            buildingManager.Build(building);
            newBuilding.transform.Find("Model").GetComponent<SpriteRenderer>().sprite = UnityEngine.Resources.Load<Sprite>("Buildings/" + building.Name);
        }
        selectedBuilding = map.Buildings.Find(x => x.Name == "Goldmine");
        RequestUIUpdate();
        FindBuildingGameobject();
    }
    private void Update()
    {
        int len = map.Buildings.Count;
        for (int i = 0; i < len; i++)
        {
            if (map.Buildings[i].UpgradeState)//Buildings with active upgrade
            {
                UpgradeTimeTick(map.Buildings[i]);
                UpdateBuildingSlider();
            }
        }
    }

    private void UpdateBuildingSlider()
    {
        float percentValue = selectedBuilding.TimeLeft / selectedBuilding.TimeToBuild;
        BuildingSlider.GetComponent<UnityEngine.UI.Slider>().value = 100 - (percentValue * 100);
    }

    private void UpgradeTimeTick(Building building)
    {
        building.TimeLeft -= Time.deltaTime;
        if (building.TimeLeft <= 0)
        {
            Debug.Log("Finished building");
            building.UpgradeState = false;
            UpgradeFinished(building);
        }
    }

    private void UpgradeFinished(Building building)
    {
        building.UpgradeState = false;
        this.map.UpgradeBuilding(building);//Handles level up
        //if (this.selectedBuilding.Name == building.Name)
        //{
        //    this.selectedBuilding = building;
        //}
        RequestUIUpdate();
    }

    public Building NextBuilding(Vector2 direction)
    {
        int index = map.Buildings.IndexOf(selectedBuilding) + (int)direction.x;
        selectedBuilding = (index >= 0 && index < map.Buildings.Count) ? map.Buildings[index] : selectedBuilding;
        GameObject.Find("/UI").GetComponent<UIController>().UpdateBuildingUI(selectedBuilding);
        FindBuildingGameobject();
        return selectedBuilding;
    }

    private void FindBuildingGameobject()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<BuildingManager>() == null)
            {
                continue;
            }
            if (child.position.x == selectedBuilding.Position.X)
            {
                SelectedBuildingGameObject = child.gameObject;
                break;
            }
        }
    }

    public void OpenBuildingMenu()
    {
        Debug.Log("Opening selected building menu");
        SelectedBuildingGameObject.GetComponent<BuildingManager>().SwitchMenu();
    }

    //Called on upgrade click
    public void UpgradeBuilding(BuildingUpgrade bu)
    {
        Debug.Log("Switched building state");
        selectedBuilding.TimeToBuild = bu.Time;
        selectedBuilding.TimeLeft = bu.Time;
        selectedBuilding.UpgradeState = true;
    }

    private void RequestUIUpdate()
    {
        GameObject.Find("/UI").GetComponent<UIController>().UpdateBuildingUI(selectedBuilding);
    }

    //private int GetBuildingMemoryIndex(BuildingMemory building)
    //{
    //    return map.Buildings.IndexOf(building);
    //}
}

[System.Serializable]
public class Map
{
    public List<Building> Buildings = new List<Building>();

    public Map(string persistentDataPath)
    {
        //Try to load save, if there is no file then create new
        string savePath = Path.Combine(persistentDataPath, "Map.bin");
        if(File.Exists(savePath))
        {
            Debug.Log("SaveLoaded");
            this.Buildings = SaveManagment.Deserialize<List<Building>>(savePath);
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
        Alchemist a = new Alchemist("Alchemist", "Desc", new Vector2(-5.75f, 0));
        Goldmine b = new Goldmine("Goldmine", "Desc", new Vector2(0, 0));
        Blacksmith c = new Blacksmith("Blacksmith", "Desc", new Vector2(5.92f, 0));
        this.Buildings.Add(a);
        this.Buildings.Add(b);
        this.Buildings.Add(c);
        Buildings.Sort((x, y) => x.Position.X.CompareTo(y.Position.X));
        //Buildings.Add(new BuildingMemory("Goldmine", 
        //    "You can mine gold in here",
        //    BigFloat.BuildNumber(1),
        //    0,
        //    new Goldmine()));
        //Buildings.Add(new BuildingMemory("Blacksmith", 
        //    "You can upgrade your hero in here", 
        //    BigFloat.BuildNumber(1),
        //    5.92f, 
        //    new Blacksmith()));
        //Buildings.Add(new BuildingMemory("Alchemist", 
        //    "Create potions",
        //    BigFloat.BuildNumber(1),
        //    -5.75f, 
        //    new Alchemist()));
    }

    //Upgrades building
    public void UpgradeBuilding(Building building)
    {
        building.LevelUP(BigFloat.BuildNumber(1));
    }
}

/// <summary>
/// Holds specific information about building.
/// Should Be used for save&load system only
/// </summary>
[System.Serializable]
public struct BuildingMemory
{
    public string Name;
    public string Description;
    public BigFloat Level;
    public float PositionX;
    public object Type;
    public bool BuildActive;

    public BuildingMemory(string name, string description, BigFloat level, float position, object type, bool buildActive = false)
    {
        this.Name = name;
        this.Description = description;
        this.Level = level;
        this.PositionX = position;
        this.Type = type;
        this.BuildActive = buildActive;
    }

    public BuildingMemory LevelUP()
    {
        return new BuildingMemory(this.Name, this.Description, this.Level + BigFloat.BuildNumber(1), this.PositionX, this.Type);
    }
    public BuildingMemory SwitchBuildState()
    {
        return new BuildingMemory(this.Name, this.Description, this.Level, this.PositionX, this.Type, !this.BuildActive);
    }
}