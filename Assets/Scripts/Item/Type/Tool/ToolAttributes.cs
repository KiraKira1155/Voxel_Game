using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolAttributes", menuName = "Voxel_Game / Tool Attribute")]
public class ToolAttributes : ScriptableObject
{
    public ToolData[] toolDatas;
}

public interface IToolData : IItemData
{
    EnumGameData.ItemType itemType();
    EnumGameData.WeaponAttribute attribute();
    int rarity();
    float damage();
    float attackPerSecond();
    int durability();
}

[System.Serializable]
public class ToolData : IToolData
{
    [SerializeField] private string _name;
    [SerializeField] private Texture _texture;
    [SearchableEnum][SerializeField] private EnumGameData.WeaponAttribute _attribute;
    [Range(1, 10)][SerializeField] private int _rarity;
    [Range(1, 100)][SerializeField] private float _damage;
    [Tooltip("-1�́A�ϋv������")]
    [Range(-1, 10000)][SerializeField] private int _durability;
    [Range(0, 10)][SerializeField] private float _attackPerSecond;

    public EnumGameData.WeaponAttribute attribute()
    {
        return _attribute;
    }

    public float damage()
    {
        return _damage;
    }

    public int durability()
    {
        return _durability;
    }

    public string name()
    {
        return _name;
    }

    public int rarity()
    {
        return _rarity;
    }

    public float attackPerSecond()
    {
        return _attackPerSecond;
    }

    public int stackMaxSize()
    {
        return 1;
    }

    public EnumGameData.ItemType itemType()
    {
        return EnumGameData.ItemType.meleeWeapon;
    }

    public EnumGameData.ItemKinds kinds()
    {
        return EnumGameData.ItemKinds.item;
    }

    public Texture texture()
    {
        return _texture;
    }
}
