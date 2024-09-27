using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGravel : BaseBlockSand
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.gravel;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (10, 10, 10, 10, 10, 10);
}
