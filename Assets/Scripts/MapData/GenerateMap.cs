using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class GenerateMap
{
    private int biomeNum;
    private float seed;
    private float[,] voxelNoise = new float[VoxelData.Width, VoxelData.Width];
    
    private const float mapScale = 0.1f;
    private const short temperatureMapSize = 2048;
    private const short precipitationMapSize = 2897;
    private const short vegetationMapSize = 4096;
    private const short continentalnessMapSize = 6689;
    private byte[,] temperatureMap = new byte[temperatureMapSize, temperatureMapSize]; //64�`�����N��
    private byte[,] precipitationMap = new byte[precipitationMapSize, precipitationMapSize];  //32�`�����N��
    private byte[,] vegetationMap = new byte[vegetationMapSize, vegetationMapSize];  //16�`�����N��
    private byte[,] continentalnessMap = new byte[continentalnessMapSize, continentalnessMapSize];  //6�`�����N��

    private BiomeAttributes[] biome;
    private OceanBiomeAttributes[] oceanBiome;
    [SerializeField] private int[] tempetatureMapDebug;
    [SerializeField] private int[] precipitationMapDebug;
    [SerializeField] private int[] vegetationMapDebug;
    [SerializeField] private int[] continentalnessMapDebug;

    //biome[]�ɑΉ��������n���x���𐶐�
    //(256- 64 + 64 / 2=)160�ȏ�ōō����x��
    [SerializeField] private int[] terrestrialLv;

    System.Random rand = new System.Random(ConfigManager.thisWorldSeed);

    public void Init()
    {
        biomeNum = World.I.biome.Length;
        biome = World.I.biome;
        oceanBiome = World.I.oceanBiome;
        seed = ConfigManager.seed;
        GenerateMapData();
        //InitTerrestrialLvForBiome(); 
        //InitVoxelPerlinNoise();
    }

    private void GenerateMapData()
    {
        GenarateTemperatureMap();
        GenaratePrecipitationMap();
        GenarateVegetationMap();
        GenerateContinentalnessMap();
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

    /// <summary>
    /// �`�����N�̃o�C�I�[���^�C�v
    /// </summary>
    /// <param name="chunkCoord"></param>
    /// <param name="voxelPos"></param>
    /// <returns>
    /// ��1�߂�l���o�C�I�[���̎��
    /// <para>
    /// ��2�߂�l��true�Ȃ�C�Afalse�Ȃ痤�n�̃o�C�I�[��
    /// </para>
    /// </returns>
    public (byte, bool) GetBiomeType(ChunkCoord chunkCoord, Vector2 voxelPos)
    {
        if(BiomeTypeOcean(chunkCoord))
            for (byte i = 0; i < biomeNum; i++)
            {
                if (biome[i].temperatureLv == GetTemperatureLv(chunkCoord) &&
                    biome[i].precipitationLv == GetPrecipitationLv(chunkCoord) &&
                    biome[i].vegetationLv == GetVegetationLv(chunkCoord))
                {
                    return (i, true);
                }
            }
        else
            for (byte i = 0; i < biomeNum; i++)
            {
                if (biome[i].temperatureLv == GetTemperatureLv(chunkCoord) &&
                    biome[i].precipitationLv == GetPrecipitationLv(chunkCoord) &&
                    biome[i].vegetationLv == GetVegetationLv(chunkCoord))
                {
                    return (i, false);
                }
            }

        return (0, true);
    }

    private bool BiomeTypeOcean(ChunkCoord chunkCoord)
    {

        switch (GetContinentalnessLv(chunkCoord))
        {
            case 0:
            case 1:
            case 2:
                return true;

            case 3:
            case 4:
            case 5:
            case 6:
                return false;
        }

        return true;
    }

    private byte GetTerrestrialLv(int solidGroundHight)
    {
        byte x = 10;
        if (solidGroundHight >= 120)
            x = 7;
        else if (solidGroundHight >= 112)
            x = 6;
        else if (solidGroundHight >= 104)
            x = 5;
        else if (solidGroundHight >= 96)
            x = 4;
        else if (solidGroundHight >= 88)
            x = 3;
        else if (solidGroundHight >= 80)
            x = 2;
        else if (solidGroundHight >= 72)
            x = 1;
        else if (solidGroundHight >= 64)
            x = 0;

        return x;
    }

    /// <summary>
    /// �C�����x��
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0�`5�̐����l
    /// <para>
    /// 0���珇�� 13% 18% 27% 23% 12% 7%
    /// </para>
    /// </returns>
    private byte TemperatureLv(float noise)
    {
        if (noise > 0.93f)
            return 5;
        else if(noise > 0.81f)
            return 4;
        else if(noise > 0.58f)
            return 3;
        else if (noise > 0.31f)
            return 2;
        else if (noise > 0.13f)
            return 1;
        else
            return 0;

    }

    /// <summary>
    /// �~���ʃ��x��
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0�`4�̐����l
    /// <para>
    /// 0���珇�� 22% 29% 27% 17% 5%
    /// </para>
    /// </returns>
    private byte PrecipitationLv(float noise)
    {
        if (noise > 0.95f)
            return 4;
        else if (noise > 0.78f)
            return 3;
        else if (noise > 0.51f)
            return 2;
        else if (noise > 0.22f)
            return 1;
        else
            return 0;

    }

    /// <summary>
    /// �A�����x��
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0�`4�̐����l
    /// <para>
    /// 0���珇�� 15% 22% 26% 22% 15%
    /// </para>
    /// </returns>
    private byte VegetationLv(float noise)
    {
        if (noise > 0.85f)
            return 4;
        else if (noise > 0.63f)
            return 3;
        else if (noise > 0.37f)
            return 2;
        else if (noise > 0.15f)
            return 1;
        else
            return 0;

    }

    /// <summary>
    /// �嗤��
    /// �C�Ɨ��n�𕪂���̂Ɏg�p
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0�`6�̐����l
    /// <para>
    /// 0�`2���C�i�������Ⴂ�قǐ[���j���͗��n
    /// </para>
    /// <para>
    /// 0���珇�� �C(50%) 16.5% % 21% 12.5% ���n(50%) 18.5% 17.5% 9.5% 4.5%
    /// </para>
    /// </returns>
    private byte ContinentalnessLv(float noise)
    {
        if (noise > 0.955f)
            return 6;
        else if (noise > 0.86f)
            return 5;
        else if (noise > 0.685f)
            return 4;
        else if (noise > 0.5f)
            return 3;
        else if (noise > 0.375f)
            return 2;
        else if (noise > 0.165f)
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
            }
    }

    /// <summary>
    /// ���[���h�̋C���}�b�v����
    /// <para>
    /// 64�`�����N�ɂP��g�p
    /// </para>
    /// </summary>
    private void GenarateTemperatureMap()
    {
        float x = (float)rand.NextDouble() * seed * mapScale + 0.001f;
        float y = (float)rand.NextDouble() * seed * mapScale + 0.001f;
        for (int i= 0; i < temperatureMapSize; i++)
            for(int j= 0; j < temperatureMapSize; j++)
            {
                temperatureMap[i, j] = TemperatureLv(Mathf.PerlinNoise((i + seed) * 0.0001f + (temperatureMapSize - i) * x, (j + seed) * 0.0001f + (temperatureMapSize - j) * y));
                tempetatureMapDebug[temperatureMap[i, j]] += 1;
            }
    }
    /// <summary>
    /// �C�����x��
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0�`5�̐����l
    /// </returns>
    private byte GetTemperatureLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 64);
        int z = Mathf.FloorToInt(coord.z / 64);
        return temperatureMap[x,z];
    }

    /// <summary>
    /// ���[���h�̍~���ʃ}�b�v����
    /// <para>
    /// 32�`�����N�ɂP��g�p
    /// </para>
    /// </summary>
    private void GenaratePrecipitationMap()
    {
        float x = (float)rand.NextDouble() * seed * mapScale + 0.001f;
        float y = (float)rand.NextDouble() * seed * mapScale + 0.001f;
        for (int i = 0; i < precipitationMapSize; i++)
            for (int j = 0; j < precipitationMapSize; j++)
            {
                precipitationMap[i, j] = PrecipitationLv(Mathf.PerlinNoise((i + seed) * 0.0001f + (precipitationMapSize - i) * x, (j + seed) * 0.0001f + (precipitationMapSize - j) * y));
                precipitationMapDebug[precipitationMap[i, j]]++;
            }
    }
    /// <summary>
    /// �~���ʃ��x��
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0�`4�̐����l
    /// </returns>
    private byte GetPrecipitationLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 32);
        int z = Mathf.FloorToInt(coord.z / 32);
        return precipitationMap[x, z];
    }

    /// <summary>
    /// ���[���h�̐A���}�b�v����
    /// <para>
    /// 16�`�����N�ɂP��g�p
    /// </para>
    /// </summary>
    private void GenarateVegetationMap()
    {
        float x = (float)rand.NextDouble() * seed * mapScale + 0.001f;
        float y = (float)rand.NextDouble() * seed * mapScale + 0.001f;
        for (int i = 0; i < vegetationMapSize; i++)
            for (int j = 0; j < vegetationMapSize; j++)
            {
                vegetationMap[i, j] = VegetationLv(Mathf.PerlinNoise((i + seed) * 0.0001f + (vegetationMapSize - i) * x, (j + seed) * 0.0001f + (vegetationMapSize - j) * y));
                vegetationMapDebug[vegetationMap[i, j]]++;
            }
    }
    /// <summary>
    /// �A�����x��
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0�`4�̐����l
    /// </returns>
    private byte GetVegetationLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 16);
        int z = Mathf.FloorToInt(coord.z / 16);
        return vegetationMap[x, z];
    }

    /// <summary>
    /// �嗤���̐���
    /// </summary>
    private void GenerateContinentalnessMap()
    {
        float x = (float)rand.NextDouble() * seed * mapScale + 0.001f;
        float y = (float)rand.NextDouble() * seed * mapScale + 0.001f;
        for (int i = 0; i < continentalnessMapSize; i++)
            for (int j = 0; j < continentalnessMapSize; j++)
            {
                continentalnessMap[i, j] = ContinentalnessLv(Mathf.PerlinNoise((i + seed) * 0.0001f + (continentalnessMapSize - i) * x, (j + seed) * 0.0001f + (continentalnessMapSize - j) * y));
                continentalnessMapDebug[continentalnessMap[i, j]]++;
            }
    }
    /// <summary>
    /// �嗤�����x��
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0�`6�̐����l
    /// </returns>
    private byte GetContinentalnessLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 6);
        int z = Mathf.FloorToInt(coord.z / 6);
        return continentalnessMap[x, z];
    }
}
