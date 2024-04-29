using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenerateMap
{
    private int biomeNum;
    private float seed;
    private const float temperatureScale = 0.05f;
    private float[,] voxelNoise = new float[VoxelData.Width, VoxelData.Width];

    private BiomeAttributes[] biome;

    //biome[]�ɑΉ��������n���x���𐶐�
    //(256- 64 + 64 / 2=)160�ȏ�ōō����x��
    [SerializeField] private int[] terrestrialLv;

    public void Init()
    {
        biomeNum = World.I.biome.Length;
        biome = World.I.biome;
        seed = ConfigManager.seed;
        InitTerrestrialLvForBiome(); 
        InitVoxelPerlinNoise();
    }

    /// <summary>
    /// �`�����N�������̃{�N�Z�����x
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="biomeType"></param>
    /// <returns>
    /// �`�����N���W�ƃo�C�I�[���̎�ނ���u���b�N�����݂���ō����x��Ԃ�
    /// </returns>
    public int GetSolidGroundHight(ChunkCoord coord, int biomeType)
    {
        return Mathf.FloorToInt(biome[biomeType].terrainHeight * GetVoxelPerBiomeHight(coord, biome[biomeType].terrainScale)) + biome[biomeType].solidGroundHight;
    }

    public int GetBiomeType(ChunkCoord chunkCoord, Vector2 voxelPos)
    {
        for (int i = 0; i < biomeNum; i++)
        {
            if (biome[i].temperatureLv == GetTemperatureLv(chunkCoord, voxelPos))
            {
                return i;
            }
        }
        return 0;
    }

    private int GetTerrestrialLv(int solidGroundHight)
    {
        int x = solidGroundHight;
        if (x >= 160)
            x = 7;
        else if (x >= 136)
            x = 6;
        else if (x >= 112)
            x = 5;
        else if (x >= 88)
            x = 4;
        else if (x >= 64)
            x = 3;
        else if (x >= 54)
            x = 2;
        else if (x >= 40)
            x = 1;
        else if (x >= 26)
            x = 0;
        else
            x = -1;

        return x;
    }
    
    /// <summary>
    /// Y=64�̊�{���xLv�̐���
    /// </summary>
    /// <returns>
    /// �p�[�����m�C�Y�Ő��������U�i�K�̉��x���x�����Ԃ��Ă���
    /// <para>�����ɂ̓`�����N���W�ƃV�[�h�l�A�{�N�Z�����W���֌W</para>
    /// </returns>
    private int GetTemperatureLv(ChunkCoord coord, Vector2 voxelPos)
    {
        Vector2 chunkPerlin = ChunkPerlinNoise(coord);
        float voxelScale = voxelNoise[(int)voxelPos.x, (int)voxelPos.y];
        return TemperatureLv(Mathf.PerlinNoise((chunkPerlin.x + voxelScale + coord.x * seed)�@* temperatureScale, (chunkPerlin.y + voxelScale + coord.z * seed) * temperatureScale));
    }

    private int TemperatureLv(float getTemperatureLv)
    {
        if (getTemperatureLv > 0.9f)
            return 5;
        else if(getTemperatureLv > 0.75f)
            return 4;
        else if(getTemperatureLv > 0.55f)
            return 3;
        else if (getTemperatureLv > 0.25f)
            return 2;
        else if (getTemperatureLv > 0.10f)
            return 1;
        else
            return 0;

    }

    /// <summary>
    /// ���A����
    /// </summary>
    public void GetDepthSpaceLv()
    {

    }
    private float GetVoxelPerBiomeHight(ChunkCoord coord, float scale)
    {
        Vector2 chunkPerlin = ChunkPerlinNoise(coord);
        return Mathf.PerlinNoise((chunkPerlin.x + seed) * scale, (chunkPerlin.y + seed) * scale);
    }

    /// <summary>
    /// �`�����N�̃p�[�����m�C�Y����
    /// </summary>
    /// <returns>
    /// x,z���Ƀp�[�����m�C�Y�ɂĒl���擾���AVector2�ɕϊ�
    /// <para>
    /// �߂�l�̍ŏ��l��0.001�A�ő��1������������\������
    /// </para>
    /// </returns>
    private Vector2 ChunkPerlinNoise(ChunkCoord coord)
    {
        float vec = new Vector2(coord.x, coord.z).magnitude * 0.00007f;
        return new Vector2(Mathf.PerlinNoise((coord.x + seed) * 0.00009f, vec) + 0.001f, Mathf.PerlinNoise((coord.z + seed) * 0.0009f, vec) + 0.001f);
    }


    private void InitTerrestrialLvForBiome()
    {
        int x = 0;
        terrestrialLv = new int[biomeNum];
        foreach (var type in biome)
        {
            terrestrialLv[x] = GetTerrestrialLv((type.solidGroundHight + type.solidGroundHight + type.terrainHeight) / 2);
            x++;
        }
    }

    private void InitVoxelPerlinNoise()
    {
        for(int x = 0; x < VoxelData.Width; x++)
            for(int y = 0;y < VoxelData.Width; y++)
            {
                voxelNoise[x, y] = Mathf.PerlinNoise((x + seed) * 0.01f, (y + seed) * 0.01f);
                Debug.Log(voxelNoise[x, y]);
            }
    }
}
