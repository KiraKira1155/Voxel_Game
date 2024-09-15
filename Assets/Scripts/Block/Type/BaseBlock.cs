using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBlock : IBaseBlock
{

    private (int back, int front, int top, int bottom, int left, int right) texture;

    public virtual void Init()
    {
        texture = SetTexture();
    }

    public abstract EnumGameData.BlockID ID();

    public abstract (int back, int front, int top, int bottom, int left, int right) SetTexture();

    public virtual bool IsDisplay()
    {
        return true;
    }
    public virtual bool IsTransparent()
    {
        return false;
    }
    public virtual bool IsSolid()
    {
        return true;
    }
    public virtual bool CheckDirection()
    {
        return false;
    }
    public abstract byte NeedRarity();
    public abstract EnumGameData.ItemType EfficientTool();
    public abstract float DestructionTime();
    public virtual bool Gravity()
    {
        return false;
    }
    public virtual bool Burning()
    {
        return false;
    }

    public EnumGameData.BlockID GetTextureFace(BlockManager.faceIndex faceIndex)
    {
        switch (faceIndex)
        {
            case BlockManager.faceIndex.back:
                return (EnumGameData.BlockID)texture.back;
            case BlockManager.faceIndex.front:
                return (EnumGameData.BlockID)texture.front;
            case BlockManager.faceIndex.top:
                return (EnumGameData.BlockID)texture.top;
            case BlockManager.faceIndex.bottom:
                return (EnumGameData.BlockID)texture.bottom;
            case BlockManager.faceIndex.left:
                return (EnumGameData.BlockID)texture.left;
            case BlockManager.faceIndex.right:
                return (EnumGameData.BlockID)texture.right;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }

}
