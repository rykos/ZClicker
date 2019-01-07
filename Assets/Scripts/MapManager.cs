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
    private BuildingMemory selectedBuilding;//building the player is currently looking at
    private Dictionary<string, Building> nameToBuilding = new Dictionary<string, Building>();

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
            newBuilding.transform.localPosition = new Vector2(bm.PositionX, -2);
            BuildingManager buildingManager = newBuilding.GetComponent<BuildingManager>();
            LoadIntoBuildingManager(buildingManager, bm);
            newBuilding.transform.Find("Model").GetComponent<SpriteRenderer>().sprite = UnityEngine.Resources.Load<Sprite>("Buildings/" + bm.Name);
        }
        selectedBuilding = map.Buildings.Find(x => x.Name == "Goldmine");
        RequestUIUpdate();
        FindBuildingGameobject();
        PopulateBuildingDic();
    }
    private void Update()
    {
        int len = map.Buildings.Count;
        for (int i = 0; i < len; i++)
        {
            if (map.Buildings[i].BuildActive)//Buildings with active upgrade
            {
                UpgradeTimeTick(map.Buildings[i]);
                UpdateBuildingSlider();
            }
        }
    }

    private void UpdateBuildingSlider()
    {
        Building building = nameToBuilding[selectedBuilding.Name];
        float percentValue = building.TimeLeft / building.TimeToBuild;
        BuildingSlider.GetComponent<UnityEngine.UI.Slider>().value = 100 - (percentValue * 100);
    }

    private void PopulateBuildingDic()
    {
        Transform x = GameObject.Find("/Map").transform;
        foreach (Transform y in x)
        {
            if (y.CompareTag("Building"))
            {
                Building building = y.GetComponent<BuildingManager>().Building;
                nameToBuilding.Add(building.Name, building);
            }
        }
    }

    private void UpgradeTimeTick(BuildingMemory bm)
    {
        Building building = nameToBuilding[bm.Name];
        building.TimeLeft -= Time.deltaTime;
        if (building.TimeLeft <= 0)
        {
            Debug.Log("Finished building");
            int index = GetBuildingMemoryIndex(bm);
            map.Buildings[index] = map.Buildings[index].SwitchBuildState();
            UpgradeFinished(bm);
        }
    }

    private void UpgradeFinished(BuildingMemory bm)
    {
        this.map.UpgradeBuilding(bm);//Handles level up
        //this.selectedBuilding = map.Buildings.First(x => x.Name == selectedBuilding.Name);
        nameToBuilding[bm.Name].UpgradeState = false;
        if (this.selectedBuilding.Name == bm.Name)
        {
            this.selectedBuilding = bm;
        }
        RequestUIUpdate();
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
        selectedBuilding = map.Buildings.First(x => x.Name == selectedBuilding.Name);
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
            if (child.position.x == selectedBuilding.PositionX)
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
        nameToBuilding[this.selectedBuilding.Name].TimeToBuild = bu.Time;
        nameToBuilding[this.selectedBuilding.Name].TimeLeft = bu.Time;
        nameToBuilding[this.selectedBuilding.Name].UpgradeState = true;
        map.Buildings[GetBuildingMemoryIndex(selectedBuilding)] = map.Buildings[GetBuildingMemoryIndex(selectedBuilding)].SwitchBuildState();
    }

    private void RequestUIUpdate()
    {
        GameObject.Find("/UI").GetComponent<UIController>().UpdateBuildingUI(selectedBuilding);
    }

    private int GetBuildingMemoryIndex(BuildingMemory building)
    {
        return map.Buildings.IndexOf(building);
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
        Buildings.Add(new BuildingMemory("Goldmine", 
            "You can mine gold in here",
            BigFloat.BuildNumber(1),
            0,
            new Goldmine()));
        Buildings.Add(new BuildingMemory("Blacksmith", 
            "You can upgrade your hero in here", 
            BigFloat.BuildNumber(1),
            5.92f, 
            new Blacksmith()));
        Buildings.Add(new BuildingMemory("Alchemist", 
            "Create potions",
            BigFloat.BuildNumber(1),
            -5.75f, 
            new Alchemist()));
        Buildings.Sort((x, y) => x.PositionX.CompareTo(y.PositionX));
    }

    //Upgrades building
    public void UpgradeBuilding(BuildingMemory building)
    {
        int index = this.Buildings.IndexOf(this.Buildings.First(x => x.Name == building.Name));
        BuildingMemory newBuildingMem = this.Buildings[index].LevelUP();
        this.Buildings[index] = newBuildingMem;
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