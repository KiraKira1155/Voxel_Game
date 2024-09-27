using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDirt : BaseBlockDirt
{
    protected override EnumGameData.BlockID id { get; set; } 
        = EnumGameData.BlockID.dirt;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (11, 11, 11, 11, 11, 11);
}
