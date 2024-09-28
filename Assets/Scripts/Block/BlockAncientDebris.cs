using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAncientDebris : BaseBlock
{

    public override EnumGameData.BlockID ID()
    {
        return EnumGameData.BlockID.ancientDebris;
    }

    public override (int back, int front, int top, int bottom, int left, int right) SetTexture()
    {
        return (27, 27, 28, 28, 27, 27);
    }

    public override byte NeedRarity()
    {
        return 4;
    }

    public override EnumGameData.ItemType EfficientTool()
    {
        return EnumGameData.ItemType.pickaxe;
    }

    public override float DestructionTime()
    {
        return 30.0f;
    }
}
