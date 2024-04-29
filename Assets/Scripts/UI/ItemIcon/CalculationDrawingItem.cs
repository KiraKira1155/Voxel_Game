using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationDrawingItem
{
    public void ItemUI(MeshFilter meshFilter)
    {
        Vector2[] uvs = new Vector2[] 
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
        };

        int[] triangles = new int[] { };
        Mesh mesh = new Mesh();

        // 頂点の座標
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0),
        };

        // 頂点のインデックス
        triangles = new int[]
        {
            0, 1, 2,
            2, 3, 0,
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }
}
