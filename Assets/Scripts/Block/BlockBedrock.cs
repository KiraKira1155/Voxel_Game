using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBedrock : BaseBlock
{
    public override EnumGameData.BlockID ID()
    {
        return EnumGameData.BlockID.bedrock;
    }

    public override (int back, int front, int top, int bottom, int left, int right) SetTexture()
    {
        return (0, 0, 0, 0, 0, 0);
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
