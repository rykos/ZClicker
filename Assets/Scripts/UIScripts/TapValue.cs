using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TapValue : MonoBehaviour
{
    public float death_time;
    public Vector2 move_destination;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        Destroy(transform.gameObject, death_time);
    }
    private void Update()
    {
        text.color -= new Color32(0,0,0,15);
        Vector3 offset = new Vector3(0,0,-8);
        transform.position = Vector3.Lerp(transform.position,
            transform.position + (Vector3)move_destination,
            death_time);
    }
}
