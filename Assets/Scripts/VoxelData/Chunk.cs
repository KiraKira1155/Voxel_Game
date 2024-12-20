using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static VoxelData;

public class Chunk
{
	private ChunkCoord coord;

	private GameObject thisObj;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private Mesh mesh;

    private int vertexIndex = 0;
	private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> triangles = new List<int>();
    List<int> transparentTriangles = new List<int>();
    Material[] materials = new Material[2];

	public Vector3 position {  get; private set; }

	private int[,,] voxelMap = new int[Width, Hight, Width];
	//private byte[,,] blockDirectionMap = new byte[Width, Hight, Width];
    private bool[,] OceanMap = new bool[Width, Width];
    private byte[,] biomeMap = new byte[Width, Width];
	private byte BiomeNum;
	private bool IsOcean;
    private int[,] heightMap = new int[Width, Width];

	public bool isInit {  get; private set; } = false;
	private bool generateBiomeMap = false;
    private bool _isActive;
    public bool isVoxelMapPopulated { get; private set; } = false;

    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    public Chunk(ChunkCoord coord)
	{
		this.coord = coord;
        isActive = true;
        position = new Vector3(coord.x * Width, 0f, coord.z * Width);
    }

	public void Init()
    {
        var task = Task.Run(() =>
        {
            if (!generateBiomeMap)
            {
                GenerateMap.I.AddGenerateBiomeMap(coord);
				generateBiomeMap = true;
            }
            PopulateVoxelMap();
        });
		thisObj = new GameObject();
        thisObj.transform.position = new Vector3(coord.x * Width, 0f, coord.z * Width);
		mesh = new Mesh();

        meshRenderer = thisObj.AddComponent<MeshRenderer>();
		meshFilter = thisObj.AddComponent<MeshFilter>();

		materials[0] = WorldManager.I.material;
		materials[1] = WorldManager.I.transparentMaterial;
		meshRenderer.materials = materials;

		thisObj.transform.SetParent(WorldManager.I.transform);
		thisObj.name = "Chunk" + coord.x + ", " + coord.z;

        task.Wait();
        UpdateChunk();

        isInit = true;
	}

	public void InitVoxelMap(Vector3 pos)
    {
        if (!generateBiomeMap)
        {
            GenerateMap.I.AddGenerateBiomeMap(coord);
            generateBiomeMap = true;
        }

        Vector3 voxelPos = GetVoxelPos(pos);
        (BiomeNum, IsOcean) = GenerateMap.I.GetBiomeType(coord);
        (biomeMap[(int)voxelPos.x, (int)voxelPos.z], OceanMap[(int)voxelPos.x, (int)voxelPos.z]) = (BiomeNum, IsOcean);
        if (OceanMap[(int)voxelPos.x, (int)voxelPos.z])
            heightMap[(int)voxelPos.x, (int)voxelPos.z] = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(position.x + (int)voxelPos.x, position.z + (int)voxelPos.z), biomeMap[(int)voxelPos.x, (int)voxelPos.z]);
        else
            heightMap[(int)voxelPos.x, (int)voxelPos.z] = GenerateMap.I.GetSolidGroundHight(new Vector2(position.x + (int)voxelPos.x, position.z + (int)voxelPos.z), biomeMap[(int)voxelPos.x, (int)voxelPos.z]);


