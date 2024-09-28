using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBlockLog : BaseBlock
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
        return 0;
    }

    public override EnumGameData.ItemType EfficientTool()
    {
        return EnumGameData.ItemType.axe;
    }

    public override float DestructionTime()
    {
        return 2.0f;
    }

    public override bool Burning()
    {
        return true;
    }
}