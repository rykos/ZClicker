using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private Vector2 targetPosition;
    private Vector3 offset = new Vector3(0, 1, -10);

    private void Update()
    {
        if (targetPosition != null)
        {
            transform.position = new Vector3(Vector2.Lerp(this.transform.position, targetPosition, Time.deltaTime * moveSpeed).x, 1, -10);
        }
    }
    public void MoveTo(Vector2 position)
    {
        targetPosition = position;
    }
}
