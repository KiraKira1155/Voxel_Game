using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure
{
    public static void MakeTree(Queue<VoxelMod> queue, float seed, Vector3 position, int minHeight, int maxHeight)
    {
        int height = (int)(maxHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), seed, 3f));

        if(height < minHeight)
            height = minHeight;

        for (int x = -2; x < 3; x++)
        {
            for (int z = -2; z < 3; z++)
            {
                //chunk.EditVoxel(new Vector3(position.x + x, position.y + height - 2, position.z + z), EnumGameData.BlockID.leaf);
                //chunk.EditVoxel(new Vector3(position.x + x, position.y + height - 3, position.z + z), EnumGameData.BlockID.leaf);
                if(x == 0 && z == 0) continue;
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 2, position.z + z), EnumGameData.BlockID.leaf));
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 3, position.z + z), EnumGameData.BlockID.leaf));
            }
        }

        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                //chunk.EditVoxel(new Vector3(position.x + x, position.y + height - 1, position.z + z), EnumGameData.BlockID.leaf);
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 1, position.z + z), EnumGameData.BlockID.leaf));
            }
        }
        for (int x = -1; x < 2; x++)
        {
            if (x == 0)
                for (int z = -1; z < 2; z++)
                {
                    //chunk.EditVoxel(new Vector3(position.x + x, position.y + height, position.z + z), EnumGameData.BlockID.leaf);
                    queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height, position.z + z), EnumGameData.BlockID.leaf));
                }
            else
            {
                //chunk.EditVoxel(new Vector3(position.x + x, position.y + height, position.z), EnumGameData.BlockID.leaf);
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height, position.z), EnumGameData.BlockID.leaf));
            }
        }

        for (int i = 1; i < height -1; i++)
        {
            //chunk.EditVoxel(new Vector3(position.x, position.y + i, position.z), EnumGameData.BlockID.log);
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), EnumGameData.BlockID.log));
        }
    }



    public static void MakeAncientTree(Queue<VoxelMod> queue, float seed, Vector3 position, int minHeight, int maxHeight)
    {
        int height = (int)(maxHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), seed, 5f));

        if (height < minHeight)
            height = minHeight;

        for (int x = -5; x < 5; x++)
        {
            for (int z = -5; z < 5; z++)
            {
                //chunk.EditVoxel(new Vector3(position.x + x, position.y + height - 2, position.z + z), EnumGameData.BlockID.leaf);
                //chunk.EditVoxel(new Vector3(position.x + x, position.y + height - 3, position.z + z), EnumGameData.BlockID.leaf);
                if(x == 0 && z == 0 || x == -1 && z == -1 || x == 0 && z == -1 || x == -1 && z == 0) continue;
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 2, position.z + z), EnumGameData.BlockID.diamondBlock));
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 3, position.z + z), EnumGameData.BlockID.diamondBlock));
            }
        }

        for (int x = -2; x < 2; x++)
        {
            for (int z = -2; z < 2; z++)
            {
                //chunk.EditVoxel(new Vector3(position.x + x, position.y + height - 1, position.z + z), EnumGameData.BlockID.leaf);
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 1, position.z + z), EnumGameData.BlockID.diamondBlock));
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height, position.z + z), EnumGameData.BlockID.diamondBlock));
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height + 1, position.z + z), EnumGameData.BlockID.diamondBlock));
            }
        }

        for (int i = 1; i < height - 1; i++)
        {
            //chunk.EditVoxel(new Vector3(position.x, position.y + i, position.z), EnumGameData.BlockID.log);
            queue.Enqueue(new VoxelMod(new Vector3(position.x - 1, position.y + i, position.z), EnumGameData.BlockID.log));
        }
        for (int i = 1; i < height - 1; i++)
        {
            //chunk.EditVoxel(new Vector3(position.x, position.y + i, position.z), EnumGameData.BlockID.log);
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z - 1), EnumGameData.BlockID.log));
        }
        for (int i = 1; i < height - 1; i++)
        {
            //chunk.EditVoxel(new Vector3(position.x, position.y + i, position.z), EnumGameData.BlockID.log);
            queue.Enqueue(new VoxelMod(new Vector3(position.x - 1, position.y + i, position.z - 1), EnumGameData.BlockID.log));
        }
        for (int i = 1; i < height - 1; i++)
        {
            //chunk.EditVoxel(new Vector3(position.x, position.y + i, position.z), EnumGameData.BlockID.log);
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), EnumGameData.BlockID.log));
        }
    }
}
