using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OceanBiomeAttributes", menuName = "Voxel_Game / Ocean Biome Attribute")]
public class OceanBiomeAttributes : ScriptableObject
{
    [Tooltip("y=64の時の温度" +
        "\n1より下は氷雪地帯、2が温帯のイメージ、4が猛暑" +
        "\n1より下はY64で水が氷る")]
    [Range(0, 4)]
    public byte temperatureLv;


    [Tooltip("海洋深度、y=63より下が水になる" +
        "\n0が深海、1が海洋、2が浅海" +
        "\n0は基本の深さがy=24、1がy=43、2がy=55")]
    [Range(0, 2)]
    public byte continentalness; 
    
    [Tooltip("地形の高度幅")]
    public short terrainHeight;

    [Tooltip("数字が小さいほどなめらかな地形になる")]
    public float terrainScale;

    [SearchableEnum] public EnumGameData.BlockID topBlocks;
    [SearchableEnum] public EnumGameData.BlockID middleLayer;
    [SearchableEnum] public EnumGameData.BlockID basicsBlocks;
}