        voxelMap[(int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z] = WorldManager.I.GetVoxel(new Vector3((int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z) + position, biomeMap[(int)voxelPos.x, (int)voxelPos.z], OceanMap[(int)voxelPos.x, (int)voxelPos.z], heightMap[(int)voxelPos.x, (int)voxelPos.z]);
    }

    private void PopulateVoxelMap()
    {
		(BiomeNum, IsOcean) = GenerateMap.I.GetBiomeType(coord);
        for (int x = 0; x < Width; x++)
            for (int z = 0; z < Width; z++)
            {
				(biomeMap[x, z], OceanMap[x, z]) = (BiomeNum, IsOcean);
                if (OceanMap[x, z])
                    heightMap[x, z] = GenerateMap.I.GetSolidOceanGroundHight(new Vector2(position.x + x, position.z + z), biomeMap[x, z]);
                else
                    heightMap[x, z] = GenerateMap.I.GetSolidGroundHight(new Vector2(position.x + x, position.z + z), biomeMap[x, z]);
            }

        for (int y = 0; y < Hight; y++)
			for (int x = 0; x < Width; x++)
				for (int z = 0; z < Width; z++)
				{
                    voxelMap[x, y, z] = WorldManager.I.GetVoxel(new Vector3(x, y, z) + position, biomeMap[x, z], OceanMap[x, z], heightMap[x, z]);
                }

        isVoxelMapPopulated = true;
    }

	public void CheckModifications()
	{
		bool t = modifications.Count > 0;
        while (modifications.Count > 0)
        {
            VoxelMod v = modifications.Dequeue();
            Vector3 pos = v.position -= position;
            voxelMap[(int)pos.x, (int)pos.y, (int)pos.z] = (int)v.blockID;
        }

		if (t)
			UpdateChunk();
    }

    public void UpdateChunk()
    {
        mesh.Clear();

        var task = Task.Run(() =>
        {
            ClearMeshData();

            for (int y = 0; y < Hight; y++)
                for (int x = 0; x < Width; x++)
                    for (int z = 0; z < Width; z++)
                    {
						if (BlockManager.I.GetBlockData(voxelMap[x, y, z]).IsDisplay())
							UpdateMeshData(new Vector3(x, y, z));
                    }
        });

		task.Wait();
        CreateMesh();
	}

	private void ClearMeshData()
    {
		vertexIndex = 0;
		vertices.Clear();
		triangles.Clear();
        transparentTriangles.Clear();
        uvs.Clear();
    }

	public bool isActive
    {
        get { return _isActive; }
		set 
		{
			_isActive = value;
			if(thisObj != null)
                meshRenderer.enabled = value;
        }
    }

	private bool IsVoxelInChunk(int x, int y,int z)
    {
		if (x < 0 || x > Width - 1 || y < 0 || y > Hight - 1 || z < 0 || z > Width - 1)
			return false;
		else
			return true;

	}

	/// <summary>
	/// ボクセル編集用
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="newID"></param>
	public void EditVoxel(Vector3 pos, EnumGameData.BlockID newID)
	{
		int xCheck = Mathf.FloorToInt(pos.x);
		int yCheck = Mathf.FloorToInt(pos.y);
		int zCheck = Mathf.FloorToInt(pos.z);

		xCheck -= Mathf.FloorToInt(position.x);
		zCheck -= Mathf.FloorToInt(position.z);

		voxelMap[xCheck, yCheck, zCheck] = (int)newID;

		UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
		UpdateChunk();
	}

	/// <summary>
	/// 構造体用のボクセル描画、事前にvoxelMapに登録しておく
	/// </summary>
	public void UpdateStructure()
	{

	}


	private void UpdateSurroundingVoxels(int x, int y, int z)
    {
		Vector3 thisVoxel = new Vector3(x, y, z);

		for (int p = 0; p < 6; p++)
		{
			Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {
                WorldManager.I.GetChunkFromVector3(currentVoxel + position).UpdateChunk();
            }
		}
	}

	private bool CheckVoxelTransParent(Vector3 pos)
	{

		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);

		if(!IsVoxelInChunk(x, y, z))
			return WorldManager.I.CheckIfVoxelTransparent(pos + position);

		return BlockManager.I.GetBlockData(voxelMap[x, y, z]).IsTransparent();
	}
	private bool CheckVoxelDisplay(Vector3 pos)
	{

		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);

		if (!IsVoxelInChunk(x, y, z))
			return WorldManager.I.CheckIfVoxelDisplay(pos + position);

