using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSnow : BaseBlock
{
    public override EnumGameData.BlockID ID()
    {
        return EnumGameData.BlockID.snow;
    }

    public override (int back, int front, int top, int bottom, int left, int right) SetTexture()
    {
        return (4, 4, 4, 4, 4, 4);
    }

    public override byte NeedRarity()
    {
        return 0;
    }

    public override EnumGameData.ItemType EfficientTool()
    {
        return EnumGameData.ItemType.shovel;
    }

    public override float DestructionTime()
    {
        return 0.15f;
    }
}
