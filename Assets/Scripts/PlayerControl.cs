using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerControl : MonoBehaviour
{
    private Vector2 clickStarted = Vector2.zero;
    private float timePressed = 0;
    
    private void Update()
    {
        UserClick();
    }
    
    private void UserClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickStarted = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && clickStarted != Vector2.zero)
        {
            Vector2 direction = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - clickStarted;
            float directionDegrees = SignedDegToDeg(-Vector2.SignedAngle(direction, Vector2.right));
            float magnitude = Mathf.Sqrt(Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2));
            Click click = new Click(directionDegrees, magnitude, timePressed);
            timePressed = 0;
            clickStarted = Vector2.zero;
            ConsiderClick(click);
        }
        if (clickStarted != Vector2.zero)
        {
            timePressed += Time.deltaTime;
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

    private void ConsiderClick(Click click)
    {
        if (click.time < 0.05f || click.magnitude < 0.2f)//Tap
        {

        }
        else if (click.magnitude > 0.5f)//Swipe
        {
            if (click.direction < 90 || click.direction > 270)//right
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
    public float direction; //degree
    public float magnitude;
    public float time;

    public Click(float direction, float magnitude, float time)
    {
        this.direction = direction;
        this.magnitude = magnitude;
        this.time = time;
    }
}