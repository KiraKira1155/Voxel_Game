using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDiorite : BaseBlockStone
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.diorite;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (19, 19, 19, 19, 19, 19);

    public override float DestructionTime()
    {
        return 1.2f;
    }
}
