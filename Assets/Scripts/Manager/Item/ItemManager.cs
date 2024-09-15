using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    /*
     新しいアイテムの追加手順
    ・新しくDataフォルダのItemフォルダにToolAttributesを作成
    ・ここに各アイテムのToolAttributes追加
    ・必要な場合は下の関数に追記
    ・InventoryクラスのCreativeInventory関数に追記
    ・EnumGameDataクラスのItemIDにインスペクター設定順で追記
     */

    [SerializeField] private ToolAttributes meleeWeapon;
    [SerializeField] private ToolAttributes pickaxe;
    [SerializeField] private ToolAttributes axe;
    [SerializeField] private ToolAttributes shovel;
    [SerializeField] private ToolAttributes hoe;

    [SerializeField] private ProjectileAttributes projectile;

    [SerializeField] private MaterialAttributes material;
    [SerializeField] private MaterialAttributes ore;

    private void Awake()
    {
        Init();
    }

    public int GetEachTypeCount(EnumGameData.ItemType itemType)
    {
        switch (itemType)
        {
            case EnumGameData.ItemType.Null:
                return 0;

            case EnumGameData.ItemType.meleeWeapon:
                return meleeWeapon.toolDatas.Length;

            case EnumGameData.ItemType.pickaxe:
                return pickaxe.toolDatas.Length;

            case EnumGameData.ItemType.axe:
                return axe.toolDatas.Length;

            case EnumGameData.ItemType.shovel:
                return shovel.toolDatas.Length;

            case EnumGameData.ItemType.hoe:
                return hoe.toolDatas.Length;

            case EnumGameData.ItemType.projectile:
                return projectile.projectileDatas.Length;

            case EnumGameData.ItemType.material:
                return material.materialDatas.Length;

            case EnumGameData.ItemType.ore:
                return ore.materialDatas.Length;
        }
        return 0;
    }

    public Texture GetTexture(EnumGameData.ItemType itemType, EnumGameData.ItemID itemID)
    {
        switch (itemType)
        {
            case EnumGameData.ItemType.Null:
                return null;

            case EnumGameData.ItemType.meleeWeapon:
                return meleeWeapon.toolDatas[(int)itemID].texture();

            case EnumGameData.ItemType.pickaxe:
                return pickaxe.toolDatas[(int)itemID].texture();

            case EnumGameData.ItemType.axe:
                return axe.toolDatas[(int)itemID].texture();

            case EnumGameData.ItemType.shovel:
                return shovel.toolDatas[(int)itemID].texture();

            case EnumGameData.ItemType.hoe:
                return hoe.toolDatas[(int)itemID].texture();

            case EnumGameData.ItemType.projectile:
                return projectile.projectileDatas[(int)itemID].texture();

            case EnumGameData.ItemType.material:
                return material.materialDatas[(int)itemID].texture();

            case EnumGameData.ItemType.ore:
                return ore.materialDatas[(int)itemID].texture();
        }
        return null;
    }

    public int GetRarity(EnumGameData.ItemType itemType, EnumGameData.ItemID itemID)
    {
        switch (itemType)
        {
            case EnumGameData.ItemType.meleeWeapon:
                return meleeWeapon.toolDatas[(int)itemID].rarity();

            case EnumGameData.ItemType.pickaxe:
                return pickaxe.toolDatas[(int)itemID].rarity();

            case EnumGameData.ItemType.axe:
                return axe.toolDatas[(int)itemID].rarity();

            case EnumGameData.ItemType.shovel:
                return shovel.toolDatas[(int)itemID].rarity();

            case EnumGameData.ItemType.hoe:
                return hoe.toolDatas[(int)itemID].rarity();
        }

        return 0;
    }
    public int MaxAmount(EnumGameData.ItemType itemType, EnumGameData.ItemID itemID)
    {
        switch (itemType)
        {
            case EnumGameData.ItemType.Null:
                break;

            case EnumGameData.ItemType.meleeWeapon:
                return meleeWeapon.toolDatas[(int)itemID].stackMaxSize();

            case EnumGameData.ItemType.pickaxe:
                return pickaxe.toolDatas[(int)itemID].stackMaxSize();

            case EnumGameData.ItemType.axe:
                return axe.toolDatas[(int)itemID].stackMaxSize();

            case EnumGameData.ItemType.shovel:
                return shovel.toolDatas[(int)itemID].stackMaxSize();

            case EnumGameData.ItemType.hoe:
                return hoe.toolDatas[(int)itemID].stackMaxSize();

            case EnumGameData.ItemType.projectile:
                return projectile.projectileDatas[(int)itemID].stackMaxSize();

            case EnumGameData.ItemType.material:
                return material.materialDatas[(int)itemID].stackMaxSize();

            case EnumGameData.ItemType.ore:
                return ore.materialDatas[(int)itemID].stackMaxSize();
        }
        return 0;
    }

    public int MaxDurability(EnumGameData.ItemType itemType, EnumGameData.ItemID itemID)
    {
        switch (itemType)
        {
            case EnumGameData.ItemType.Null:
                break;

            case EnumGameData.ItemType.meleeWeapon:
                return meleeWeapon.toolDatas[(int)itemID].durability();

            case EnumGameData.ItemType.pickaxe:
                return pickaxe.toolDatas[(int)itemID].durability();

            case EnumGameData.ItemType.axe:
                return axe.toolDatas[(int)itemID].durability();

            case EnumGameData.ItemType.shovel:
                return shovel.toolDatas[(int)itemID].durability();

            case EnumGameData.ItemType.hoe:
                return hoe.toolDatas[(int)itemID].durability();
        }
        return -1;
    }

    public EnumGameData.ItemType TypeFromID(EnumGameData.ItemID itemID)
    {
        int id = (int)itemID;
        if (id >= 600)
            return EnumGameData.ItemType.block;
        else if (id >= 500)
            return EnumGameData.ItemType.ore;
        else if (id >= 200)
            return EnumGameData.ItemType.material;
        else if (id >= 150)
            return EnumGameData.ItemType.projectile;
        else if (id >= 100)
            return EnumGameData.ItemType.hoe;
        else if (id >= 75)
            return EnumGameData.ItemType.shovel;
        else if (id >= 50)
            return EnumGameData.ItemType.axe;
        else if (id >= 25)
            return EnumGameData.ItemType.pickaxe;
        else if (id >= 1)
            return EnumGameData.ItemType.meleeWeapon;
        else
            return EnumGameData.ItemType.Null;
    }

    public EnumGameData.BlockID BlockIDFromItemID(EnumGameData.ItemID itemID)
    {
        int id = (int)itemID - 599;
        return (EnumGameData.BlockID)id;
    }
}
