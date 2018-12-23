using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Increase gold per click|Obsolete?
public class IncreaseTapPower : MonoBehaviour, IUpgrade
{
    private string _name;
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

    private void Awake()
    {
        this.Name = transform.name;
    }

    public void Interact(Building building)
    {
        building.OnUpgrade();
    }
}
