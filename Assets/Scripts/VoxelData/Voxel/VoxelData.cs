using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public static class VoxelData
{
	public static readonly short Width = 16;
	public static readonly short Hight = 260;
	public static readonly byte TextureAtlasSizeInBlocks = 16;
	public static int WorldSizeInVoxels
	{
		get { return ConfigManager.WorldSizeInChunks * Width; }

	}
	public static float NormalizedBlockTextureSize
	{
		get { return 1f / (float)TextureAtlasSizeInBlocks; }
	}

    public static readonly Vector3[] voxelVerts = new Vector3[8] {

		new Vector3(0.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 1.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 1.0f),

	};

	public static readonly Vector3[] faceChecks = new Vector3[6] {

		new Vector3(0.0f, 0.0f, -1.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
		new Vector3(0.0f, -1.0f, 0.0f),
		new Vector3(-1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f)

	};

	public static readonly int[,] voxelTris = new int[6, 4] {

		{0, 3, 1, 2}, // Back Face
		{5, 6, 4, 7}, // Front Face
		{3, 7, 2, 6}, // Top Face
		{1, 5, 0, 4}, // Bottom Face
		{4, 7, 0, 3}, // Left Face
		{1, 2, 5, 6} // Right Face

	};

	public static readonly Vector2[] voxelUvs = new Vector2[4] {

		new Vector2 (0.0f, 0.0f),
		new Vector2 (0.0f, 1.0f),
		new Vector2 (1.0f, 0.0f),
		new Vector2 (1.0f, 1.0f)
	};
}

public class VoxelMod
{
    public Vector3 position;
    public EnumGameData.BlockID blockID;

    public VoxelMod()
    {
        position = new Vector3();
        blockID = EnumGameData.BlockID.air;
    }

    public VoxelMod(Vector3 position, EnumGameData.BlockID blockID)
    {
        this.position = position;
        this.blockID = blockID;
    }
}
