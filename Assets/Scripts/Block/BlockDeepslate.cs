using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDeepslate : BaseBlockStone
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.deepslate;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (13, 13, 14, 14, 13, 13);

    public override float DestructionTime()
    {
        return 2.0f;
    }
}
