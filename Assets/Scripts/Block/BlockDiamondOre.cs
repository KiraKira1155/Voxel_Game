using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDiamondOre : BaseBlockOre
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.diamondOre;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (23, 23, 23, 23, 23, 23);
    protected override byte needRarity { get; set; }
        = 3;

}
