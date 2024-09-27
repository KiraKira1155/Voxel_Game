using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCopperOre : BaseBlockOre
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.copperOre;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (25, 25, 25, 25, 25, 25);
    protected override byte needRarity { get; set; }
        = 2;

}