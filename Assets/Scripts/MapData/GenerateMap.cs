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

    //biome[]に対応した陸地レベルを生成
    //(256- 64 + 64 / 2=)160以上で最高レベル
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
    /// チャンク生成時のボクセル高度
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="biomeType"></param>
    /// <returns>
    /// チャンク座標とバイオームの種類からブロックが存在する最高高度を返す
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
    /// チャンクのバイオームタイプ
    /// </summary>
    /// <param name="chunkCoord"></param>
    /// <param name="voxelPos"></param>
    /// <returns>
    /// 第1戻り値がバイオームの種類
    /// <para>
    /// 第2戻り値がtrueなら海、falseなら陸地のバイオーム
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
    /// 気温レベル
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0～4の整数値
    /// <para>
    /// 0から順に 5% 21% 30% 27% 17%
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
    /// 降水量レベル
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0～3の整数値
    /// <para>
    /// 0から順に 27% 29% 25% 19%
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
    /// 植生レベル
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0～4の整数値
    /// <para>
    /// 0から順に 15% 22% 26% 22% 15%
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
    /// 大陸性
    /// 海と陸地を分けるのに使用
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0～6の整数値
    /// <para>
    /// 0～2が海（数字が低いほど深く）他は陸地
    /// </para>
    /// <para>
    /// 0から順に 海(50%) 16.5% % 21% 12.5% 陸地(50%) 18.5% 17.5% 9.5% 4.5%
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
    /// 洞窟生成
    /// </summary>
    public void GetDepthSpaceLv()
    {

    }
    private float GetVoxelPerBiomeHight(Vector2 position, float scale)
    {
        return Mathf.PerlinNoise(((position.x + 0.01f) / VoxelData.Width) * scale + seed, ((position.y + 0.01f) / VoxelData.Width) * scale + seed);
    }

    /// <summary>
    /// チャンクのパーリンノイズ生成
    /// </summary>
    /// <returns>
    /// x,z共にパーリンノイズにて値を取得し、Vector2に変換
    /// <para>
    /// 戻り値の最小値は0.001、最大は1を少し超える可能性あり
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
    /// ワールドの気温マップ生成
    /// <para>
    /// 64チャンクに１回使用
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
    /// 気温レベル
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0～5の整数値
    /// </returns>
    private byte GetTemperatureLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 16);
        int z = Mathf.FloorToInt(coord.z / 16);
        return temperatureMap[x,z];
    }

    /// <summary>
    /// ワールドの降水量マップ生成
    /// <para>
    /// 32チャンクに１回使用
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
    /// 降水量レベル
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0～4の整数値
    /// </returns>
    private byte GetPrecipitationLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 8);
        int z = Mathf.FloorToInt(coord.z / 8);
        return precipitationMap[x, z];
    }

    /// <summary>
    /// ワールドの植生マップ生成
    /// <para>
    /// 16チャンクに１回使用
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
    /// 植生レベル
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0～4の整数値
    /// </returns>
    private byte GetVegetationLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 4);
        int z = Mathf.FloorToInt(coord.z / 4);
        return vegetationMap[x, z];
    }

    /// <summary>
    /// 大陸性の生成
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
    /// 大陸性レベル
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0～6の整数値
    /// </returns>
    private byte GetContinentalnessLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 8);
        int z = Mathf.FloorToInt(coord.z / 8);
        return continentalnessMap[x, z];
    }
}
