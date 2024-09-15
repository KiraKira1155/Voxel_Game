using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private bool creative;

    private Slot slot;
    public List<ItemSlot> toolBarSlots = new List<ItemSlot>(PlayerStatus.SlotNum);

    private void Awake()
    {
        Init();
    }

    public void DoAwake(Slot slot)
    {
        this.slot = slot;
    }

    public void DoStart()
    {
        if (creative)
        {
            CreativeInventory();
        }
        else
        {
            SurvivalInventory();
        }
        GetSlotsToolBar();
    }

    private void SurvivalInventory()
    {

    }

    private void CreativeInventory()
    {
        for (int i = 1; i < BlockManager.I.blocktype.Length; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, transform);
            newSlot.transform.GetChild(0).GetComponent<ItemBlockUI>().DoAwake();

            ItemStack stack = new ItemStack(EnumGameData.ItemKinds.blockItem, (EnumGameData.BlockID)i, BlockManager.I.blocktype[i].stackMaxSize);
            ItemSlot slot = new ItemSlot(newSlot.GetComponent<UIItemSlot>(), stack);
            slot.isCreative = creative;
        }

        for (int i = 0; i < ItemManager.I.GetEachTypeCount(EnumGameData.ItemType.meleeWeapon); i++)
        {
            SlotTypeItemInit(EnumGameData.ItemType.meleeWeapon, i);
        }

        for (int i = 0; i < ItemManager.I.GetEachTypeCount(EnumGameData.ItemType.pickaxe); i++)
        {
            SlotTypeItemInit(EnumGameData.ItemType.pickaxe, i);
        }

        for (int i = 0; i < ItemManager.I.GetEachTypeCount(EnumGameData.ItemType.axe); i++)
        {
            SlotTypeItemInit(EnumGameData.ItemType.axe, i);
        }

        for (int i = 0; i < ItemManager.I.GetEachTypeCount(EnumGameData.ItemType.shovel); i++)
        {
            SlotTypeItemInit(EnumGameData.ItemType.shovel, i);
        }

        for (int i = 0; i < ItemManager.I.GetEachTypeCount(EnumGameData.ItemType.hoe); i++)
        {
            SlotTypeItemInit(EnumGameData.ItemType.hoe, i);
        }

        for (int i = 0; i < ItemManager.I.GetEachTypeCount(EnumGameData.ItemType.projectile); i++)
        {
            SlotTypeItemInit(EnumGameData.ItemType.projectile, i);
        }

        for (int i = 0; i < ItemManager.I.GetEachTypeCount(EnumGameData.ItemType.material); i++)
        {
            SlotTypeItemInit(EnumGameData.ItemType.material, i);
        }

        for (int i = 0; i < ItemManager.I.GetEachTypeCount(EnumGameData.ItemType.ore); i++)
        {
            SlotTypeItemInit(EnumGameData.ItemType.ore, i);
        }
    }

    private void SlotTypeItemInit(EnumGameData.ItemType type, int id)
    {
        GameObject newSlot = Instantiate(slotPrefab, transform);
        newSlot.transform.GetChild(1).GetComponent<ItemUI>().DoAwake();

        ItemStack stack = new ItemStack(EnumGameData.ItemKinds.item, type, (EnumGameData.ItemID)id, ItemManager.I.MaxAmount(type, (EnumGameData.ItemID)id), ItemManager.I.MaxDurability(type, (EnumGameData.ItemID)id));
        ItemSlot slot = new ItemSlot(newSlot.GetComponent<UIItemSlot>(), stack);
        slot.isCreative = creative;
    }

    private void GetSlotsToolBar()
    {
        int index = 0;
        foreach (UIItemSlot s in slot.toolbarSlots)
        {
            toolBarSlots.Add(slot.toolbarSlots[index].itemSlot);
            index++;
        }
    }

    public EnumGameData.BlockID GetToolBarSlotsBlockItemID(int slotID)
    {
        if (toolBarSlots[slotID].stack == null)
            return EnumGameData.BlockID.air;

        return toolBarSlots[slotID].stack.blockID;
    }

    public (EnumGameData.ItemType, EnumGameData.ItemID) GetToolBarSlotsItemID(int slotID)
    {
        if (toolBarSlots[slotID].stack == null)
            return (EnumGameData.ItemType.Null, EnumGameData.ItemID.None);

        return (toolBarSlots[slotID].stack.itemType, toolBarSlots[slotID].stack.itemID);
    }
}
