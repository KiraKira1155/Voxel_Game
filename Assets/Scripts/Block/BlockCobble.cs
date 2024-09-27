using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCobble : BaseBlockStone
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.cobble;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (15, 15, 15, 15, 15, 15);
}
