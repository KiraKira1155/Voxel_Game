using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
public class DebugScreenManager : Singleton<DebugScreenManager>
{
    private TextMeshPro text;

    private const float SIMPLE_FPS_INTERVAL = 0.25f;
    private float simpleDffTime;
    private float preTime;
    private float dffTime;
    private float elapsedTime;

    private int worldSizeVoxels;
    private int worldSizeChunks;

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

        debugText.Append(DebugSimpleFPS());
        debugText.Append(NewLine());
        debugText.Append(DebugFPS());
        debugText.Append(NewLine());
        debugText.Append(DebugFrameSeconds());
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
        if (PlayerManager.I.highlightBlock.activeSelf)
        {
            blockName = BlockManager.I.GetBlockData(WorldManager.I.CheckForBlockID(PlayerManager.I.highlightBlock.transform.position)).ID().ToString();
        }
        return
        string.Format("BlockName: {0}", blockName);
    }

    private string DebugSimpleFPS()
    {
        if(elapsedTime >= SIMPLE_FPS_INTERVAL)
        {
            elapsedTime = 0.0f;
            simpleDffTime = dffTime;
        }
        return
        string.Format("{0} fps  {1} Sec  : interval {2}", (1.0f / simpleDffTime).ToString("0.00"), simpleDffTime.ToString("0.00000"), SIMPLE_FPS_INTERVAL);
    }

    private string DebugFPS()
    {
        float time = Time.realtimeSinceStartup;
        dffTime = time - preTime;

        elapsedTime += dffTime;

        preTime = time;

        return
        string.Format("{0} fps", (1.0f / dffTime).ToString("0.00"));
    }

    private string DebugFrameSeconds()
    {
        return
        string.Format("{0} Sec", dffTime.ToString("0.00000"));
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
