using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "Voxel_Game / Material Attribute")]
public class MaterialAttributes : ScriptableObject
{
    public MaterialData[] materialDatas;
}

[System.Serializable]
public class MaterialData : IItemData
{
    [SerializeField] private string _name;
    [SerializeField] private Texture _texture;
    [SerializeField] private int _stackMaxSize;
    public string name()
    {
        return _name;
    }

    public int stackMaxSize()
    {
        return _stackMaxSize;
    }

    public Texture texture()
    {
        return _texture;
    }

    public EnumGameData.ItemKinds kinds()
    {
        return EnumGameData.ItemKinds.item;
    }
}