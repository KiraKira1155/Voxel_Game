using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAir : BaseBlock
{
    public override EnumGameData.BlockID ID()
    {
        return EnumGameData.BlockID.air;
    }

    public override bool IsDisplay()
    {
        return false;
    }

    public override (int back, int front, int top, int bottom, int left, int right) SetTexture()
    {
        return (0, 0, 0, 0, 0, 0);
    }
    public override bool IsSolid()
    {
        return false;
    }

    public override byte NeedRarity()
    {
        return 255;
    }

    public override EnumGameData.ItemType EfficientTool()
    {
        return EnumGameData.ItemType.Hand;
    }

    public override float DestructionTime()
    {
        return 0;
    }
}
