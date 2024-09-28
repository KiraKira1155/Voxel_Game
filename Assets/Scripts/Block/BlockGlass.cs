using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGlass : BaseBlockGlass
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.glass;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (0, 0, 0, 0, 0, 0);
}
