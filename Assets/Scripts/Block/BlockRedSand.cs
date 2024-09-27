using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRedSand : BaseBlockSand
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.redSand;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
        = (9, 9, 9, 9, 9, 9);
}
