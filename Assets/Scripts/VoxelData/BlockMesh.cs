using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMesh
{
    private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private int vertexIndex = 0;

	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
    List<int> transparentTriangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    Material[] materials = new Material[2];

    public void Init(GameObject thisObj, EnumGameData.BlockID blockID)
	{
        meshRenderer = thisObj.AddComponent<MeshRenderer>();
		meshFilter = thisObj.AddComponent<MeshFilter>();

        materials[0] = WorldManager.I.material;
        materials[1] = WorldManager.I.transparentMaterial;
        meshRenderer.materials = materials;

        UpdateMeshData(thisObj.transform.position, (int)blockID);
	}
	private void CreateMesh()
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
	private void UpdateMeshData(Vector3 pos, int blockID)
    {
        bool isTransparent = BlockManager.I.GetBlockData(blockID).IsTransparent();
        for (int p = 0; p < 6; p++)
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

		CreateMesh();
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
