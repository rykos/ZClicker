using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Vector2 clickStarted;
    
    private void Update()
    {
        Swipe();
    }

    private void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickStarted = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && clickStarted != Vector2.zero)
        {
            Vector2 direction = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - clickStarted;
            clickStarted = Vector2.zero;
            float degrees = SignedDegToDeg(-Vector2.SignedAngle(direction, Vector2.right));
            DegToDirection(degrees);
        }
    }

    private float SignedDegToDeg(float signedDeg)
    {
        if (signedDeg < 0)
        {
            signedDeg = 180 - (180 + signedDeg);
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
}
