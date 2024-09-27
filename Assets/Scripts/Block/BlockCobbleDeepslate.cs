using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCobbleDeepslate : BaseBlockStone
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.cobbleDeepslate;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (16, 16, 16, 16, 16, 16);

    public override float DestructionTime()
    {
        return 2.0f;
    }
}
