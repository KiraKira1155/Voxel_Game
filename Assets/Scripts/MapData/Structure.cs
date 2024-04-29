using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure
{
    public static void MakeTree
        (Vector3 position, Queue<VoxelMod> queue, int minHeight, int maxHeight)
    {
        int height = 0;//(int)(maxHeight * World.I.generateMap.Get2DPerlinNoise(position.x, position.z, 3f));

        if(height < minHeight)
            height = minHeight;

        for (int x = -2; x < 3; x++)
        {
            for (int z = -2; z < 3; z++)
            {
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 2, position.z + z), EnumGameData.BlockID.oakLeaf));
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 3, position.z + z), EnumGameData.BlockID.oakLeaf));
            }
        }

        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 1, position.z + z), EnumGameData.BlockID.oakLeaf));
            }
        }
        for (int x = -1; x < 2; x++)
        {
            if (x == 0)
                for (int z = -1; z < 2; z++)
                {
                    queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height, position.z + z), EnumGameData.BlockID.oakLeaf));
                }
            else
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height, position.z), EnumGameData.BlockID.oakLeaf));
        }

        for (int i = 1; i < height -1; i++)
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), EnumGameData.BlockID.oakLog));
    }
}
