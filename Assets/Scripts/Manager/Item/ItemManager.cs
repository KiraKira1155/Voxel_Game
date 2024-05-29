using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    /*
     �V�����A�C�e���̒ǉ��菇
    �E�V����Data�t�H���_��Item�t�H���_��ToolAttributes���쐬
    �E�����Ɋe�A�C�e����ToolAttributes�ǉ�
    �E�K�v�ȏꍇ�͉��̊֐��ɒǋL
    �EInventory�N���X��CreativeInventory�֐��ɒǋL
    �EEnumGameData�N���X��ItemID�ɃC���X�y�N�^�[�ݒ菇�ŒǋL
     */

    [SerializeField] private ToolAttributes meleeWeapon;
    [SerializeField] private ToolAttributes pickaxe;
    [SerializeField] private ToolAttributes axe;
    [SerializeField] private ToolAttributes shovel;
    [SerializeField] private ToolAttributes hoe;

    [SerializeField] private MaterialAttributes ore;

    private IItemData itemData;

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
}
