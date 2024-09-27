using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAndesite : BaseBlockStone
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.andesite;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (17, 17, 17, 17, 17, 17);

    public override float DestructionTime()
    {
        return 1.2f;
    }
}
