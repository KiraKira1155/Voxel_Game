using System.Collections;
using System.Collections.Generic;
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
    private List<ChunkCoord> chunksToHidden = new List<ChunkCoord>();

    private bool checkCreate = false;
    private bool checkHidden = false;

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

        //�v���C���[���`�����N���ړ������ꍇ�̂ݍX�V
        if (!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();

        if (chunksToCreate.Count > 0 && !checkCreate)
            StartCoroutine(CreateChunk());

        if (chunksToHidden.Count > 0 && !checkHidden)
            StartCoroutine(HiddenChunk());
    }

    public void DoFixedUpdate()
    {
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

        successGenerateWorld = true;

        yield break;
    }

    IEnumerator CreateChunk()
    {
        checkCreate = true;

        var wait = new WaitForSeconds(0.048f);

        while (chunksToCreate.Count > 0)
        {
            ChunkCoord c = chunksToCreate[0];
            chunksToCreate.RemoveAt(0);
            activeChunk.Add(c);
            if (!chunks[c.x, c.z].isInit)
            {
                chunks[c.x, c.z].Init();
                yield return wait;
            }
        }

        checkCreate = false;

        yield break;
    }

    IEnumerator HiddenChunk()
    {
        checkHidden = true;

        var wait = new WaitForSeconds(0.016f);

        while (chunksToHidden.Count > 0)
        {
            ChunkCoord c = chunksToHidden[0];
            chunksToHidden.RemoveAt(0);
            chunks[c.x, c.z].isActive = false;
            yield return wait;
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

    //�`�悳��Ă��邩�̃`�F�b�N
    IEnumerator CheckView()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(PlayerManager.I.transform.position);

        for (int x = coord.x - ViewDistanceInChunk; x < coord.x + ViewDistanceInChunk; x++)
        {
            for (int z = coord.z - ViewDistanceInChunk; z < coord.z + ViewDistanceInChunk; z++)
            {
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    if (!chunks[x, z].isActive)
                        chunks[x, z].isActive = true;
                    if (chunks[x, z] == null)
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                        chunksToCreate.Add(new ChunkCoord(x, z));
                        chunks[x, z].isActive = true;
                        yield return null;
                    }
                }
            }
        }
    }

    //�v���C���[�̊m�F�ł���`�����N�̕`��
    private void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(PlayerManager.I.transform.position);
        playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunk);
        activeChunk.Clear();

        //�v���C���[�̎��E���ɂ���`�����N�����[�v
        for (int x = coord.x - ViewDistanceInChunk; x < coord.x + ViewDistanceInChunk; x++)
        {
            for (int z = coord.z - ViewDistanceInChunk; z < coord.z + ViewDistanceInChunk; z++)
            {
                //���݂̃`�����N�����[���h���ɂ���ꍇ
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    //�A�N�e�B�u�̊m�F�ƁA�A�N�e�B�u�łȂ��ꍇ�̓A�N�e�B�u��
                    if (chunks[x, z] == null)
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z));
                    
                    if (!chunks[x, z].isInit)
                        chunksToCreate.Add(new ChunkCoord(x, z));

                    if (!chunks[x, z].isActive && chunks[x, z].isInit)
                        chunks[x, z].isActive = true;
                }

                // �ȑO�ɃA�N�e�B�u�������`�����N�𒲂ׂāA���̃`�����N�����݂��邩�ǂ������m�F�B���݂���ꍇ�́A���X�g����폜�B
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);

                activeChunk.Add(new ChunkCoord(x, z));
            }
        }

        //�ȑO�̃��X�g�Ɏc���Ă���`�����N�͎��E���ɂȂ�����A���[�v��through���Ė�����
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunksToHidden.Add(c);
    }

    public Vector3 CheckVoxelPos(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        return chunks[thisChunk.x, thisChunk.z].GetVoxelPos(pos);
    }

    //�v���C���[�̓����蔻��p
    public bool CheckForVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;

        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;
    }

    public bool CheckIfVoxelTransparent(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return false;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isTransparent;

        if (chunks[thisChunk.x, thisChunk.z] == null)
        {
            chunks[thisChunk.x, thisChunk.z] = new Chunk(thisChunk);
        }
        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isTransparent;
    }

    public int CheckIfVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return (int)EnumGameData.BlockID.air;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos);

        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos);
    }

    public string CheckForBlockType(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return "null";

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].name;

        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].name;
    }

    public float BlockNeedDestructionTime(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return 0;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].destructionTime;

        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].destructionTime;
    }

    public int BlockNeedRarity(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return -1;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].needRarity;

        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].needRarity;
    }
    public EnumGameData.ItemType BlockEfficientTool(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.Hight)
            return EnumGameData.ItemType.Null;

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].efficientTool;

        chunks[thisChunk.x, thisChunk.z].InitVoxelMap(pos);
        return BlockManager.I.blocktype[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].efficientTool;
    }

    //�`�����N�̏��������p
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

        //���[���h�͈͊O��air��
        if (!IsVoxelInWorld(pos))
            return (int)EnumGameData.BlockID.air;

        //�ŉ��w�̃u���b�N��bedrock��
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
