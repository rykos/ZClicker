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
    #endregion
    public GuildInterfaceState IState
    {
        get { return this.istate; }
        set
        {
            this.istate = value;
        }
    }
    private GuildInterfaceState istate = GuildInterfaceState.main;
    private bool touchSensitive = true;
    private float touchFrozenTime = 0f;

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
    }

    //Open hero details page, and fills it with hero details
    private void OpenHeroDetails(Hero hero)
    {
        HeroDetailsUI.SetActive(true);
        HeroDetailsUI.GetComponent<GuildHeroDetailsManager>().FillDetails(hero);
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
            gameObjects.Add(res.gameObject);
        }
        return gameObjects;
    }

    public void Button_Dungeons()
    {
        Debug.Log("Dungeons");
        DungeonsUI.SetActive(true);
    }

    public void Button_Recrutation()
    {
        Debug.Log("Recrutation");
        RecrutationUI.SetActive(true);
    }

    public void FreezeTouch(float time = 0.1f)
    {
        touchSensitive = false;
    }

    public void TouchBegan(Touch touch)
    {
        Debug.Log(touch.fingerId + " began");
    }

    public void TouchMoved(Touch touch)
    {
        Debug.Log(touch.fingerId + " moved");
    }

    public void TouchEnded(Touch touch)
    {
        List<GameObject> clickedGameObjects = FetchGameObjects(touch.position);
        if (this.IState == GuildInterfaceState.main)
        {
            foreach (var go in clickedGameObjects)
            {
                if (go.CompareTag("UI#HERO"))
                {
                    Hero hero = go.GetComponent<GuildHero>().hero;
                    this.OpenHeroDetails(hero);
                    break;
                }
            }
        }
        else if(this.IState == GuildInterfaceState.heroDetails)
        {

        }
        else if (this.IState == GuildInterfaceState.Recrutation)
        {

        }
        else if (this.IState == GuildInterfaceState.Dungeons)
        {

        }
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