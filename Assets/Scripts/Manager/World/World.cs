using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConfigManager;

public class World : Singleton<World>
{
    public GenerateMap generateMap = new GenerateMap();
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

    private Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    private bool applyingModifications = false;
    private bool checkView = false;

    public static int WorldSizeInVoxels
    {
        get { return WorldSizeInChunks * VoxelData.Width; }
    }

    private void Awake()
    {
        Init();
    }

    public void DoStart()
    {
        generateMap.Init();

        seed = ConfigManager.seed;

        GenerateWorld();

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
        int spawmXZ = WorldSizeInVoxels / 2;
        ChunkCoord chunk = new ChunkCoord(new Vector3(spawmXZ, 0, spawmXZ));
        Vector2 voxelPos = chunks[chunk.x, chunk.z].GetVoxelPos(new Vector2(spawmXZ, spawmXZ));
        int biomeType = generateMap.GetBiomeType(chunk, voxelPos).Item1;
        float spawnY = generateMap.GetSolidGroundHight(chunk, biomeType);
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

        if (modifications.Count > 0 && !applyingModifications)
            StartCoroutine(ApplyModifications());

        if (chunksToCreate.Count > 0 && !checkView)
            CreateChunk();

        if(chunksToUpdate.Count > 0)
            UpdateChunks();
    }
    private void GenerateWorld()
    {
        for (int x = (WorldSizeInChunks / 2) - ViewDistanceInChunk; x < (WorldSizeInChunks / 2) + ViewDistanceInChunk; x++)
            for (int z = (WorldSizeInChunks / 2) - ViewDistanceInChunk; z < (WorldSizeInChunks / 2) + ViewDistanceInChunk; z++)
            {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), true);
                activeChunk.Add(new ChunkCoord(x, z));
            }



        while (modifications.Count > 0)
        {

            VoxelMod v = modifications.Dequeue();

            ChunkCoord c = GetChunkCoordFromVector3(v.position);

            if (chunks[c.x, c.z] == null)
            {
                chunks[c.x, c.z] = new Chunk(c, true);
                activeChunk.Add(c);
            }

            chunks[c.x, c.z].modifications.Enqueue(v);

            if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
                chunksToUpdate.Add(chunks[c.x, c.z]);

        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[0].UpdateChunk();
            chunksToUpdate.RemoveAt(0);

        }
    }

    private void CreateChunk()
    {
        ChunkCoord c = chunksToCreate[0];
        chunksToCreate.RemoveAt(0);
        activeChunk.Add(c);
        chunks[c.x, c.z].Init();
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

    IEnumerator ApplyModifications()
    {
        applyingModifications = true;
        int count = 0;

        while (modifications.Count > 0)
        {

            VoxelMod v = modifications.Dequeue();
            ChunkCoord c = GetChunkCoordFromVector3(v.position);

            if (!IsChunkInWorld(new ChunkCoord(c.x, c.z)))
            {
                applyingModifications = false;
                yield break;
            }

            if (chunks[c.x, c.z] == null)
            {
                chunks[c.x, c.z] = new Chunk(c, true);
                activeChunk.Add(c);
            }

            chunks[c.x, c.z].modifications.Enqueue(v);

            if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
            {
                chunksToUpdate.Add(chunks[c.x, c.z]);
            }

            count++;
            if (count > 20)
            {
                count = 0;
                yield return null;
            }
        }

        applyingModifications = false;
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

            yield return 2;
        }

        //以前のリストに残っているチャンクは視界内にないから、ループをthroughして無効に
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;

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

        return false;
    }

    public bool CheckIfVoxelTransparent(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isTransparent;

        return false;
    }

    public int CheckIfVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return (int)EnumGameData.BlockID.air;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos);

        return 0;
    }

    public string CheckForBlockType(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return "null";

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].name;

        return "null";
    }

    public float BlockNeedDestructionTime(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return 0;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].destructionTime;

        return 0;
    }

    public int BlockNeedRarity(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return -1;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].needRarity;

        return -1;
    }
    public EnumGameData.ItemType BlockEfficientTool(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return EnumGameData.ItemType.Null;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].efficientTool;

        return EnumGameData.ItemType.Null;
    }

    //チャンクの初期生成用
    public int GetVoxel(Vector3 voxelPos, int biomeType, int terrainHeight)
    {
        voxelPos.y = Mathf.FloorToInt(voxelPos.y);

        int voxelValue;

        voxelValue = PopulateVoxel(voxelPos, terrainHeight, biomeType);
        if (voxelPos.y == terrainHeight)
        {
        }

        if (voxelValue == (int)EnumGameData.BlockID.stone)
        {
        }

        return voxelValue;
    }

    private int PopulateVoxel(Vector3 pos, int terrainHight, int biomeType)
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

        if (pos.y == terrainHight)
            voxelValue = (int)biome[biomeType].topBlocks;
        else if (pos.y < terrainHight && pos.y > terrainHight - 4)
            voxelValue = (int)biome[biomeType].middleLayer;
        else if (pos.y > terrainHight)
            return (int)EnumGameData.BlockID.air;
        else
            voxelValue = (int)biome[biomeType].basicsBlocks;

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
