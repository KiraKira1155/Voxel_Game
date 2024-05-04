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
    private byte[,] temperatureMap = new byte[temperatureMapSize, temperatureMapSize]; //64チャンク毎
    private byte[,] precipitationMap = new byte[precipitationMapSize, precipitationMapSize];  //32チャンク毎
    private byte[,] vegetationMap = new byte[vegetationMapSize, vegetationMapSize];  //16チャンク毎
    private byte[,] continentalnessMap = new byte[continentalnessMapSize, continentalnessMapSize];  //6チャンク毎

    private BiomeAttributes[] biome;
    private OceanBiomeAttributes[] oceanBiome;
    [SerializeField] private int[] tempetatureMapDebug;
    [SerializeField] private int[] precipitationMapDebug;
    [SerializeField] private int[] vegetationMapDebug;
    [SerializeField] private int[] continentalnessMapDebug;

    //biome[]に対応した陸地レベルを生成
    //(256- 64 + 64 / 2=)160以上で最高レベル
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
    /// 気温レベル
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0～5の整数値
    /// <para>
    /// 0から順に 13% 18% 27% 23% 12% 7%
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
    /// 降水量レベル
    /// </summary>
    /// <param name="noise"></param>
    /// <returns>
    /// 0～4の整数値
    /// <para>
    /// 0から順に 22% 29% 27% 17% 5%
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
    /// 気温レベル
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0～5の整数値
    /// </returns>
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
    /// 降水量レベル
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0～4の整数値
    /// </returns>
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
    /// 植生レベル
    /// </summary>
    /// <param name="coord"></param>
    /// <returns>
    /// 0～4の整数値
    /// </returns>
    private byte GetVegetationLv(ChunkCoord coord)
    {
        int x = Mathf.FloorToInt(coord.x / 16);
        int z = Mathf.FloorToInt(coord.z / 16);
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
                continentalnessMapDebug[continentalnessMap[i, j]]++;
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
        int x = Mathf.FloorToInt(coord.x / 6);
        int z = Mathf.FloorToInt(coord.z / 6);
        return continentalnessMap[x, z];
    }
}
