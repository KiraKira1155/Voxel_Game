using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockOakLeaf : BaseBlockLeaf
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.oakLeaf;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (1, 1, 1, 1, 1, 1);
}
