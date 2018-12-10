using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class PlayerControl : MonoBehaviour
{
    private List<Click> activeClicks = new List<Click>();
    
    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                Click click = new Click(touch.fingerId, touch.position);
                activeClicks.Add(click);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                int index = activeClicks.FindIndex(click => click.fingerId == touch.fingerId);
                ConsiderClick(activeClicks[index], touch.position);
                activeClicks.Remove(activeClicks[index]);
            }
            else
            {
                int index = activeClicks.FindIndex(click => click.fingerId == touch.fingerId);
                activeClicks[index] = activeClicks[index].AddTime(Time.deltaTime);
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
        Vector2 activePosition = Camera.main.ScreenToWorldPoint(newPosition);
        Vector2 directionVector = activePosition - (Vector2)Camera.main.ScreenToWorldPoint(click.startVector);
        float magnitude = Mathf.Sqrt(Mathf.Pow(directionVector.x, 2) + Mathf.Pow(directionVector.y, 2));
        float direction = SignedDegToDeg(-Vector2.SignedAngle(directionVector, Vector2.right));

        if (click.time < 0.07f || magnitude < 0.2f)//Tap
        {

        }
        else if (magnitude > 2f)//Swipe
        {
            Debug.Log("Swipe detected " + magnitude);
            if (direction < 90 || direction > 270)//right
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().
                    MoveTo(new Vector2(GameObject.Find("Map").GetComponent<MapManager>().
                    NextBuilding(Vector2.left).PositionX, 0));
            }
            else//left
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().
                    MoveTo(new Vector2(GameObject.Find("Map").GetComponent<MapManager>().
                    NextBuilding(Vector2.right).PositionX, 0));
            }
        }
    }
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