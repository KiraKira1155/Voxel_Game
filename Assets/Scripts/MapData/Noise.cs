using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float Get2DPerlin(Vector2 position, float offset, float scale)
    {
        return Mathf.PerlinNoise((position.x + 0.1f) / VoxelData.Width * scale + offset, (position.y + 0.1f) / VoxelData.Width * scale + offset);
    }

    public static bool Get3DPerlin(Vector3 position, float seed, float scale, float threshold)
    {
        scale = 1 - scale;
        // https://www.youtube.com/watch?v=Aga0TBJkchM Carpilot on YouTube

        float x = (position.x + seed + 0.1f) * scale;
        float y = (position.y + seed + 0.1f) * scale;
        float z = (position.z + seed + 0.1f) * scale;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);
        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        if ((AB + BC + AC + BA + CB + CA) / 6f > threshold)
            return true;
        else
            return false;

    }
}
