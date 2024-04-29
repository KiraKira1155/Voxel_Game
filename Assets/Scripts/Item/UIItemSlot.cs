using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    public bool isLinked = false;
    public ItemSlot itemSlot;
    public Image slotImage;
    public TextMeshPro slotAmount;
    [SerializeField] private MeshRenderer blockItem;
    [SerializeField] private MeshRenderer item;

    public bool HasItem
    {
        get
        {
            if (itemSlot == null)
                return false;
            else
                return itemSlot.HasItem;
        }
    }

    public void Link(ItemSlot itemSlot)
    {
        this.itemSlot = itemSlot;
        isLinked = true;
        this.itemSlot.LinkUISlot(this);
        UpdateSlot();
    }

    public void UnLink()
    {
        itemSlot.unLinkUISlot();
        itemSlot = null;
        UpdateSlot();
    } 

    public void UpdateSlot()
    {
        if(itemSlot != null && itemSlot.HasItem)
        {
            if (itemSlot.stack.kinds == EnumGameData.ItemKinds.blockItem)
            {
                blockItem.gameObject.GetComponent<ItemBlockUI>().UpdateUI(itemSlot.stack.blockID);
                blockItem.enabled = true;
                item.enabled = false;
            }
            else if(itemSlot.stack.kinds == EnumGameData.ItemKinds.item)
            {
                item.gameObject.GetComponent<ItemUI>().UpdateUI(itemSlot.stack.itemType, itemSlot.stack.itemID);
                item.enabled = true;
                blockItem.enabled = false;
            }
            UpdateAmount();
            slotAmount.enabled = true;
        }
        else
        {
            Clear();
        }
    }

    public void UpdateAmount()
    {
        if (ItemManager.I.MaxDurability(itemSlot.stack.itemType, itemSlot.stack.itemID) > -1)
        {
            slotAmount.text = itemSlot.stack.durability.ToString();
            slotAmount.color = Color.red;
        }
        else
        {
            slotAmount.text = itemSlot.stack.amount.ToString();
            slotAmount.color = Color.white;
        }
    }

    public void Clear()
    {
        slotAmount.text = "";
        blockItem.enabled = false;
        item.enabled = false;
        slotAmount.enabled = false;
    }

    private void OnDestroy()
    {
        if (isLinked)
        {
            itemSlot.unLinkUISlot();
        }
    }
}

public class ItemSlot
{
    public ItemStack stack = null;
    public UIItemSlot slot= null;

    public bool isCreative;

    public ItemSlot(UIItemSlot uiItemSlot)
    {
        stack = null;
        slot = uiItemSlot;
        slot.Link(this);
    }
    public ItemSlot(UIItemSlot uiItemSlot, ItemStack stack)
    {
        this.stack = stack;
        slot = uiItemSlot;
        slot.Link(this);
    }

    public void LinkUISlot(UIItemSlot uiItemSlot)
    {
        slot = uiItemSlot; 
    }

    public void unLinkUISlot()
    {
        slot = null;
    }

    public void EmptySlot()
    {
        stack = null;
        if(slot != null)
            slot.UpdateSlot();
    }

    public int Take(int amt)
    {
        if(amt > stack.amount)
        {
            int _amt = stack.amount;
            EmptySlot();
            return _amt;
        }
        else if(amt < stack.amount)
        {
            stack.amount -= amt;
            slot.UpdateSlot();
            return amt;
        }
        else
        {
            EmptySlot();
            return amt;
        }
    }

    public ItemStack TakeAll(EnumGameData.ItemKinds kinds)
    {
        ItemStack handOver = new ItemStack(kinds, stack.blockID, stack.amount);
        EmptySlot();
        return handOver;
    }
    public ItemStack TakeAll(EnumGameData.ItemKinds kinds, EnumGameData.ItemType itemType)
    {
        ItemStack handOver = new ItemStack(kinds, itemType, stack.itemID, stack.amount, stack.durability);
        EmptySlot();
        return handOver;
    }

    public void InsertStack(ItemStack stack)
    {
        this.stack = stack;
        slot.UpdateSlot();
    }

    public ItemStack TakeItem(EnumGameData.ItemKinds kinds, int getAmount)
    {
        ItemStack handOver = new ItemStack(kinds, stack.blockID, getAmount);
        if(getAmount == 0)
        {
            return null;
        }
        stack.amount -= getAmount;
        if (stack.amount == 0)
        {
            EmptySlot();
        }
        else
        {
            slot.UpdateAmount();
        }
        return handOver;
    }
    public ItemStack TakeItem(EnumGameData.ItemKinds kinds, EnumGameData.ItemType itemType, int getAmount, int durability)
    {
        ItemStack handOver = new ItemStack(kinds, itemType, stack.itemID, getAmount, durability);
        if (getAmount == 0)
        {
            return null;
        }
        stack.amount -= getAmount;
        if (stack.amount == 0)
        {
            EmptySlot();
        }
        else
        {
            slot.UpdateAmount();
        }
        return handOver;
    }

    public int AddItem(int getAmount)
    {
        stack.amount -= getAmount;
        if (stack.amount == 0)
        {
            EmptySlot();
        }
        else
        {
            slot.UpdateAmount();
        }

        return getAmount;
    }

    public void GetItem(int getAmount)
    {
        stack.amount += getAmount;
        slot.UpdateAmount();
    }

    public void GetStack(int getAmount, UIItemSlot targetSlot)
    {
        stack.amount += getAmount;
        switch (stack.kinds)
        {
            case EnumGameData.ItemKinds.blockItem:
                if (stack.amount > BlockManager.I.MaxAmount(stack.blockID))
                {
                    ItemStack handOver = null;
                    switch (stack.kinds)
                    {
                        case EnumGameData.ItemKinds.blockItem:
                            handOver = new ItemStack(stack.kinds, stack.blockID, stack.amount - BlockManager.I.MaxAmount(stack.blockID));
                            break;

                        case EnumGameData.ItemKinds.item:
                            handOver = new ItemStack(stack.kinds, stack.itemType, stack.itemID, stack.amount - BlockManager.I.MaxAmount(stack.blockID), stack.durability);
                            break;
                    }
                    targetSlot.itemSlot.InsertStack(handOver);

                    stack.amount = BlockManager.I.MaxAmount(stack.blockID);
                }
                break;

            case EnumGameData.ItemKinds.item:
                if (stack.amount > ItemManager.I.MaxAmount(stack.itemType, stack.itemID))
                {
                    ItemStack handOver = null;
                    switch (stack.kinds)
                    {
                        case EnumGameData.ItemKinds.blockItem:
                            handOver = new ItemStack(stack.kinds, stack.blockID, stack.amount - ItemManager.I.MaxAmount(stack.itemType, stack.itemID));
                            break;

                        case EnumGameData.ItemKinds.item:
                            handOver = new ItemStack(stack.kinds, stack.itemType, stack.itemID, stack.amount - ItemManager.I.MaxAmount(stack.itemType, stack.itemID), stack.durability);
                            break;
                    }
                    targetSlot.itemSlot.InsertStack(handOver);

                    stack.amount = ItemManager.I.MaxAmount(stack.itemType, stack.itemID);
                }
                break;
        }
        slot.UpdateAmount();
    }

    public void UseTool(int decreaseDurabirity)
    {
        stack.durability -= decreaseDurabirity;
        slot.UpdateAmount();
        if (stack.durability <= 0)
        {
            EmptySlot();
        }
    }

    public bool HasItem
    {
        get
        {
            if(stack != null)
                return true;
            else
                return false;
        }
    }
}
