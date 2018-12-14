using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Increase gold per click
public class IncreaseTapPower : MonoBehaviour, IUpgrade
{
    private string _name;
    private int _cost;
    private float _time;
    public string Name
    {
        get
        {
            return this._name;
        }
        set
        {
            this._name = value;
        }
    }
    public int Cost
    {
        get
        {
            return this._cost;
        }

        set
        {
            this._cost = value;
        }
    }
    public float Time
    {
        get
        {
            return this._time;
        }

        set
        {
            this._time = value;
        }
    }

    private void Awake()
    {
        _name = transform.name;
    }
}
