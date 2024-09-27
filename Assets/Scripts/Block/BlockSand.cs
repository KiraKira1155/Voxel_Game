using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSand : BaseBlockSand
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.sand;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (8, 8, 8, 8, 8, 8);
}
