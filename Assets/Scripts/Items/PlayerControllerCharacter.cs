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
        if (clickDatas[clickID].Item != null)
        {
            ClickData fetchNew = ScanClick(touch, clickDatas[clickID]);
            if (fetchNew.ItemSlot != null && fetchNew.Item == null)
            {
                Debug.Log("Free slot");
                clickDatas[clickID].Item.transform.SetParent(fetchNew.ItemSlot.transform);
            }
            else//Taken slot
            {
                Debug.Log("Taken slot");
                clickDatas[clickID].Item.transform.SetParent(clickDatas[clickID].ItemSlot.transform);
            }
            clickDatas[clickID].Item.transform.localPosition = new Vector2(0, 0);
        }
        clickDatas.RemoveAt(clickID);
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
            if (rr.gameObject.CompareTag("ItemSlot"))
            {
                if (heldData != null && heldData.Value.ItemSlot != rr.gameObject)
                {
                    cd.ItemSlot = rr.gameObject;
                }
                else if (heldData == null)
                {
                    cd.ItemSlot = rr.gameObject;
                }
            }
            else if (rr.gameObject.CompareTag("Item"))
            {
                if (heldData != null && heldData.Value.Item != rr.gameObject)
                {
                    cd.Item = rr.gameObject;
                }
                else if (heldData == null)
                {
                    cd.Item = rr.gameObject;
                }
            }
        }
        if(cd.Item != null)
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
        move,
    }
}
