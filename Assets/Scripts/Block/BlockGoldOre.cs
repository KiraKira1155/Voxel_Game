using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGoldOre : BaseBlockOre
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.goldOre;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (22, 22, 22, 22, 22, 22);
    protected override byte needRarity { get; set; }
        = 3;

}