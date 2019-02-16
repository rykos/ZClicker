using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles input handlers switching
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField]
    private PlayerControllerCharacter playerControllerCharacter;
    [SerializeField]
    private PlayerControl playerControl;
    //
    private MonoBehaviour activeInputHandler;
    public MonoBehaviour ActiveInputHandler
    {
        get
        {
            return this.activeInputHandler;
        }
        set
        {
            this.activeInputHandler = value;
        }
    }

    private void Awake()
    {
        activeInputHandler = (GetComponent<IInput>() as MonoBehaviour);
    }
    public void ChangePlayerInput(GameObject selectedInterface)
    {
        Debug.Log("Switch player controller to " + selectedInterface.name);
        if (activeInputHandler.gameObject.tag == selectedInterface.tag)
        {
            return;
        }
        else if (selectedInterface.activeSelf == false)
        {
            SetStandardInput();
            return;
        }
        activeInputHandler.enabled = false;
        switch (selectedInterface.tag)
        {
            case "VillageInterface":
                playerControl.enabled = true;
                activeInputHandler = playerControl;
                break;
            case "BossInterface":
                Debug.Log("BI detected");
                playerControl.enabled = true;
                activeInputHandler = playerControl;
                break;
            default:
                Debug.Log("Def state");
                break;
        }
    }

    public void SetStandardInput()
    {
        Debug.Log("Forcing standard one");
        activeInputHandler.enabled = false;
        playerControl.enabled = true;
        activeInputHandler = playerControl;
    }
}
