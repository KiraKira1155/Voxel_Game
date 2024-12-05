using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToolBar
{
    private GameObject[] eachSlot;
    private Slot slot;
    private int slotIndex;

    private GameObject highlight;

    public void DoAwake(Slot slot)
    {
        this.slot = slot;
        eachSlot = slot.eachSlot;
        int i = 0;
        foreach (GameObject s in eachSlot)
        {
            s.transform.GetChild(0).GetComponent<ItemBlockUI>().DoAwake();
            s.transform.GetChild(1).GetComponent<ItemUI>().DoAwake();
            i++;
        }
    }

    public void DoStart()
    {
        highlight = slot.highlight;
    }

    public void DoUpdate()
    {
        int scroll = (int)Input.mouseScrollDelta.y;

        if (scroll != 0)
        {
            if (scroll < 0)
                slotIndex++;
            else
                slotIndex--;

            if (slotIndex > eachSlot.Length -1)
                slotIndex = 0;
            if (slotIndex < 0)
                slotIndex = eachSlot.Length - 1;

            highlight.transform.position = eachSlot[slotIndex].transform.position;
        }
    }

    public int CheckSlotIndex()
    {
        return slotIndex;
    }

    /// <summary>
    /// 現在選択中のスロットがアイテムを持っているかの確認
    /// </summary>
    /// <returns></returns>
    public bool CheckHaveItem()
    {
        if (slot.toolbarSlots[slotIndex].itemSlot.stack != null)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 現在選択中のスロットのアイテムタイプを確認
    /// </summary>
    /// <returns></returns>
    public EnumGameData.ItemType CheckHaveItemType()
    {
        if (CheckHaveItem())
            return slot.toolbarSlots[slotIndex].itemSlot.stack.itemType;
        else
            return EnumGameData.ItemType.Hand;
    }

    /// <summary>
    /// 現在選択中のスロットのアイテム種を確認
    /// </summary>
    /// <returns></returns>
    public EnumGameData.ItemKinds CheckHaveItemKinds()
    {
        if (CheckHaveItem())
            return slot.toolbarSlots[slotIndex].itemSlot.stack.kinds;
        else
            return EnumGameData.ItemKinds.Hand;
    }

    /// <summary>
    /// 現在選択中のスロットのアイテムIDを確認
    /// </summary>
    /// <returns></returns>
    public EnumGameData.ItemID CheckHaveItemID()
    {
        if (CheckHaveItem())
            return slot.toolbarSlots[slotIndex].itemSlot.stack.itemID;
        else
            return EnumGameData.ItemID.None;
    }

    /// <summary>
    /// 現在選択中のスロットのブロックIDを確認
    /// </summary>
    /// <returns></returns>
    public EnumGameData.BlockID CheckHaveBlockID()
    {
        if (CheckHaveItem())
            return slot.toolbarSlots[slotIndex].itemSlot.stack.blockID;
        else
            return EnumGameData.BlockID.air;
    }

    /// <summary>
    /// 現在選択中スロットのアイテムの耐久値を使用
    /// </summary>
    /// <param name="decreaseDurability"></param>
    public void UseHaveTool(int decreaseDurability)
    {
        if (CheckHaveItem())
            if (slot.toolbarSlots[slotIndex].itemSlot.stack.durability > -1)
                slot.toolbarSlots[slotIndex].itemSlot.UseTool(decreaseDurability);
    }

    /// <summary>
    /// 現在選択中スロットのアイテムを使用
    /// </summary>
    /// <param name="amount"></param>
    public void UseHaveItem(int amount)
    {
        if (CheckHaveItem())
            slot.toolbarSlots[slotIndex].itemSlot.Take(amount);
    }
}
