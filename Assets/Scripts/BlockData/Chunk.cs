using System.Collections.Generic;
using UnityEngine;
using static VoxelData;
public class Chunk
{
	private ChunkCoord coord;

	private GameObject thisObj;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;

	private int vertexIndex = 0;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvs = new List<Vector2>();
	Material[] materials = new Material[2];
	List<int> transparentTriangles = new List<int>();

	public Vector3 position;

	public int[,,] voxelMap = new int[Width, Hight, Width];
	public int[,] biomeMap = new int[Width, Width];
    public int[,] heightMap = new int[Width, Width];

    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    private bool _isActive;
    public bool isVoxelMapPopulated { get; private set; } = false;

    public Chunk(ChunkCoord coord, bool generateOnLoad)
	{
		this.coord = coord;
        isActive = true;

        if (generateOnLoad)
			Init();
    }

	public void Init()
	{
		thisObj = new GameObject();
		meshRenderer = thisObj.AddComponent<MeshRenderer>();
		meshFilter = thisObj.AddComponent<MeshFilter>();

		materials[0] = World.I.material;
		materials[1] = World.I.transparentMaterial;
		meshRenderer.materials = materials;

		thisObj.transform.SetParent(World.I.transform);
		thisObj.transform.position = new Vector3(coord.x * Width, 0f, coord.z * Width);
		thisObj.name = "Chunk" + coord.x + ", " + coord.z;

		position = thisObj.transform.position;

        PopulateVoxelMap();
		UpdateChunk();
	}

	private void PopulateVoxelMap()
    {
        for (int x = 0; x < Width; x++)
            for (int z = 0; z < Width; z++)
            {
				biomeMap[x, z] = World.I.generateMap.GetBiomeType(coord, new Vector2(x, z)).Item1;
				heightMap[x, z] = World.I.generateMap.GetSolidGroundHight(coord, biomeMap[x, z]);
            }

        for (int y = 0; y < Hight; y++)
			for (int x = 0; x < Width; x++)
				for (int z = 0; z < Width; z++)
				{
					voxelMap[x, y, z] = World.I.GetVoxel(new Vector3(x, y, z) + position, biomeMap[x, z], heightMap[x, z]);
				}

		isVoxelMapPopulated = true;
	}

    public void UpdateChunk()
    {
        while (modifications.Count > 0)
		{
			VoxelMod v = modifications.Dequeue();
			Vector3 pos = v.position - position;
			voxelMap[(int)pos.x, (int)pos.y, (int)pos.z] = (int)v.blockID;
        }

        ClearMeshData();

        for (int y = 0; y < Hight; y++)
			for (int x = 0; x < Width; x++)
				for (int z = 0; z < Width; z++)
                {
                    if (BlockManager.I.blocktype[voxelMap[x, y, z]].isSolid)
						UpdateMeshData(new Vector3(x, y, z));
				}

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
				thisObj.SetActive(value);
		}
    }

	private bool IsVoxelInChunk(int x, int y,int z)
    {
		if (x < 0 || x > Width - 1 || y < 0 || y > Hight - 1 || z < 0 || z > Width - 1)
			return false;
		else
			return true;

	}

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

	private void UpdateSurroundingVoxels(int x, int y, int z)
    {
		Vector3 thisVoxel = new Vector3(x, y, z);

		for (int p = 0; p < 6; p++)
		{
			Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {
                World.I.GetChunkFromVector3(currentVoxel + position).UpdateChunk();
            }
		}
	}

	private bool CheckVoxelTransParent(Vector3 pos)
	{

		int x = Mathf.FloorToInt(pos.x);
		int y = Mathf.FloorToInt(pos.y);
		int z = Mathf.FloorToInt(pos.z);

		if(!IsVoxelInChunk(x, y, z))
			return World.I.CheckIfVoxelTransparent(pos + position);

		return BlockManager.I.blocktype[voxelMap[x, y, z]].isTransparent;
	}

    private int CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return World.I.CheckIfVoxel(pos + position);

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

    private void UpdateMeshData(Vector3 pos)
	{
		int blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
		bool isTransparent = BlockManager.I.blocktype[blockID].isTransparent;

		for (int p = 0; p < 6; p++)
		{
			if (CheckVoxelTransParent(pos + VoxelData.faceChecks[p]) && !CheckVoxelTransParent(pos) 
				|| CheckVoxel(pos + VoxelData.faceChecks[p]) == (int)EnumGameData.BlockID.air)
			{

				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
				vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

				AddTexture(BlockManager.I.blocktype[blockID].GetTextureFace((BlockType.faceIndex)p));

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

	public void CreateMesh()
	{
		Mesh mesh = new Mesh();
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
