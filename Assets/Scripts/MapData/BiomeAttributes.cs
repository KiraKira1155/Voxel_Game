using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes",menuName = "Voxel_Game / Biome Attribute")]
public class BiomeAttributes : ScriptableObject
{
    [Tooltip("y=64�̎��̉��x" +
        "\n2�������ʂ̃C���[�W�A4������")]
    [Range(0, 5)]
    public int temperatureLv;

    [Tooltip("���x���x��")]
    [Range(0, 4)]
    public int precipitationLv;

    [Tooltip("�A�����x��" +
        "\n�A���̐����₷���Ƃ͊֌W�Ȃ����Ƃɒ���")]
    [Range(0, 4)]
    public int vegetationLv;

    [Tooltip("�n�`�̍Œፂ�x" +
        "\nterrainHeigh�𑫂������ϒl�Ń��x��������")]
    [Range(64, 256)]
    public int solidGroundHight;

    [Tooltip("�n�`�̍��x��" +
        "\nsolidGroundHight�Ƒ�����256�͉z���Ȃ��悤�ɒ���")]
    public int terrainHeight;

    [Tooltip("�������������قǂȂ߂炩�Ȓn�`�ɂȂ�")]
    public float terrainScale;

    [SearchableEnum] public EnumGameData.BlockID topBlocks;
    [SearchableEnum] public EnumGameData.BlockID middleLayer;
    [SearchableEnum] public EnumGameData.BlockID basicsBlocks;
}

[System.Serializable]
public class UndergroundProducts
{
    [SearchableEnum] public EnumGameData.BlockID blockID;
    public int minHeight;
    public int maxHeight;
    public int minScale;
    public int maxScale;
    [Tooltip("�`�����N���̐������s��")]
    public int trialNum;
    [Range(0f, 0.7f)]
    public float threshold;
    [Range(0, 1f)]
    public float noiseOffset;
}

[System.Serializable]
public class TreeProducts
{
    [SearchableEnum] public EnumGameData.BlockID lootBlock;
    [SearchableEnum] public EnumGameData.BlockID trunkBlock;
    [SearchableEnum] public EnumGameData.BlockID leafBlock;

    public float treeZoneScale = 1.3f;
    [Range(0.1f, 1f)]
    public float treeZoneThreshold = 0.6f;
    public float treePlacementScale = 15f;
    [Range(0.1f, 1f)]
    public float treePlacementThreshold = 0.8f;
    public int maxTreeHeight = 12;
    public int minTreeHeight = 5;
}