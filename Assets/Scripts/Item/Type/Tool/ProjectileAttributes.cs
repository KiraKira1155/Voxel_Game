using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileAttributes", menuName = "Voxel_Game / Projectile Attribute")]
public class ProjectileAttributes : ScriptableObject
{
    public ProjectileData[] projectileDatas;
}

public interface IProjectileData : IItemData
{
    EnumGameData.ItemType itemType();
    EnumGameData.WeaponAttribute attribute();
    float damage();
    float reuseTime();
}

[System.Serializable]
public class ProjectileData : IProjectileData
{
    [SerializeField] private string _name;
    [SerializeField] private Texture _texture;
    [SearchableEnum][SerializeField] private EnumGameData.WeaponAttribute _attribute;
    [SerializeField] private int _stackMaxSize;
    [Range(0, 100)][SerializeField] private float _damage;
    [SerializeField] private float _reuseTime;

    public EnumGameData.WeaponAttribute attribute()
    {
        return _attribute;
    }

    public float damage()
    {
        return _damage;
    }

    public string name()
    {
        return _name;
    }

    public float reuseTime()
    {
        return _reuseTime;
    }

    public int stackMaxSize()
    {
        return _stackMaxSize;
    }

    public Texture texture()
    {
        return _texture;
    }
    public EnumGameData.ItemType itemType()
    {
        return EnumGameData.ItemType.projectile;
    }

    public EnumGameData.ItemKinds kinds()
    {
        return EnumGameData.ItemKinds.item;
    }
}