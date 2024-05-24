using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GenerateMap : Singleton<GenerateMap>
{
    private int biomeNum;
    private int oceanBiomeNum;
    private float seed;
    private float[,] voxelNoise = new float[VoxelData.Width, VoxelData.Width];
    
    private const float mapScale = 0.1f;
    private const short temperatureMapSize = 512;
    private const short precipitationMapSize = 1024;
    private const short vegetationMapSize = 2048;
    private const short continentalnessMapSize = 1024;
    private byte[,] temperatureMap = new byte[temperatureMapSize, temperatureMapSize];
    private byte[,] precipitationMap = new byte[precipitationMapSize, precipitationMapSize];
    private byte[,] vegetationMap = new byte[vegetationMapSize, vegetationMapSize];
    private byte[,] continentalnessMap = new byte[continentalnessMapSize, continentalnessMapSize];

    private (byte, bool)[,] biomeMap = new (byte, bool)[ConfigManager.WorldSizeInChunks, ConfigManager.WorldSizeInChunks];

    private BiomeAttributes[] biome;
    private OceanBiomeAttributes[] oceanBiome;

    //biome[]�ɑΉ��������n���x���𐶐�
    //(256- 64 + 64 / 2=)160�ȏ�ōō����x��
    [SerializeField] private int[] terrestrialLv;

    System.Random rand = new System.Random(ConfigManager.thisWorldSeed);

    [SerializeField] int DebugOceanTrue;
    [SerializeField] int DebugOceanFalse;
    [SerializeField] int[] DebugBiome;

    public bool successGenerateMap = false;

    private void Awake()
    {
        Init();
    }

    public void DoAwake()
    {
        biomeNum = World.I.biome.Length;
        biome = World.I.biome;
        oceanBiomeNum = World.I.oceanBiome.Length;
        oceanBiome = World.I.oceanBiome;
        seed = ConfigManager.seed;

        DebugBiome = new int[biomeNum];

        var task = Task.Run(() =>
        {
            GenarateTemperatureMap();
            GenaratePrecipitationMap();
            GenarateVegetationMap();
            GenerateContinentalnessMap();
        });
        task.Wait();

        successGenerateMap = true;
        //InitTerrestrialLvForBiome(); 
        //InitVoxelPerlinNoise();
    }

    private void Debug()
    {
        //int ocean = 0;
        //int continant = 0;
        //int[] biome = new int[biomeNum];

        //for (int x = 0; x < ConfigManager.WorldSizeInChunks; x++)
        //    for (int z = 0; z < ConfigManager.WorldSizeInChunks; z++)
        //    {
        //        ChunkCoord chunk = new ChunkCoord(x, z);
        //        if (GetBiomeType(chunk).Item2)
        //            ocean++;
        //        else
        //        {
        //            continant++;
        //            biome[GetBiomeType(chunk).Item1]++;
        //        }
        //    }

        //DebugOceanTrue = ocean;
        //DebugOceanFalse = continant;
        //biome.CopyTo(DebugBiome, 0);
    }

    /// <summary>
    /// �`�����N�������̃{�N�Z�����x
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="biomeType"></param>
    /// <returns>
    /// �`�����N���W�ƃo�C�I�[���̎�ނ���u���b�N�����݂���ō����x��Ԃ�
    /// </returns>
    public int GetSolidGroundHight(Vector2 position, byte biomeType)
    {
        return Mathf.FloorToInt(biome[biomeType].terrainHeight * GetVoxelPerBiomeHight(position, biome[biomeType].terrainScale)) + 64;
    }

    public int GetSolidOceanGroundHight(Vector2 position, byte biomeType)
    {
        int solidGroundHight()
        {
            switch(oceanBiome[biomeType].continentalness)
            {
                case 0:
                    return 24;

                case 1:
                    return 43;
                
                default:
                    return 55;
            }
        }
        return Mathf.FloorToInt(oceanBiome[biomeType].terrainHeight * GetVoxelPerBiomeHight(position, oceanBiome[biomeType].terrainScale)) + solidGroundHight();
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
    public (byte, bool) GetBiomeType(ChunkCoord chunkCoord)
    {
        return biomeMap[chunkCoord.x, chunkCoord.z];
    }

    public void AddGenerateBiomeMap(ChunkCoord chunkCoord)
    {
        biomeMap[chunkCoord.x, chunkCoord.z] = GenerateBiomeMap(chunkCoord);
    }

    private (byte, bool) GenerateBiomeMap(ChunkCoord chunkCoord)
    {
        Queue<byte> candidateBiome = new Queue<byte>();
        if (BiomeTypeOcean(chunkCoord))
        {
            for (byte i = 0; i < oceanBiomeNum; i++)
            {
                if (oceanBiome[i].temperatureLv == GetTemperatureLv(chunkCoord) &&
                    oceanBiome[i].continentalness == GetContinentalnessLv(chunkCoord))
                {
                    return (i, true);
                }
            }
            return (0, true);
        }
        else
        {
            for (byte i = 0; i < biomeNum; i++)
            {
                if (biome[i].temperatureLv == GetTemperatureLv(chunkCoord))
                {
                    if (biome[i].vegetationLv == GetVegetationLv(chunkCoord))
                    {
                        candidateBiome.Enqueue(i);
                        if (biome[i].precipitationLv == GetPrecipitationLv(chunkCoord))
                        {
                            return (i, false);
                        }
                    }
                }
            }

            return (candidateBiome.Dequeue(), false);
        }
    }

    private bool BiomeTypeOcean(ChunkCoord chunkCoord)
    {
        bool ocean;
        switch (GetContinentalnessLv(chunkCoord))
        {
            case 0:
                ocean = true;
                break;
            case 1:
                ocean = true;
                break;
            case 2:
                ocean = true;
                break;

            case 3:
                ocean = false;
                break;
            case 4:
                ocean = false;
                break;
            case 5:
                ocean = false;
                break;
            default:
                ocean = false;
                break;
        }
        
        return ocean;
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
    /// 0�`4�̐����l
    /// <para>
    /// 0���珇�� 5% 21% 30% 27% 17%
    /// </para>
    /// </returns>
    private byte TemperatureLv(float noise)
    {   
        if(noise > 0.83f)
            return 4;
        else if(noise > 0.56f)
            return 3;
        else if (noise > 0.26f)
            return 2;
        else if (noise > 0.05f)
            return 1;
        else
            return 0;

    }

    /// <summary>
    /// �~���ʃ��x��
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0�`3�̐����l
    /// <para>
    /// 0���珇�� 27% 29% 25% 19%
    /// </para>
    /// </returns>
    private byte PrecipitationLv(float noise)
    {
        if (noise > 0.81f)
            return 3;
        else if (noise > 0.56f)
            return 2;
        else if (noise > 0.27f)
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
    private float GetVoxelPerBiomeHight(Vector2 position, float scale)
    {
        return Mathf.PerlinNoise(((position.x + 0.01f) / VoxelData.Width) * scale + seed, ((position.y + 0.01f) / VoxelData.Width) * scale + seed);
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
        int x = Mathf.FloorToInt(coord.x / 16);
        int z = Mathf.FloorToInt(coord.z / 16);
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
        int x = Mathf.FloorToInt(coord.x / 8);
        int z = Mathf.FloorToInt(coord.z / 8);
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
        int x = Mathf.FloorToInt(coord.x / 4);
        int z = Mathf.FloorToInt(coord.z / 4);
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
        int x = Mathf.FloorToInt(coord.x / 8);
        int z = Mathf.FloorToInt(coord.z / 8);
        return continentalnessMap[x, z];
    }
}