		return BlockManager.I.GetBlockData(voxelMap[x, y, z]).IsDisplay();
	}

	private int CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return WorldManager.I.CheckForBlockID(pos + position);

        return voxelMap[x, y, z];
    }

    public int GetVoxelFromGlobalVector3(Vector3 pos)
	{
		int xCheck = Mathf.FloorToInt(pos.x);
		int yCheck = Mathf.FloorToInt(pos.y);
		int zCheck = Mathf.FloorToInt(pos.z);

		xCheck -= Mathf.FloorToInt(position.x);
		zCheck -= Mathf.FloorToInt(position.z);

		return voxelMap[xCheck, yCheck, zCheck];
	}

	/// <summary>
	/// Voxel内の座標を返す
	/// </summary>
	/// <param name="pos"></param>
	/// <returns>
	/// X,Y,Z座標を返す
	/// </returns>
	public Vector3 GetVoxelPos(Vector3 pos)
	{
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(position.x);
        zCheck -= Mathf.FloorToInt(position.z);

		return new Vector3(xCheck, yCheck, zCheck);
    }

    /// <summary>
    /// Voxel内の座標を返す
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>
	/// X,Z座標を返す
	/// </returns>
    public Vector2 GetVoxelPos(Vector2 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int zCheck = Mathf.FloorToInt(pos.y);

        xCheck -= Mathf.FloorToInt(position.x);
        zCheck -= Mathf.FloorToInt(position.z);

        return new Vector2(xCheck, zCheck);
    }

	public byte GetBiomeType(Vector3 pos)
	{
		return biomeMap[(int)GetVoxelPos(pos).x, (int)GetVoxelPos(pos).z];
    }
    public bool GetOceanMap(Vector3 pos)
    {
        return OceanMap[(int)GetVoxelPos(pos).x, (int)GetVoxelPos(pos).z];
    }

	public int GetSolidGroundHeight(Vector3 pos)
	{
		return heightMap[(int)GetVoxelPos(pos).x, (int)GetVoxelPos(pos).z];

    }

    private void UpdateMeshData(Vector3 pos)
	{
		int blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
		bool isTransparent = BlockManager.I.GetBlockData(blockID).IsTransparent();

		for (int p = 0; p < 6; p++)
		{
			if ((CheckVoxelTransParent(pos + VoxelData.faceChecks[p]) && !CheckVoxelTransParent(pos))
				|| !CheckVoxelDisplay(pos + VoxelData.faceChecks[p]))
            {
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(BlockManager.I.GetBlockData(blockID).GetTextureFace((BlockManager.faceIndex)p));

                if (!isTransparent)
                {
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 3);
				}
				else
                {
                    transparentTriangles.Add(vertexIndex);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 3);
                }
				vertexIndex += 4;
			}
		}
	}

	private void CreateMesh()
	{
		mesh.vertices = vertices.ToArray();

        mesh.subMeshCount = 2;

        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(transparentTriangles.ToArray(), 1);
        mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
	}

	private void AddTexture(EnumGameData.BlockID blockId)
	{
		float y = (int)blockId / VoxelData.TextureAtlasSizeInBlocks;
		float x = (int)blockId - (y * VoxelData.TextureAtlasSizeInBlocks);

		x *= VoxelData.NormalizedBlockTextureSize;
		y *= VoxelData.NormalizedBlockTextureSize;

		y = 1f - y - VoxelData.NormalizedBlockTextureSize;

		uvs.Add(new Vector2(x, y));
		uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
		uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
		uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
	}
}

public class ChunkCoord
{
	public int x;
	public int z;

	public ChunkCoord()
	{
		x = 0;
		z = 0;
	}

	public ChunkCoord(int _x, int _z)
    {
		x = _x;
		z = _z;
	}

	public ChunkCoord(Vector3 pos)
    {
		int xCheck = Mathf.FloorToInt(pos.x);
		int zCheck = Mathf.FloorToInt(pos.z);

		x = xCheck / VoxelData.Width;
		z = zCheck / VoxelData.Width;
	} 

	public bool Equals(ChunkCoord other)
	{

		if (other == null)
			return false;
		else if (other.x == x && other.z == z)
			return true;
		else
			return false;

	}
}
