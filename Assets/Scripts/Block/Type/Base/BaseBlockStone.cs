using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBlockStone : BaseBlock
{
    protected abstract EnumGameData.BlockID id { get; set; }
    protected abstract (int back, int front, int top, int bottom, int left, int right) texture { get; set; }

    public override EnumGameData.BlockID ID()
    {
        return id;
    }

    public override (int back, int front, int top, int bottom, int left, int right) SetTexture()
    {
        return texture;
    }

    public override byte NeedRarity()
    {
        return 1;
    }

    public override EnumGameData.ItemType EfficientTool()
    {
        return EnumGameData.ItemType.pickaxe;
    }

    public override float DestructionTime()
    {
        return 1.5f;
    }
}
