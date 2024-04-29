using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "Voxel_Game / Tool Attribute")]
public class ToolAttributes : ScriptableObject
{
    public ToolData[] toolDatas;
}

public interface IToolData : IItemData
{
    EnumGameData.ItemType itemType();
    EnumGameData.Attribute attribute();
    int rarity();
    float damage();
    float reuseTime();
    int durability();
}

[System.Serializable]
public class ToolData : IToolData
{
    [SerializeField] private string _name;
    [SerializeField] private Texture _texture;
    [SearchableEnum][SerializeField] private EnumGameData.Attribute _attribute;
    [Range(1, 10)][SerializeField] private int _rarity;
    [Range(1, 100)][SerializeField] private float _damage;
    [Tooltip("-1ÇÕÅAëœãvÇ™ñ≥å¿")]
    [Range(-1, 10000)][SerializeField] private int _durability;
    [Range(0, 10)][SerializeField] private float _reuseTime;

    public EnumGameData.Attribute attribute()
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

    public float reuseTime()
    {
        return _reuseTime;
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
