using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockStone : BaseBlockStone
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.stone;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (12, 12, 12, 12, 12, 12);
}
