using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static ConfigManager;

public class WorldManager : Singleton<WorldManager>
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
    private List<ChunkCoord> chunksToHidden = new List<ChunkCoord>();

    private bool checkCreate = false;
    private bool checkHidden = false;

    public bool successGenerateWorld = false;

    WaitForSeconds createChunkWait = new WaitForSeconds(0.048f);
    WaitForSeconds hiddenChunkWait = new WaitForSeconds(0.016f);

    Queue<VoxelMod> modifications = new Queue<VoxelMod>();
    bool applyingModifications = false;

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

        playerChunkCoord = GetChunkCoordFromVector3(PlayerManager.I.transform.position);
        playerLastChunkCoord = playerChunkCoord;
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
        if (!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();

        if (chunksToCreate.Count > 0 && !checkCreate)
            StartCoroutine(CreateChunk());

        if (chunksToHidden.Count > 0 && !checkHidden)
            StartCoroutine(HiddenChunk());

        if (modifications.Count > 0 && !applyingModifications)
            StartCoroutine(ApplyModifications());
    }

    public void DoFixedUpdate()
    {
    }

    IEnumerator ApplyModifications()
    {
        applyingModifications = true;
        int cnt = 0;
        Queue<VoxelMod> queue = new Queue<VoxelMod>();

        while (modifications.Count > 0)
        {
            VoxelMod v = modifications.Dequeue();
            ChunkCoord c = GetChunkCoordFromVector3(v.position);

            if (chunks[c.x, c.z] == null)
            {
                queue.Enqueue(v);
            }
            else
            {
                chunks[c.x, c.z].modifications.Enqueue(v);
            }

            cnt++;
            if (cnt > 200)
            {
                cnt = 0;
                yield return null;
            }
        }

        while(queue.Count > 0)
        {
            modifications.Enqueue(queue.Dequeue());
        }

        applyingModifications = false;
    }

    IEnumerator GenerateWorld()
    {
        for (int x = (WorldSizeInChunks / 2) - ViewDistanceInChunk; x < (WorldSizeInChunks / 2) + ViewDistanceInChunk; x++)
        {
            for (int z = (WorldSizeInChunks / 2) - ViewDistanceInChunk; z < (WorldSizeInChunks / 2) + ViewDistanceInChunk; z++)
            {
                if (chunks[x, z] == null)
                    chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                if (!chunks[x, z].isInit)
                    chunks[x, z].Init();
                activeChunk.Add(new ChunkCoord(x, z));

                yield return null;
            }
        }

        Queue<VoxelMod> queue = new Queue<VoxelMod>();
        int cnt = 0;

        while (modifications.Count > 0)
        {
            VoxelMod v = modifications.Dequeue();
            ChunkCoord c = GetChunkCoordFromVector3(v.position);

            if (chunks[c.x, c.z] == null)
            {
                queue.Enqueue(v);
            }
            else
            {
                chunks[c.x, c.z].modifications.Enqueue(v);
            }

            cnt++;
            if(cnt >= 500)
            {
                cnt = 0;
                yield return null;
            }
        }

        while (queue.Count > 0)
        {
            modifications.Enqueue(queue.Dequeue());
        }

        for (int x = (WorldSizeInChunks / 2) - ViewDistanceInChunk; x < (WorldSizeInChunks / 2) + ViewDistanceInChunk; x++)
        {
            for (int z = (WorldSizeInChunks / 2) - ViewDistanceInChunk; z < (WorldSizeInChunks / 2) + ViewDistanceInChunk; z++)
            {
                chunks[x, z].CheckModifications();
            }
            yield return null;
        }
        successGenerateWorld = true;

        yield break;
    }

    IEnumerator CreateChunk()
    {
        checkCreate = true;

        while (chunksToCreate.Count > 0)
        {
            ChunkCoord c = chunksToCreate[0];
            chunksToCreate.RemoveAt(0);
            activeChunk.Add(c);
            if (!chunks[c.x, c.z].isInit)
            {
                chunks[c.x, c.z].Init();

                yield return createChunkWait;
            }
        }

        checkCreate = false;

        StartCoroutine(CheckView());

        yield break;
    }

    IEnumerator HiddenChunk()
    {
        checkHidden = true;


        while (chunksToHidden.Count > 0)
        {
            ChunkCoord c = chunksToHidden[0];
            chunksToHidden.RemoveAt(0);
            chunks[c.x, c.z].isActive = false;
            yield return hiddenChunkWait;
        }

        checkHidden = false;

        yield break;
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
    IEnumerator CheckView()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(PlayerManager.I.transform.position);

        for (int x = coord.x - ViewDistanceInChunk; x < coord.x + ViewDistanceInChunk; x++)
        {
            for (int z = coord.z - ViewDistanceInChunk; z < coord.z + ViewDistanceInChunk; z++)
            {
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    if (!chunks[x, z].isActive && chunks[x, z].isInit)
                        chunks[x, z].isActive = true;
                    chunks[x, z].CheckModifications();
                }
            }
            yield return null;
        }
    }

    //プレイヤーの確認できるチャンクの描画
    private void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(PlayerManager.I.transform.position);
        playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunk);
        activeChunk.Clear();

        //プレイヤーの視界内にあるチャンクをループ
        for (int x = coord.x - ViewDistanceInChunk; x < coord.x + ViewDistanceInChunk; x++)
        {
            for (int z = coord.z - ViewDistanceInChunk; z < coord.z + ViewDistanceInChunk; z++)
            {
                var task = Task.Run(() =>
                {
                    // 以前にアクティブだったチャンクを調べて、このチャンクが存在するかどうかを確認。存在する場合は、リストから削除。
                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                        if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                            previouslyActiveChunks.RemoveAt(i);
                });

                //現在のチャンクがワールド内にある場合
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    //アクティブの確認と、アクティブでない場合はアクティブに
                    if (chunks[x, z] == null)
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                    
                    if (!chunks[x, z].isInit)
                        chunksToCreate.Add(new ChunkCoord(x, z));

                    if (!chunks[x, z].isActive && chunks[x, z].isInit)
                        chunks[x, z].isActive = true;
                }

                task.Wait();
                activeChunk.Add(new ChunkCoord(x, z));
            }
        }

        //以前のリストに残っているチャンクは視界内にないから、ループをthroughして無効に
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunksToHidden.Add(c);
    }

    public Vector3 CheckVoxelPos(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        return chunks[thisChunk.x, thisChunk.z].GetVoxelPos(pos);
    }

    /// <summary>
    /// プレイヤーの当たり判定用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool CheckForVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.GetBlockData(chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)).IsSolid();

        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return BlockManager.I.GetBlockData(chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)).IsSolid();
    }

    public bool CheckIfVoxelTransparent(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.GetBlockData(chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)).IsTransparent();

        if (chunks[thisChunk.x, thisChunk.z] == null)
        {
            chunks[thisChunk.x, thisChunk.z] = new Chunk(thisChunk);
        }
        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return BlockManager.I.GetBlockData(chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)).IsTransparent();
    }
    public bool CheckIfVoxelDisplay(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.GetBlockData(chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)).IsDisplay();

        if (chunks[thisChunk.x, thisChunk.z] == null)
        {
            chunks[thisChunk.x, thisChunk.z] = new Chunk(thisChunk);
        }
        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return BlockManager.I.GetBlockData(chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)).IsDisplay();
    }

    public int CheckForBlockID(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return (int)EnumGameData.BlockID.air;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos);

        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos);
    }

    /// <summary>
    /// チャンクの初期生成用
    /// </summary>
    /// <param name="voxelPos"></param>
    /// <param name="biomeType"></param>
    /// <param name="ocean"></param>
    /// <param name="terrainHeight"></param>
    /// <returns></returns>
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
            else if(pos.y > 64)
                return (int)EnumGameData.BlockID.air;
            else
                voxelValue = (int)EnumGameData.BlockID.stone;
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

        if (voxelValue == (int)EnumGameData.BlockID.grass)
            PopulatedTree(pos);

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

    public void PopulatedTree(Vector3 pos)
    {
        /* TREE PASS */
        if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), seed, 1.3f) > 0.6f)
        {
            if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), seed, 15f) > 0.8f)
            {
                Structure.MakeTree(modifications, seed, pos, 5, 12);
            }
        }

        if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), seed, 0.5f) > 0.7f)
        {
            if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), seed, 5f) > 0.9f)
            {
                Structure.MakeAncientTree(modifications, seed, pos, 30, 40);
            }
        }
    }

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
