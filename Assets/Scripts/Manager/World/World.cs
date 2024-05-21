using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static ConfigManager;

public class World : Singleton<World>
{
    public float seed { get; private set; }
    public BiomeAttributes[] biome;
    public OceanBiomeAttributes[] oceanBiome;

    [SerializeField] private Vector3 spawnPosition;

    public Material material;
    public Material transparentMaterial;

    private Chunk[,] chunks = new Chunk[WorldSizeInChunks, WorldSizeInChunks];

    private List<ChunkCoord> activeChunk = new List<ChunkCoord>();
    private ChunkCoord playerChunkCoord;
    private ChunkCoord playerLastChunkCoord;

    private List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    private List<Chunk> chunksToUpdate = new List<Chunk>();

    private bool checkView = false;
    private bool checkCreate = false;

    public bool successGenerateWorld = false;

    public static int WorldSizeInVoxels
    {
        get { return WorldSizeInChunks * VoxelData.Width; }
    }

    private void Awake()
    {
        Init();
    }

    public void DoAwake()
    {
        seed = ConfigManager.seed;
        StartCoroutine(GenerateWorld());
    }

    public void DoStart()
    {
        Vector3 player = SpawnPositionGenerate(); ;
        for (int i = 0; i < 30 ;i++)
        {
            if (
                !CheckForVoxel(player + new Vector3(0, i - 1, 0))
                && !CheckForVoxel(player + new Vector3(0, i - 2, 0))
                && !CheckForVoxel(player + new Vector3(0, i - 3, 0))
                && !CheckForVoxel(player + new Vector3(0, i, 0))
                && !CheckForVoxel(player + new Vector3(0, i + 1, 0))
                && !CheckForVoxel(player + new Vector3(0, i + 2, 0))
                && !CheckForVoxel(player + new Vector3(0, i + 3, 0))
                && !CheckForVoxel(player + new Vector3(0, i + 4, 0))
                )
            {
                player.y += i + 4;
                break;
            }
        }

        PlayerManager.I.transform.position = player;

        playerLastChunkCoord = GetChunkCoordFromVector3(PlayerManager.I.transform.position);
    }

