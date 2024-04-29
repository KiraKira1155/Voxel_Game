using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes",menuName = "Voxel_Game / Biome Attribute")]
public class BiomeAttributes : ScriptableObject
{
    [Tooltip("y=64の時の温度" +
        "\n2が中央位のイメージ、4が砂漠")]
    [Range(0, 5)]
    public int temperatureLv;

    [Tooltip("湿度レベル")]
    [Range(0, 4)]
    public int precipitationLv;

    [Tooltip("植生レベル" +
        "\n植物の生えやすさとは関係ないことに注意")]
    [Range(0, 4)]
    public int vegetationLv;

    [Tooltip("地形の最低高度" +
        "\nterrainHeighを足した平均値でレベルが決定")]
    [Range(64, 256)]
    public int solidGroundHight;

    [Tooltip("地形の高度幅" +
        "\nsolidGroundHightと足して256は越えないように注意")]
    public int terrainHeight;

    [Tooltip("数字が小さいほどなめらかな地形になる")]
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
    [Tooltip("チャンク毎の生成試行回数")]
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