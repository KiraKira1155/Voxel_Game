using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGranite : BaseBlockStone
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.granite;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (18, 18, 18, 18, 18, 18);

    public override float DestructionTime()
    {
        return 1.2f;
    }
}
