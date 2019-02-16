using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuildBuildingManager : MonoBehaviour, IInput
{
    #region Editor
    public GameObject DungeonsUI;
    public GameObject RecrutationUI;
    public GameObject HeroDetailsUI;
    public GameObject HeroDetailsUIShop;
    public GameObject HeroPreviewPrefab;
    #endregion
    public GuildInterfaceState IState
    {
        get { return this.istate; }
        set
        {
            this.istate = value;
            if (this.istate == GuildInterfaceState.main)
            {
                LoadHeroes();
            }
        }
    }
    private GuildInterfaceState istate = GuildInterfaceState.main;
    private bool touchSensitive = true;
    private float touchFrozenTime = 0f;
    private TouchHandler touchHandler = new TouchHandler();
    private UIHandler uiHandler;
    public Building building;

    private void Awake()
    {
        uiHandler = new UIHandler(this.gameObject, this);
        this.building = FindBuildingManagerInParent().Building;
    }
    private void Update()
    {
        if (touchSensitive == false)
        {
            Debug.Log("FrozenTime");
            if (touchFrozenTime <= 0)
            {
                touchSensitive = true;
            }
            touchFrozenTime -= Time.deltaTime;
            return;
        }
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                TouchBegan(touch);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                TouchMoved(touch);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                TouchEnded(touch);
            }
        }
        touchHandler.TouchAddTime(Time.deltaTime);
    }

    //Open hero details page, and fills it with hero details
    private void OpenHeroDetails(Hero hero, GameObject prefab)
    {
        //prefab.SetActive(true);
        SwitchTo(prefab);
        prefab.GetComponent<GuildHeroDetailsManager>().FillDetails(hero);
    }

    public List<GameObject> FetchGameObjects(Vector2 position)
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = position;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, raycastResults);
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (var res in raycastResults)
        {
            if (res.gameObject.CompareTag("UI#IGNORE"))
            {
                break;
            }
            gameObjects.Add(res.gameObject);
        }
        return gameObjects;
    }

    public void Button_Dungeons()
    {
        Debug.Log("Dungeons");
        IState = GuildInterfaceState.Dungeons;
        DungeonsUI.SetActive(true);
    }

    public void Button_Recrutation()
    {
        //Debug.Log("Recrutation");
        //List<Hero> heroes = (FindBuildingManagerInParent().Building as Guild).Heroes;
        //heroes.Add(new Hero("Bob", new Level(24, 1000)));
        //LoadHeroes();
        //IState = GuildInterfaceState.Recrutation;
        //RecrutationUI.SetActive(true);
        this.uiHandler.SwitchUITo(RecrutationUI);
    }

    public void FreezeTouch(float time = 0.1f)
    {
        touchSensitive = false;
    }

    public void OnEnable()
    {
        LoadHeroes();
    }

    //Loads heroes gameobjects to display list 
    public void LoadHeroes()
    {
        GameObject list = GameObject.Find("Interface/Content/ScrollingList");
        Transform buildingT = this.transform;
        var heroes = (FindBuildingManagerInParent().Building as Guild).Heroes;
        foreach (Hero hero in heroes)
        {
            List<Hero> loadedHeroes = LoadedHeroes(list.transform);
            if (loadedHeroes.Contains(hero))
            {
                Debug.Log("We have him");
            }
            else
            {
                GameObject HeroGO = Instantiate(HeroPreviewPrefab, list.transform);
                HeroGO.GetComponent<GuildHero>().Hero = hero;
            }
        } 
    }

    public void DestroyHeroGameobject(Hero hero)
    {
        GameObject list = GameObject.Find("Interface/Content/ScrollingList");
        foreach (Transform t in list.transform)
        {
            GuildHero gHero = t.GetComponent<GuildHero>();
            if (gHero != null)
            {
                if(gHero.Hero == hero)
                {
                    Destroy(t.gameObject);
                    break;
                }
            }
        }
    }

    private List<Hero> LoadedHeroes(Transform list)
    {
        List<Hero> heroes = new List<Hero>();
        foreach (Transform heroGO in list)
        {
            heroes.Add(heroGO.GetComponent<GuildHero>().Hero);
        }
        return heroes;
    }

    private BuildingManager FindBuildingManagerInParent()
    {
        Transform lt = this.transform;
        while (lt.GetComponent<BuildingManager>() == null)
        {
            lt = lt.parent;
        }
        return lt.GetComponent<BuildingManager>();
    }

    public void SwitchTo(GameObject newUI)
    {
        this.uiHandler.SwitchUITo(newUI);
    }

    #region TouchHandlers
    public void TouchBegan(Touch touch)
    {
        if (this.IState == GuildInterfaceState.main)
        {
            touchHandler.AddTouch(touch);
        }
    }

    public void TouchMoved(Touch touch)
    {
        Debug.Log(touch.fingerId + " moved");
    }

    public void TouchEnded(Touch touch)
    {
        List<GameObject> clickedGameObjects = FetchGameObjects(touch.position);
        TouchTime tt = touchHandler.GetTouch((byte)touch.fingerId);

        if (this.IState == GuildInterfaceState.main)
        {
            foreach (var go in clickedGameObjects)
            {
                if (tt.Time < 0.10f)
                {
                    HeroClick(go);
                }
            }
        }
        else if(this.IState == GuildInterfaceState.heroDetails)
        {
            
        }
        else if (this.IState == GuildInterfaceState.Recrutation)
        {
            foreach (var go in clickedGameObjects)
            {
                if (tt.Time < 0.1f)
                {
                    HeroClick(go, HeroDetailsUIShop);
                }
            }
        }
        else if (this.IState == GuildInterfaceState.Dungeons)
        {

        }
    }
    #endregion
    private void HeroClick(GameObject go, GameObject prefab = null)
    {
        if (prefab == null)
        {
            prefab = HeroDetailsUI;
        }
        if (go.CompareTag("UI#HERO"))
        {
            Hero hero = go.GetComponent<GuildHero>().Hero;
            this.OpenHeroDetails(hero, prefab);
        }
    }

    public void SellHero(Hero hero)
    {
        (GameObject.Find("BuildingInterface_Guild").GetComponent<GuildBuildingManager>().building as Guild).Heroes.Remove(hero);
        this.DestroyHeroGameobject(hero);
        Debug.Log("Sold this hero");
        SwitchTo(gameObject);
    }

}
public enum GuildInterfaceState
{
    main,
    heroDetails,
    Recrutation,
    Dungeons
}

