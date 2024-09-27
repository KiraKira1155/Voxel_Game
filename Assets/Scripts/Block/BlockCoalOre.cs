using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCoalOre : BaseBlockOre
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.coalOre;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (20, 20, 20, 20, 20, 20);
    protected override byte needRarity { get; set; } 
        = 1;

}
