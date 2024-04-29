using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDropHandlar : MonoBehaviour
{
    [SerializeField] private UIItemSlot cursorSlot = null;
    private ItemSlot cursorItemSlot;

    [SerializeField] private GraphicRaycaster raycaster = null;
    private PointerEventData pointerEventData;
    [SerializeField] private EventSystem eventSystem = null;
    [SerializeField] private Camera UICamera;

    private UIItemSlot lastClickSlot;
    private UIItemSlot currentClickSlot;

    private bool gather;
    private float time;
    public void DoAwake()
    {
        cursorSlot.transform.GetChild(0).GetComponent<ItemBlockUI>().DoAwake();
        cursorSlot.transform.GetChild(1).GetComponent<ItemUI>().DoAwake();
    }

    public void DoStart()
    {
        cursorItemSlot = new ItemSlot(cursorSlot);
    }

    public void DoUpdate()
    {
        Vector3 cursorPos = Input.mousePosition;
        Vector3 screenPos = new Vector3(cursorPos.x, cursorPos.y - 20f, 1f);
        cursorSlot.transform.position = UICamera.ScreenToWorldPoint(screenPos);

        if (KeyConfig.GetKeyDown(KeyConfig.KeyName.RightClick))
        {
            if (CheckForSlot() != null)
                SlotRightClick(CheckForSlot());
        }
        else if(KeyConfig.GetKeyDown(KeyConfig.KeyName.LeftClick))
        {
            if(CheckForSlot() != null)
                SlotLeftClick(CheckForSlot());
        }

        if (gather)
        {
            Gather();
        }
    }

    public void ChangeToInactive()
    {
        gather = false;
        if (cursorItemSlot.HasItem && !currentClickSlot.HasItem)
        {
            switch (cursorItemSlot.stack.kinds)
            {
                case EnumGameData.ItemKinds.blockItem:
                    currentClickSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll(cursorItemSlot.stack.kinds));
                    break;

                case EnumGameData.ItemKinds.item:
                    currentClickSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll(cursorItemSlot.stack.kinds, cursorItemSlot.stack.itemType));
                    break;
            }
            cursorItemSlot.EmptySlot();
        }
    }

    private bool Gather()
    {
        if (time > 1f)
        {
            gather = false;
            return false;
        }
        else
        {
            time += Time.deltaTime;
            return true;
        }
    }

    private void SlotLeftClick(UIItemSlot clickedSlot)
    {
        currentClickSlot = clickedSlot;

        //クリエイティブ時のインベントリ―操作
        if (clickedSlot.itemSlot.isCreative)
        {
            cursorSlot.itemSlot.EmptySlot();

            lastClickSlot = clickedSlot;
            return;
        }

        if (cursorSlot.HasItem && clickedSlot.HasItem)
        {
            //同じアイテムなら
            if (cursorSlot.itemSlot.stack.blockID == clickedSlot.itemSlot.stack.blockID && 
                (cursorSlot.itemSlot.stack.itemID == clickedSlot.itemSlot.stack.itemID && cursorSlot.itemSlot.stack.itemType == clickedSlot.itemSlot.stack.itemType))
            {
                switch (clickedSlot.itemSlot.stack.kinds)
                {
                    case EnumGameData.ItemKinds.blockItem:
                        if (BlockManager.I.MaxAmount(clickedSlot.itemSlot.stack.blockID) == clickedSlot.itemSlot.stack.amount)
                            return;
                        break;

                    case EnumGameData.ItemKinds.item:
                        if (ItemManager.I.MaxAmount(clickedSlot.itemSlot.stack.itemType, clickedSlot.itemSlot.stack.itemID) == clickedSlot.itemSlot.stack.amount)
                            return;
                        break;
                }
                if (KeyConfig.GetKey(KeyConfig.KeyName.Squat))
                    clickedSlot.itemSlot.GetStack(cursorItemSlot.AddItem(cursorItemSlot.stack.amount / 2), cursorSlot);
                else
                    clickedSlot.itemSlot.GetStack(cursorItemSlot.AddItem(1), cursorSlot);

                lastClickSlot = clickedSlot;
                return;
            }
        }

        //空のスロットなら
        if (cursorSlot.HasItem && !clickedSlot.HasItem)
        {
            if (KeyConfig.GetKey(KeyConfig.KeyName.Squat))
                switch (cursorItemSlot.stack.kinds)
                {
                    case EnumGameData.ItemKinds.blockItem:
                        clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeItem(cursorItemSlot.stack.kinds, cursorItemSlot.stack.amount / 2));
                        break;

                    case EnumGameData.ItemKinds.item:
                        clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeItem(cursorItemSlot.stack.kinds, cursorItemSlot.stack.itemType, cursorItemSlot.stack.amount / 2, cursorItemSlot.stack.durability));
                        break;
                }
            else
                switch (cursorItemSlot.stack.kinds)
                {
                    case EnumGameData.ItemKinds.blockItem:
                        clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeItem(cursorItemSlot.stack.kinds, 1));
                        break;

                    case EnumGameData.ItemKinds.item:
                        clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeItem(cursorItemSlot.stack.kinds, cursorItemSlot.stack.itemType, 1, cursorItemSlot.stack.durability));
                        break;
                }

            lastClickSlot = clickedSlot;
            return;
        }
    }

    private void SlotRightClick(UIItemSlot clickedSlot)
    {
        currentClickSlot = clickedSlot;

        if (clickedSlot == null)
            return;

        //所持なし
        if (!cursorSlot.HasItem && !clickedSlot.HasItem) 
            return;

        //クリエイティブ時のインベントリ―操作
        if (clickedSlot.itemSlot.isCreative)
        {
            cursorItemSlot.EmptySlot();
            cursorItemSlot.InsertStack(clickedSlot.itemSlot.stack);
        }

        //アイテム選択
        if(!cursorSlot.HasItem && clickedSlot.HasItem)
        {
            switch (clickedSlot.itemSlot.stack.kinds)
            {
                case EnumGameData.ItemKinds.blockItem:
                    cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAll(clickedSlot.itemSlot.stack.kinds));
                    break;

                case EnumGameData.ItemKinds.item:
                    cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAll(clickedSlot.itemSlot.stack.kinds, clickedSlot.itemSlot.stack.itemType));
                    break;
            }

            lastClickSlot = clickedSlot;
            return;
        }

        //空のスロットにアイテム設置
        if (cursorSlot.HasItem && !clickedSlot.HasItem)
        {
            switch (cursorItemSlot.stack.kinds)
            {
                case EnumGameData.ItemKinds.blockItem:
                    clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll(cursorItemSlot.stack.kinds));
                    break;

                case EnumGameData.ItemKinds.item:
                    clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll(cursorItemSlot.stack.kinds, cursorItemSlot.stack.itemType));
                    break;
            }

            //同アイテムを整理
            Collect(clickedSlot);

            lastClickSlot = clickedSlot;
            return;
        }

        if (cursorSlot.HasItem && clickedSlot.HasItem)
        {
            //スロットにあるアイテムと選択中のアイテムを交換
            if (cursorSlot.itemSlot.stack.blockID != clickedSlot.itemSlot.stack.blockID || 
                ((cursorSlot.itemSlot.stack.itemID != clickedSlot.itemSlot.stack.itemID && cursorSlot.itemSlot.stack.itemType == clickedSlot.itemSlot.stack.itemType) 
                || (cursorSlot.itemSlot.stack.itemID == clickedSlot.itemSlot.stack.itemID && cursorSlot.itemSlot.stack.itemType != clickedSlot.itemSlot.stack.itemType)
                || cursorSlot.itemSlot.stack.itemID != clickedSlot.itemSlot.stack.itemID && cursorSlot.itemSlot.stack.itemType != clickedSlot.itemSlot.stack.itemType))
            {
                ItemStack oldCursorSlot = null;
                ItemStack oldSlot = null;
                switch (cursorItemSlot.stack.kinds)
                {
                    case EnumGameData.ItemKinds.blockItem:
                        oldCursorSlot = cursorSlot.itemSlot.TakeAll(cursorItemSlot.stack.kinds);
                        break;

                    case EnumGameData.ItemKinds.item:
                        oldCursorSlot = cursorSlot.itemSlot.TakeAll(cursorItemSlot.stack.kinds, cursorItemSlot.stack.itemType);
                        break;
                }
                switch (clickedSlot.itemSlot.stack.kinds)
                {
                    case EnumGameData.ItemKinds.blockItem:
                        oldSlot = clickedSlot.itemSlot.TakeAll(clickedSlot.itemSlot.stack.kinds);
                        break;

                    case EnumGameData.ItemKinds.item:
                        oldSlot = clickedSlot.itemSlot.TakeAll(clickedSlot.itemSlot.stack.kinds, clickedSlot.itemSlot.stack.itemType);
                        break;
                }

                clickedSlot.itemSlot.InsertStack(oldCursorSlot);
                cursorSlot.itemSlot.InsertStack(oldSlot);
            }
            else if (cursorSlot.itemSlot.stack.blockID == clickedSlot.itemSlot.stack.blockID
                && cursorSlot.itemSlot.stack.itemID == clickedSlot.itemSlot.stack.itemID
                && cursorSlot.itemSlot.stack.itemType == clickedSlot.itemSlot.stack.itemType)
            {
                if (!clickedSlot.itemSlot.isCreative)
                {
                    clickedSlot.itemSlot.GetStack(cursorItemSlot.AddItem(cursorItemSlot.stack.amount), cursorSlot);
                }
                else
                {
                    switch (cursorItemSlot.stack.kinds)
                    {
                        case EnumGameData.ItemKinds.blockItem:
                            cursorItemSlot.stack.amount = BlockManager.I.MaxAmount(cursorItemSlot.stack.blockID);
                            break;

                        case EnumGameData.ItemKinds.item:
                            cursorItemSlot.stack.amount = ItemManager.I.MaxAmount(cursorItemSlot.stack.itemType, cursorItemSlot.stack.itemID);
                            break;
                    }
                    cursorSlot.UpdateAmount();
                }
            }

            lastClickSlot = clickedSlot;
            return;
        }
    }

    private void Collect(UIItemSlot clickedSlot)
    {
        if (lastClickSlot == currentClickSlot && Gather())
        {
            gather = false;
            for (int i = Inventory.I.toolBarSlots.Count - 1; i >= 0; i--)
            {
                switch (clickedSlot.itemSlot.stack.kinds)
                {
                    case EnumGameData.ItemKinds.blockItem:
                        if (Inventory.I.GetToolBarSlotsBlockItemID(i) == clickedSlot.itemSlot.stack.blockID && clickedSlot.itemSlot != Inventory.I.toolBarSlots[i])
                        {
                            clickedSlot.itemSlot.GetStack(Inventory.I.toolBarSlots[i].AddItem(Inventory.I.toolBarSlots[i].stack.amount), Inventory.I.toolBarSlots[i].slot);
                        }
                        break;

                    case EnumGameData.ItemKinds.item:
                        if (Inventory.I.GetToolBarSlotsItemID(i) == (clickedSlot.itemSlot.stack.itemType, clickedSlot.itemSlot.stack.itemID) && clickedSlot.itemSlot != Inventory.I.toolBarSlots[i])
                        {
                            clickedSlot.itemSlot.GetStack(Inventory.I.toolBarSlots[i].AddItem(Inventory.I.toolBarSlots[i].stack.amount), Inventory.I.toolBarSlots[i].slot);
                        }
                        break;
                }
            }
        }
        else
        {
            gather = true;
            time = 0;
        }
    }

    private UIItemSlot CheckForSlot()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if(result.gameObject.tag == "UIItemSlot")
                return result.gameObject.GetComponent<UIItemSlot>();
        }

        return null;
    }
}
