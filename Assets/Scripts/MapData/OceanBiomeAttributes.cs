using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OceanBiomeAttributes", menuName = "Voxel_Game / Ocean Biome Attribute")]
public class OceanBiomeAttributes : ScriptableObject
{
    [Tooltip("y=64�̎��̉��x" +
        "\n1��艺�͕X��n�сA2�����т̃C���[�W�A4���ҏ�" +
        "\n1��艺��Y64�Ő����X��")]
    [Range(0, 4)]
    public byte temperatureLv;


    [Tooltip("�C�m�[�x�Ay=63��艺�����ɂȂ�" +
        "\n0���[�C�A1���C�m�A2����C" +
        "\n0�͊�{�̐[����y=24�A1��y=43�A2��y=55")]
    [Range(0, 2)]
    public byte continentalness; 
    
    [Tooltip("�n�`�̍��x��")]
    public short terrainHeight;

    [Tooltip("�������������قǂȂ߂炩�Ȓn�`�ɂȂ�")]
    public float terrainScale;

    [SearchableEnum] public EnumGameData.BlockID topBlocks;
    [SearchableEnum] public EnumGameData.BlockID middleLayer;
    [SearchableEnum] public EnumGameData.BlockID basicsBlocks;
}
