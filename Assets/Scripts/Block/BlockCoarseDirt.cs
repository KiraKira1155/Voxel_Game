using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCoarseDirt : BaseBlockDirt
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.coarseDirt;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (7, 7, 7, 7, 7, 7);
}