/// <summary>
/// Interface implementing basic input handlers
/// </summary>
interface IInput
{
    void TouchBegan(Touch touch);
    void TouchMoved(Touch touch);
    void TouchEnded(Touch touch);
}

struct TouchTime
{
    public byte FingerID;
    public float Time;

    public TouchTime(byte id, float time = 0)
    {
        this.FingerID = id;
        this.Time = time;
    }
    public static TouchTime operator +(TouchTime x, float time)
    {
        return new TouchTime(x.FingerID, x.Time + time);
    }
}

class TouchHandler
{
    public List<TouchTime> touchTimes = new List<TouchTime>();

    public TouchTime GetTouch(byte id, bool delete = true)
    {
        TouchTime tt = this.touchTimes.Find(x => x.FingerID == id);
        Debug.Log("Touch removed -" + tt.FingerID);
        touchTimes.Remove(tt);
        return tt;
    }

    public void TouchAddTime(float time)
    {
        for(var i = 0; i < this.touchTimes.Count; i++)
        {
            this.touchTimes[i] += time;
        }
    }

    public void AddTouch(Touch touch)
    {
        TouchTime tt = new TouchTime((byte)touch.fingerId);
        Debug.Log("<color=red>Touch added +</color>" + tt.FingerID);
        this.touchTimes.Add(tt);
    }
}

/// <summary>
/// Handles transition between ui's
/// </summary>
class UIHandler
{
    private GuildBuildingManager guildBuildingManager;
    private GameObject MainUI;
    private GameObject ActiveUI;//UI that is currently shown to user
    public UIHandler(GameObject mainUI, GuildBuildingManager gbm)
    {
        this.ActiveUI = mainUI;
        this.MainUI = mainUI;
        this.guildBuildingManager = gbm;
    }
    public void SwitchUITo(GameObject newUI)
    {
        DisableUI(ActiveUI);
        if (newUI == null)
        {
            this.guildBuildingManager.IState = GuildInterfaceState.main;
            return;
        }
        if (newUI == MainUI)
        {
            this.guildBuildingManager.IState = GuildInterfaceState.main;
        }
        else if (newUI.name == "UI_Guild_Recrutation")
        {
            this.guildBuildingManager.IState = GuildInterfaceState.Recrutation;
        }
        else if (newUI.name == "UI_Guild_Dungeons")
        {
            this.guildBuildingManager.IState = GuildInterfaceState.Dungeons;
        }
        else if (newUI.name == "UI_Guild_Hero_Details" || newUI.name == "UI_Guild_Hero_Details_Shop")
        {
            this.guildBuildingManager.IState = GuildInterfaceState.heroDetails;
        }
        newUI.SetActive(true);
        this.ActiveUI = newUI;
    }

    private void DisableUI(GameObject ui)
    {
        if (ui != MainUI)
        {
            ui.SetActive(false);
        }
    }
}