    private Vector3 SpawnPositionGenerate()
    {
        int spawmXZ = WorldSizeInVoxels / 2 + 8;
        ChunkCoord chunk = new ChunkCoord(new Vector3(spawmXZ, 0, spawmXZ));
        byte biomeType = GenerateMap.I.GetBiomeType(chunk).Item1;
        bool ocean = GenerateMap.I.GetBiomeType(chunk).Item2;
        float spawnY;
        if (ocean)
            spawnY = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(spawmXZ, spawmXZ), biomeType);
        else
            spawnY = GenerateMap.I.GetSolidGroundHight(new Vector2(spawmXZ, spawmXZ), biomeType);
        return new Vector3(spawmXZ, spawnY, spawmXZ);
    }

    public void DoUpdate()
    {
        playerChunkCoord = GetChunkCoordFromVector3(PlayerManager.I.transform.position);

        //プレイヤーがチャンクを移動した場合のみ更新
        if (!playerChunkCoord.Equals(playerLastChunkCoord) && !checkView)
            StartCoroutine(CheckViewDistance());
        
        if (!checkView)
            CheckView();

        if (chunksToCreate.Count > 0 && !checkView && !checkCreate)
            StartCoroutine(CreateChunk());

        if (chunksToUpdate.Count > 0)
            UpdateChunks();
    }
    IEnumerator GenerateWorld()
    {
        for (int x = (WorldSizeInChunks / 2) - ViewDistanceInChunk; x < (WorldSizeInChunks / 2) + ViewDistanceInChunk; x++)
        {
            for (int z = (WorldSizeInChunks / 2) - ViewDistanceInChunk; z < (WorldSizeInChunks / 2) + ViewDistanceInChunk; z++)
            {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), true);
                activeChunk.Add(new ChunkCoord(x, z));
                Debug.Log("ワールド生成" + x);
                yield return null;
            }
        }

        Debug.Log("成功");
        successGenerateWorld = true;
    }

    IEnumerator CreateChunk()
    {
        checkCreate = true;

        while(chunksToCreate.Count > 0)
        {
            ChunkCoord c = chunksToCreate[0];
            chunksToCreate.RemoveAt(0); 
            chunks[c.x, c.z].Init();
            activeChunk.Add(c);
            yield return new WaitForSeconds(0.032f);
        }

        checkCreate = false;
    }

    private void UpdateChunks()
    {
        bool update = false;
        int index = 0;

        while (!update && index < chunksToUpdate.Count -1)
        {
            if (chunksToUpdate[index].isVoxelMapPopulated)
            {
                chunksToUpdate[index].UpdateChunk();
                chunksToUpdate.RemoveAt(index);
                update = true;
            }
            else
                index++;
        }
    }

    public ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.Width);
        int z = Mathf.FloorToInt(pos.z / VoxelData.Width);
        return new ChunkCoord(x, z);
    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.Width);
        int z = Mathf.FloorToInt(pos.z / VoxelData.Width);
        return chunks[x, z];
    }

    //描画されているかのチェック
    private void CheckView()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(PlayerManager.I.transform.position);

        for (int x = coord.x - ViewDistanceInChunk; x < coord.x + ViewDistanceInChunk; x++)
        {
            for (int z = coord.z - ViewDistanceInChunk; z < coord.z + ViewDistanceInChunk; z++)
            {
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }
                    if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                    }
                }
            }
        }
    }

    //プレイヤーの確認できるチャンクの描画
    IEnumerator CheckViewDistance()
    {
        checkView = true;

        ChunkCoord coord = GetChunkCoordFromVector3(PlayerManager.I.transform.position);
        playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunk);

        //プレイヤーの視界内にあるチャンクをループ
        for (int x = coord.x - ViewDistanceInChunk; x < coord.x + ViewDistanceInChunk; x++)
        {
            for (int z = coord.z - ViewDistanceInChunk; z < coord.z + ViewDistanceInChunk; z++)
            {
                //現在のチャンクがワールド内にある場合
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    //アクティブの確認と、アクティブでない場合はアクティブに
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }
                    if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        activeChunk.Add(new ChunkCoord(x, z));
                    }
                }

                // 以前にアクティブだったチャンクを調べて、このチャンクが存在するかどうかを確認します。存在する場合は、リストから削除します。
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);
            }

            yield return null;
        }

        //以前のリストに残っているチャンクは視界内にないから、ループをthroughして無効に
        foreach (ChunkCoord c in previouslyActiveChunks)
        {
            chunks[c.x, c.z].isActive = false;
            yield return null;
        }

        checkView = false;
    }

    //プレイヤーの当たり判定用
    public bool CheckForVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;

        (byte, bool) biome = GenerateMap.I.GetBiomeType(thisChunk);
        int height;
        if (biome.Item2)
            height = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(pos.x, pos.z), biome.Item1);
        else
            height = GenerateMap.I.GetSolidGroundHight(new Vector2(pos.x, pos.z), biome.Item1);

        return BlockManager.I.blocktype[GetVoxel(pos, biome.Item1, biome.Item2, height)].isSolid;
    }

    public bool CheckIfVoxelTransparent(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isTransparent;

        (byte, bool) biome = GenerateMap.I.GetBiomeType(thisChunk);
        int height;
        if (biome.Item2)
            height = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(pos.x, pos.z), biome.Item1);
        else
            height = GenerateMap.I.GetSolidGroundHight(new Vector2(pos.x, pos.z), biome.Item1);

        return BlockManager.I.blocktype[GetVoxel(pos, biome.Item1, biome.Item2, height)].isTransparent;
    }

    public int CheckIfVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return (int)EnumGameData.BlockID.air;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos);

        (byte, bool) biome = GenerateMap.I.GetBiomeType(thisChunk);
        int height;
        if (biome.Item2)
            height = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(pos.x, pos.z), biome.Item1);
        else
            height = GenerateMap.I.GetSolidGroundHight(new Vector2(pos.x, pos.z), biome.Item1);

        return GetVoxel(pos, biome.Item1, biome.Item2, height);
    }

    public string CheckForBlockType(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return "null";

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].name;

        (byte, bool) biome = GenerateMap.I.GetBiomeType(thisChunk);
        int height;
        if (biome.Item2)
            height = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(pos.x, pos.z), biome.Item1);
        else
            height = GenerateMap.I.GetSolidGroundHight(new Vector2(pos.x, pos.z), biome.Item1);

        return BlockManager.I.blocktype[GetVoxel(pos, biome.Item1, biome.Item2, height)].name;
    }

    public float BlockNeedDestructionTime(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return 0;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].destructionTime;

        (byte, bool) biome = GenerateMap.I.GetBiomeType(thisChunk);
        int height;
        if (biome.Item2)
            height = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(pos.x, pos.z), biome.Item1);
        else
            height = GenerateMap.I.GetSolidGroundHight(new Vector2(pos.x, pos.z), biome.Item1);

        return BlockManager.I.blocktype[GetVoxel(pos, biome.Item1, biome.Item2, height)].destructionTime;
    }

    public int BlockNeedRarity(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return -1;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].needRarity;

        (byte, bool) biome = GenerateMap.I.GetBiomeType(thisChunk);
        int height;
        if (biome.Item2)
            height = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(pos.x, pos.z), biome.Item1);
        else
            height = GenerateMap.I.GetSolidGroundHight(new Vector2(pos.x, pos.z), biome.Item1);

        return BlockManager.I.blocktype[GetVoxel(pos, biome.Item1, biome.Item2, height)].needRarity;
    }
    public EnumGameData.ItemType BlockEfficientTool(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return EnumGameData.ItemType.Null;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].efficientTool;

        (byte, bool) biome = GenerateMap.I.GetBiomeType(thisChunk);
        int height;
        if (biome.Item2)
            height = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(pos.x, pos.z), biome.Item1);
        else
            height = GenerateMap.I.GetSolidGroundHight(new Vector2(pos.x, pos.z), biome.Item1);

        return BlockManager.I.blocktype[GetVoxel(pos, biome.Item1, biome.Item2, height)].efficientTool;
    }

    //チャンクの初期生成用
    public int GetVoxel(Vector3 voxelPos, byte biomeType, bool ocean, int terrainHeight)
    {
        voxelPos.y = Mathf.FloorToInt(voxelPos.y);

        int voxelValue;

        voxelValue = PopulateVoxel(voxelPos, biomeType, ocean, terrainHeight);
        if (voxelPos.y == terrainHeight)
        {
        }

        if (voxelValue == (int)EnumGameData.BlockID.stone)
        {
        }

        return voxelValue;
    }

    private int PopulateVoxel(Vector3 pos, int biomeType, bool ocean, int terrainHight)
    {
        /* IMMUTABLE PASS */

        //ワールド範囲外はairに
        if (!IsVoxelInWorld(pos))
            return (int)EnumGameData.BlockID.air;

        //最下層のブロックをbedrockに
        if (pos.y == 0)
            return (int)EnumGameData.BlockID.bedrock;

        /* BASIC TERRAIN PASS */
        int voxelValue;

        if (ocean)
        {
            if (pos.y == terrainHight)
                voxelValue = (int)oceanBiome[biomeType].topBlocks;
            else if (pos.y < terrainHight && pos.y > terrainHight - 4)
                voxelValue = (int)oceanBiome[biomeType].middleLayer;
            else if (pos.y > terrainHight && pos.y <= 64)
                voxelValue = (int)EnumGameData.BlockID.glass;
            else if (pos.y > 64)
                voxelValue = (int)EnumGameData.BlockID.air;
            else
                voxelValue = (int)oceanBiome[biomeType].basicsBlocks;
        }
        else
        {
            if (pos.y == terrainHight)
                voxelValue = (int)biome[biomeType].topBlocks;
            else if (pos.y < terrainHight && pos.y > terrainHight - biome[biomeType].middleLayerWidth)
                voxelValue = (int)biome[biomeType].middleLayer;
            else if (pos.y > terrainHight)
                return (int)EnumGameData.BlockID.air;
            else
                voxelValue = (int)EnumGameData.BlockID.stone;
        }

        return voxelValue;
    }

    public int PopulatedUnderground(Vector3 pos, int voxelValue, int biomeType)
    {
        /* SECOND PASS */

        //foreach (UndergroundProducts lode in biome[biomeType].undergrounds)
        //{
        //}

        return voxelValue;
    }

    //public int PopulatedTree(Vector3 pos, int voxelValue, int biomeType)
    //{

    //    /* TREE PASS */
    //    if (Noise.Get2DPerlinNoise(new Vector2(pos.x, pos.z), seed, biome.trees.treeZoneScale) > biome.trees.treeZoneThreshold)
    //    {
    //        voxelValue = (int)EnumGameData.BlockID.bricks;
    //        if (Noise.Get2DPerlinNoise(new Vector2(pos.x, pos.z), seed, biome.trees.treePlacementScale) > biome.trees.treePlacementThreshold)
    //        {
    //            voxelValue = (int)biome.trees.lootBlock;
    //            Structure.MakeTree(pos, modifications, biome.trees.minTreeHeight, biome.trees.maxTreeHeight);
    //        }
    //    }

    //    return voxelValue;
    //}

    private bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < WorldSizeInChunks - 1 && coord.z > 0 && coord.z < WorldSizeInChunks - 1)
            return true;
        else
            return false;
    }

    private bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.Hight && pos.z >= 0 && pos.z < WorldSizeInVoxels)
            return true;
        else
            return false;
    }
}
