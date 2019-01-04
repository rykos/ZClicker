using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossModelManager : MonoBehaviour
{
    public float horizontalMovement;
    public float verticalMovement;
    public float speed;
    private Vector2 startingPos;
    private Vector2 moveMagnitude;
    private Vector2 pos1, pos2;

    private void Start()
    {
        startingPos = transform.localPosition;
        pos1 = startingPos + new Vector2(horizontalMovement, verticalMovement);
        pos2 = startingPos - new Vector2(horizontalMovement, verticalMovement);
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(pos1, pos2, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
    }
}
