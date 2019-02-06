using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

/// <summary>
/// Item Controller Handler UI
/// </summary>
public class PlayerControllerCharacter : MonoBehaviour
{
    public int clicks;
    private List<ClickData> clickDatas = new List<ClickData>();

    private void Update()
    {
        clicks = clickDatas.Count;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                TouchBegan(touch);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                TouchEnded(touch);
            }
            else
            {
                TouchActive(touch);
            }
        }
    }

    private void OnDisable()
    {
        clicks = 0;
        clickDatas = new List<ClickData>();
    }

    #region touch handlers
    private void TouchBegan(Touch touch)
    {
        ClickData cd = ScanClick(touch);
        if (cd.Item != null)
        {
            cd.Item.transform.SetParent(transform.Find("/CharacterInterface"));
        }
        clickDatas.Add(cd);
    }
    private void TouchActive(Touch touch)
    {
        int clickID = clickDatas.IndexOf(clickDatas.Find(c => c.FingerID == touch.fingerId));
        if (clickDatas[clickID].Item != null)
        {
            GameObject dragged = clickDatas[clickID].Item;
            Vector2 new_pos = Camera.main.ScreenToWorldPoint(touch.position);
            dragged.transform.position = new_pos;
        }
    }
    private void TouchEnded(Touch touch)
    {
        int clickID = clickDatas.IndexOf(clickDatas.Find(c => c.FingerID == touch.fingerId));
        if (clickDatas[clickID].Item != null)//Item clicked
        {
            GameObject Item = clickDatas[clickID].Item;
            ClickData fetchNew = ScanClick(touch, clickDatas[clickID]);
            if (fetchNew.ItemSlot != null && fetchNew.Item == null && ItemSlotTypeValid(Item, fetchNew.ItemSlot))//There a slot but not an item
            {
                //Free slot
                Item.transform.SetParent(fetchNew.ItemSlot.transform);
            }
            else if (fetchNew.Item != null && fetchNew.ItemSlot != null && fetchNew.action == ClickDataAction.showDescription)//There is a slot and item
            {
                Debug.Log("Should i Display Log Message");
            }
            else//Taken slot
            {
                Debug.Log("Taken slot");
                Item.transform.SetParent(clickDatas[clickID].ItemSlot.transform);
            }
            clickDatas[clickID].Item.transform.localPosition = new Vector2(0, 0);
        }
        clickDatas.RemoveAt(clickID);
    }

    //Checks if held item and item slot are the valid types
    public bool ItemSlotTypeValid(GameObject heldItem, GameObject ItemSlot)
    {
        ItemType heldType = heldItem.GetComponent<ItemController>().item.itemType;
        ItemType slotType = ItemSlot.transform.GetComponent<ItemSlotManager>().HeldItemType;
        if (heldType == slotType || slotType == ItemType.Everything)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    private ClickData ScanClick(Touch touch, ClickData? heldData = null)
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        ClickData cd = new ClickData() { FingerID = touch.fingerId };
        foreach (RaycastResult rr in results)
        {
            if (rr.gameObject.CompareTag("ItemSlot"))//Over slot
            {
                if (heldData != null && heldData.Value.ItemSlot != rr.gameObject)//Held something & item held primary slot != this slot
                {
                    cd.ItemSlot = rr.gameObject.transform.parent.gameObject;//Data slot = this
                }
                else if (heldData != null && heldData.Value.ItemSlot == rr.gameObject)//Held item & held slot == this slot
                {
                    cd.action = ClickDataAction.showDescription;
                    cd.ItemSlot = rr.gameObject.transform.parent.gameObject;
                }
                else if (heldData == null)//Nothing was held
                {
                    cd.ItemSlot = rr.gameObject.transform.parent.gameObject;//Data slot = this slot
                }
            }
            else if (rr.gameObject.CompareTag("Item"))//Over item
            {
                if (heldData != null && heldData.Value.Item != rr.gameObject)//Held something & held item != this item
                {
                    cd.Item = rr.gameObject;//Data item = this item
                }
                else if (heldData != null && heldData.Value.Item == rr.gameObject)//Drag on itself
                {
                    cd.action = ClickDataAction.showDescription;
                    cd.Item = rr.gameObject;
                }
                else if (heldData == null)//Nothing was held
                {
                    cd.Item = rr.gameObject;//Data item = this item
                }
            }
        }
        if(cd.Item != null && cd.action == default)//Item pointed
        {
            cd.action = ClickDataAction.move;
        }
        return cd;
    }

    struct ClickData
    {
        public int FingerID;
        public GameObject Item;
        public GameObject ItemSlot;
        public ClickDataAction action;
        public ClickData(int fingerID, GameObject item, GameObject itemslot, ClickDataAction cda)
        {
            this.FingerID = fingerID;
            this.Item = item;
            this.ItemSlot = itemslot;
            this.action = cda;
        }
    }
    enum ClickDataAction
    {
        none,
        move,
        showDescription
    }
}
