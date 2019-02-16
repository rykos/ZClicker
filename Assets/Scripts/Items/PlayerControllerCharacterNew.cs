using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControllerCharacterNew : MonoBehaviour, IInput
{
    List<ActiveTouch> activeTouches = new List<ActiveTouch>();
    public GameObject ItemDescriptionPrefab;
    private GameObject activeDescription;
    #region InputHandlers
    public void TouchBegan(Touch touch)
    {
        DestroyDescription();
        var rr = PullRayCastResults(touch);
        ActiveTouch newTouch = new ActiveTouch((byte)touch.fingerId, PullItem(rr), PullSlot(rr));
        activeTouches.Add(newTouch);
        if (newTouch.ClickedItem != null)
        {
            PickItemUP(newTouch.ClickedItem);
        }
    }

    public void TouchEnded(Touch touch)
    {
        ActiveTouch oldTouch = GetActiveTouch(touch.fingerId);
        activeTouches.Remove(oldTouch);
        if (oldTouch.ClickedItem == null)
        {
            return;
        }//Abort, no item
        var rr = PullRayCastResults(touch);
        ActiveTouch newTouch = new ActiveTouch((byte)touch.fingerId,
            PullItem(rr, oldTouch.ClickedItem), PullSlot(rr));
        //Check event
        if (newTouch.ClickedSlot == oldTouch.ClickedSlot)//Click
        {
            RestoreItemLocation(oldTouch);
            OpenItemDescription(oldTouch);
        }
        else if (newTouch.ClickedItem == null && newTouch.ClickedSlot != null && CompatibilityTest(oldTouch.ClickedItem, newTouch.ClickedSlot))//Move item to new slot
        {
            MoveItem(oldTouch.ClickedItem, newTouch.ClickedSlot);
        }
        else if (newTouch.ClickedSlot != null && newTouch.ClickedItem != null)//Rollback
        {
            RestoreItemLocation(oldTouch);
        }
        else
        {
            RestoreItemLocation(oldTouch);
        }
    }

    public void TouchMoved(Touch touch)
    {
        ActiveTouch oldTouch = GetActiveTouch(touch.fingerId);
        if (oldTouch.ClickedItem != null)
        {
            //Vector2 newPosition = Camera.main.ScreenToWorldPoint(touch.position);
            oldTouch.ClickedItem.transform.position = touch.position;
        }
    }
    #endregion

    private void OpenItemDescription(ActiveTouch touch)
    {
        DestroyDescription();
        activeDescription = Instantiate(ItemDescriptionPrefab, transform);
        activeDescription.transform.position = touch.ClickedSlot.transform.position;
        activeDescription.GetComponentInChildren<TMPro.TextMeshProUGUI>()
            .text = string.Format(touch.ClickedItem.GetComponent<ItemController>().item.Description, "Sword").Replace("\\n", "\n");
    }

    private void DestroyDescription()
    {
        if (activeDescription != null)
        {
            Destroy(activeDescription);
            activeDescription = null;
        }
    }

    //Moves item in hierarchy to fit in new slot
    private void MoveItem(GameObject item, GameObject slot)
    {
        item.transform.SetParent(slot.transform);
        item.transform.localPosition = Vector3.zero;
    }
    
    //Restore item position in hierarchy and world to its primary form
    private void RestoreItemLocation(ActiveTouch itemTouch)
    {
        MoveItem(itemTouch.ClickedItem, itemTouch.ClickedSlot);
    }

    private bool CompatibilityTest(GameObject item, GameObject slot)
    {
        ItemType itemType = GetHoldType(item);
        ItemType slotType = GetHoldType(slot);
        if (itemType == slotType)
        {
            return true;
        }
        else if (slotType == ItemType.Everything)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private ItemType GetHoldType(GameObject target)
    {
        if (target.CompareTag("Item"))
        {
            return target.GetComponent<ItemController>().item.itemType;
        }
        else if (target.CompareTag("ItemSlot")) 
        {
            return target.GetComponent<ItemSlotManager>().HeldItemType;
        }
        return ItemType.Everything;
    }

    //Moves item higher in hierarchy
    private void PickItemUP(GameObject item)
    {
        item.transform.SetParent(transform);
    }

    private ActiveTouch GetActiveTouch(int fingerID)
    {
        ActiveTouch touch = activeTouches.Find(x => x.FingerID == (byte)fingerID);
        return touch;
    }

    private GameObject PullItem(List<RaycastResult> raycastResults, GameObject ignoredItem = null)
    {
        foreach (var result in raycastResults)
        {
            if (result.gameObject.CompareTag("Item"))
            {
                if (result.gameObject != ignoredItem)//Found
                {
                    return result.gameObject;
                }
            }
        }
        return null;
    }

    private GameObject PullSlot(List<RaycastResult> raycastResults, GameObject ignoredSlot = null)
    {
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("ItemSlot"))
            {
                if (result.gameObject != ignoredSlot)
                {
                    return result.gameObject.transform.parent.gameObject;
                }
            }
        }
        return null;
    }

    private List<RaycastResult> PullRayCastResults(Touch touch)
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results;
    }

    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                TouchBegan(touch);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                TouchMoved(touch);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                TouchEnded(touch);
            }
        }
    }
}

public struct ActiveTouch
{
    public byte FingerID;
    public GameObject ClickedItem;
    public GameObject ClickedSlot;

    public ActiveTouch(byte fingerId, GameObject clickedItem, GameObject clickedSlot)
    {
        this.FingerID = fingerId;
        this.ClickedItem = clickedItem;
        this.ClickedSlot = clickedSlot;
    }
}