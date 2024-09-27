using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGrass : BaseBlockDirt
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.grass;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (1, 1, 2, 11, 1, 1);
}
