using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CalculationDrawingBlock
{
    public void CubeIsometricView(MeshFilter meshFilter, EnumGameData.BlockID blockID)
    {
        List<Vector2> uvs = new List<Vector2>();
        int[] triangles = new int[] { };
        int[] transparentTriangles = new int[] { };
        bool isTransparent = BlockManager.I.GetBlockData(blockID).IsTransparent();
        Mesh mesh = new Mesh();

        // 頂点の座標
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1.0f, -0.5f, 0),       //0
            new Vector3(-1.0f, 0.5f, 0),        //1
            new Vector3(0.0f, 0.0f, 0.0f),      //2
            new Vector3(0.0f, -1.0f, 0.0f),     //3

            new Vector3(0.0f, -1.0f, 0.0f),     //4
            new Vector3(0.0f, 0.0f, 0.0f),      //5
            new Vector3(1.0f, 0.5f, 0.0f),      //6
            new Vector3(1.0f, -0.5f, 0.0f),     //7

            new Vector3(-1.0f, 0.5f, 0),        //8
            new Vector3(0.0f, 1.0f, 0.0f),      //9
            new Vector3(1.0f, 0.5f, 0.0f),      //10
            new Vector3(0.0f, 0.0f, 0.0f),      //11
        };

        // 頂点のインデックス
        if (!isTransparent)
        {
            triangles = new int[]
            {
            0, 1, 2,
            2, 3, 0,
            4, 5, 6,
            6, 7, 4,
            8, 9, 10,
            10, 11, 8
            };
        }
        else
        {
            transparentTriangles = new int[]
            {
            0, 1, 2,
            2, 3, 0,
            4, 5, 6,
            6, 7, 4,
            8, 9, 10,
            10, 11, 8
            };
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        AddTexture(BlockManager.I.GetBlockData(blockID).GetTextureFace(BlockManager.faceIndex.front), uvs);
        AddTexture(BlockManager.I.GetBlockData(blockID).GetTextureFace(BlockManager.faceIndex.left), uvs);
        AddTexture(BlockManager.I.GetBlockData(blockID).GetTextureFace(BlockManager.faceIndex.top), uvs);

        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles, 0);
        mesh.SetTriangles(transparentTriangles, 1);
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private void AddTexture(EnumGameData.BlockID blockId, List<Vector2> uvs)
    {
        float y = (int)blockId / VoxelData.TextureAtlasSizeInBlocks;
        float x = (int)blockId - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
    }
}