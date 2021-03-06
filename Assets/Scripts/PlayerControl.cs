﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class PlayerControl : MonoBehaviour, IInput
{
    private List<Click> activeClicks = new List<Click>();
    private MapManager mapManager;

    private void Awake()
    {
        try
        {
            this.mapManager = GameObject.Find("Map").GetComponent<MapManager>();
        }
        catch
        {
            Debug.Log("No mapManager");
        }
    }
    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                TouchBegan(touch);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                TouchEnded(touch);
            }
            else
            {
                TouchMoved(touch);
            }
        }
    }

    private float SignedDegToDeg(float signedDeg)
    {
        if (signedDeg < 0)
        {
            signedDeg = 180 + (180 + signedDeg);
        }
        return signedDeg;
    }

    private Vector2 DegToDirection(float deg)
    {
        Vector2 direction;
        if (deg <= 90 || deg > 270)// right
        {
            direction = Vector2.right;
        }
        else if (deg > 90 || deg <= 270)
        {
            direction = Vector2.left;
        }
        return Vector2.zero;
    }

    private void ConsiderClick(Click click, Vector2 newPosition)
    {
        if (IsPointerOverUIObject(newPosition))
        {
            UIClick(click, newPosition);
            return;
        }
        Vector2 activePosition = Camera.main.ScreenToWorldPoint(newPosition);
        Vector2 directionVector = activePosition - (Vector2)Camera.main.ScreenToWorldPoint(click.startVector);
        float magnitude = Mathf.Sqrt(Mathf.Pow(directionVector.x, 2) + Mathf.Pow(directionVector.y, 2));
        float direction = SignedDegToDeg(-Vector2.SignedAngle(directionVector, Vector2.right));

        if (click.time < 0.07f || magnitude < 0.2f)//Tap
        {
            Tap(newPosition);
        }
        else if (magnitude > 1.5f)//Swipe
        {
            Swipe(magnitude, direction);
        }
    }

    private void UIClick(Click click, Vector2 newPosition)
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = newPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        foreach (var obj in results)
        {
            if (obj.gameObject.CompareTag("BossInterface"))
            {
                BossInterface(click, newPosition);
                break;
            }
        }
    }

    private void Tap(Vector2 newPosition)
    {
        //Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(newPosition);
        Collider2D colliderTouched = Physics2D.OverlapPoint(touchPos);
        if (colliderTouched != null)
        {
            colliderTouched.transform.GetComponent<BuildingManager>().Interact(newPosition);
        }
    }

    private void Swipe(float magnitude, float direction)
    {
        Debug.Log("Swipe detected " + magnitude);
        if (direction < 90 || direction > 270)//right
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().
                MoveTo(new Vector2(GameObject.Find("Map").GetComponent<MapManager>().
                NextBuilding(Vector2.left).Position.X, 0));
        }
        else//left
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().
                MoveTo(new Vector2(GameObject.Find("Map").GetComponent<MapManager>().
                NextBuilding(Vector2.right).Position.X, 0));
        }
    }

    private bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(touchPos.x, touchPos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);//collect all ui objects
        foreach (RaycastResult res in results)
        {
            if (!res.gameObject.CompareTag("UI#IGNORE"))
            {
                return true;
            }
        }
        return false;
    }

    private Collider2D GetColliderAt(Vector2 position)
    {
        Collider2D colliderTouched = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(position));
        return colliderTouched;
    }

    #region Interfaces
    private void BossInterface(Click click, Vector2 touchPos)
    {
        Debug.Log("BossInterface clicked");
        Collider2D colliderTouched = GetColliderAt(touchPos);
        if (colliderTouched)
        {
            colliderTouched.transform.parent.GetComponent<BossManager>().HitBoss(touchPos);
        }
    }

    private void PullItemsSlots(Click click, Vector2 touchPos)
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = touchPos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        foreach (RaycastResult result in results)
        {
            Debug.Log(result.gameObject.tag);
        }
    }

    #region touch handlers

    public void TouchBegan(Touch touch)
    {
        Click click = new Click(touch.fingerId, touch.position);
        activeClicks.Add(click);
    }

    public void TouchMoved(Touch touch)
    {
        int index = activeClicks.FindIndex(click => click.fingerId == touch.fingerId);
        activeClicks[index] = activeClicks[index].AddTime(Time.deltaTime);
    }

    public void TouchEnded(Touch touch)
    {
        int index = activeClicks.FindIndex(click => click.fingerId == touch.fingerId);
        ConsiderClick(activeClicks[index], touch.position);
        activeClicks.Remove(activeClicks[index]);
    }
    #endregion

    #endregion
}

struct Click
{
    public int fingerId;
    public Vector2 startVector;
    public float time;

    public Click(int fingerId, Vector2 startVector, float time = 0)
    {
        this.time = time;
        this.fingerId = fingerId;
        this.startVector = startVector;
    }
    
    public Click AddTime(float amount)
    {
        return new Click(fingerId, startVector, time + amount);
    }
}