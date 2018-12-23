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
        transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + move_destination, death_time);
    }
}
