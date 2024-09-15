using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
public class DebugScreenManager : Singleton<DebugScreenManager>
{
    private TextMeshPro text;

    private float frameRate;
    private float timer;

    private int worldSizeVoxels;
    private int worldSizeChunks;

    [SerializeField] private Transform highlightBlock; 
    StringBuilder debugText = new StringBuilder(20);

    private void Awake()
    {
        Init();
    }


    public void DoAwake()
    {
        text = GetComponent<TextMeshPro>();
        worldSizeVoxels = VoxelData.WorldSizeInVoxels / 2;
        worldSizeChunks = ConfigManager.WorldSizeInChunks / 2;
    }

    public void DoUpdate()
    {
        DisplayInOrder();
    }

    private void DisplayInOrder()
    {
        debugText.Clear();

        debugText.Append(DebugFPS());
        debugText.Append(NewLine());
        debugText.Append(DebugSeed());
        debugText.Append(NewLine());
        debugText.Append(DebugPlayerPos());
        debugText.Append(NewLine());
        debugText.Append(DebugChunkCoord());
        debugText.Append(NewLine());
        debugText.Append(DebugBlockType());
        debugText.Append(NewLine());
        debugText.Append(DebugBiome());
        text.text = debugText.ToString();
    }

    private string DebugBlockType()
    {
        string blockName = "null";
        if (highlightBlock.gameObject.activeSelf)
        {
            blockName = BlockManager.I.blocktype[World.I.CheckForBlockID(highlightBlock.transform.position)].name;
        }
        return
        string.Format("BlockName: {0}", blockName.ToString());
    }

    private string DebugFPS()
    {
        if (timer > 0.25f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }

        return
        string.Format("{0}fps", frameRate.ToString());
    }

    private string DebugPlayerPos()
    {
        Vector3 playerPos = PlayerManager.I.transform.position;
        return
        string.Format("X: {0,-15:0.0000}\nY: {1,-15:0.0000}\nZ: {2,-15:0.0000}"
            , (playerPos.x - worldSizeVoxels).ToString(), playerPos.y.ToString(), (playerPos.z - worldSizeVoxels).ToString());
    }

    private string DebugChunkCoord()
    {
        Vector3 playerPos = PlayerManager.I.transform.position;
        ChunkCoord playerCoord = new ChunkCoord(playerPos);
        return
        string.Format("ChunkCoordX: {0}\nChunkCoordZ: {1}"
            , (playerCoord.x - worldSizeChunks).ToString(), (playerCoord.z - worldSizeChunks).ToString());
    }

    private string DebugBiome()
    {
        Vector3 playerPos = PlayerManager.I.transform.position;
        ChunkCoord playerCoord = new ChunkCoord(playerPos);
        return
        string.Format("Biome: {0}"
            , GenerateMap.I.GetBiomeType(playerCoord).ToString());
    }

    private string DebugSeed()
    {
        return
        string.Format("Seed: {0}"
            , ConfigManager.thisWorldSeed.ToString());
    }

    private string NewLine()
    {
        return
        string.Format("\n");
    }
}
