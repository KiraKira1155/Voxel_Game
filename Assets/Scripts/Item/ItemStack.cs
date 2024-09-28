using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStack
{
    public EnumGameData.ItemKinds kinds;
    public EnumGameData.BlockID blockID;
    public EnumGameData.ItemType itemType;
    public EnumGameData.ItemID itemID;
    public int amount;
    public int durability;


    public ItemStack(EnumGameData.ItemKinds kinds, EnumGameData.BlockID blockID, int amount)
    {
        this.kinds = kinds;
        this.blockID = blockID;
        this.amount = amount;
        durability = -1;
        itemType = EnumGameData.ItemType.Hand;
        itemID = EnumGameData.ItemID.None;
    }

    public ItemStack(EnumGameData.ItemKinds kinds, EnumGameData.ItemType itemType, EnumGameData.ItemID itemID, int amount, int durability)
    {
        this.kinds = kinds;
        this.itemType = itemType;
        this.itemID = itemID;
        this.amount = amount;
        this.durability = durability;
        blockID = EnumGameData.BlockID.air;
    }
}
