using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSnowDirt : BaseBlockDirt
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.snowDirt;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (3, 3, 4, 11, 3, 3);
}
