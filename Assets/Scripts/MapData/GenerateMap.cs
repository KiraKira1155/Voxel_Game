using System.Collections;
using System.Collections.Generic;
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
    private byte[,] temperatureMap = new byte[temperatureMapSize, temperatureMapSize]; //64チャンク毎
    private byte[,] precipitationMap = new byte[precipitationMapSize, precipitationMapSize];  //32チャンク毎
    private byte[,] vegetationMap = new byte[vegetationMapSize, vegetationMapSize];  //16チャンク毎

    private BiomeAttributes[] biome;
    [SerializeField] private int[] tempetatureMapDebug;
    [SerializeField] private int[] precipitationMapDebug;
    [SerializeField] private int[] vegetationMapDebug;

    //biome[]に対応した陸地レベルを生成
    //(256- 64 + 64 / 2=)160以上で最高レベル
    [SerializeField] private int[] terrestrialLv;

    System.Random rand = new System.Random(ConfigManager.thisWorldSeed);

    public void Init()
    {
        biomeNum = World.I.biome.Length;
        biome = World.I.biome;
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
    }

    /// <summary>
    /// チャンク生成時のボクセル高度
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="biomeType"></param>
    /// <returns>
    /// チャンク座標とバイオームの種類からブロックが存在する最高高度を返す
    /// </returns>
    public int GetSolidGroundHight(ChunkCoord coord, int biomeType)
    {
        return Mathf.FloorToInt(biome[biomeType].terrainHeight * GetVoxelPerBiomeHight(coord, biome[biomeType].terrainScale)) + biome[biomeType].solidGroundHight;
    }

    public byte GetBiomeType(ChunkCoord chunkCoord, Vector2 voxelPos)
    {
        for (byte i = 0; i < biomeNum; i++)
        {
            if (biome[i].temperatureLv == GetTemperatureLv(chunkCoord) &&
                biome[i].precipitationLv == GetPrecipitationLv(chunkCoord) &&
                biome[i].vegetationLv == GetVegetationLv(chunkCoord))
            {
                return i;
            }
        }
        return 0;
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

    private byte TemperatureLv(float noise)
    {
        if (noise > 0.9f)
            return 5;
        else if(noise > 0.75f)
            return 4;
        else if(noise > 0.55f)
            return 3;
        else if (noise > 0.25f)
            return 2;
        else if (noise > 0.10f)
            return 1;
        else
            return 0;

    }
    private byte PrecipitationLv(float noise)
    {
        if (noise > 0.92f)
            return 4;
        else if (noise > 0.8f)
            return 3;
        else if (noise > 0.45f)
            return 2;
        else if (noise > 0.15f)
            return 1;
        else
            return 0;

    }
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
    private byte ContinentalnessLv(float noise)
    {
        if (noise > 0.885f)
            return 6;
        else if (noise > 0.745f)
            return 5;
        else if (noise > 0.585)
            return 4;
        else if (noise > 0.415)
            return 3;
        else if (noise > 0.235f)
            return 2;
        else if (noise > 0.085f)
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
    private float GetVoxelPerBiomeHight(ChunkCoord coord, float scale)
    {
        Vector2 chunkPerlin = ChunkPerlinNoise(coord);
        return Mathf.PerlinNoise((chunkPerlin.x + seed) * scale, (chunkPerlin.y + seed) * scale);
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
    /// ワールドの気温マップ生成
    /// <para>
    /// 64チャンクに１回使用
    /// </para>
    /// </summary>
    private void GenarateTemperatureMap()
    {
        float x = (float)rand.NextDouble() * seed * mapScale;
        float y = (float)rand.NextDouble() * seed * mapScale;
        for (int i= 0; i < temperatureMapSize; i++)
            for(int j= 0; j < temperatureMapSize; j++)
            {
                temperatureMap[i, j] = TemperatureLv(Mathf.PerlinNoise(i * 0.00000001f + x * (4194304 - i), (i - 4194304) * 0.00000001f + y * i));
                tempetatureMapDebug[temperatureMap[i, j]]++;
            }
    }

    private byte GetTemperatureLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 64);
        int z = Mathf.FloorToInt(coord.z / 64);
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
        float x = (float)rand.NextDouble() * seed * mapScale;
        float y = (float)rand.NextDouble() * seed * mapScale;
        for (int i = 0; i < precipitationMapSize; i++)
            for (int j = 0; j < precipitationMapSize; j++)
            {
                precipitationMap[i, j] = PrecipitationLv(Mathf.PerlinNoise(i * 0.00000001f + x * (8392609 - i), (i - 8392609) * 0.00000001f + y * i));
                precipitationMapDebug[precipitationMap[i, j]]++;
            }
    }
    private byte GetPrecipitationLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 32);
        int z = Mathf.FloorToInt(coord.z / 32);
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
        float x = (float)rand.NextDouble() * seed * mapScale;
        float y = (float)rand.NextDouble() * seed * mapScale;
        for (int i = 0; i < vegetationMapSize; i++)
            for (int j = 0; j < vegetationMapSize; j++)
            {
                vegetationMap[i, j] = VegetationLv(Mathf.PerlinNoise(i * 0.00000001f + x * (16777216 - i), (i - 16777216) * 0.00000001f + y * i));
                vegetationMapDebug[vegetationMap[i, j]]++;
            }
    }
    private byte GetVegetationLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 16);
        int z = Mathf.FloorToInt(coord.z / 16);
        return vegetationMap[x, z];
    }

    /// <summary>
    /// 大陸性の生成
    /// </summary>
    private void GenerateContinentalness()
    {

    }
    private byte GetContinentalnessLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 6);
        int z = Mathf.FloorToInt(coord.z / 6);
        return vegetationMap[x, z];
    }
}
