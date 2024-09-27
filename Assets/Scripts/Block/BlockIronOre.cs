using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockIronOre : BaseBlockOre
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.ironOre;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (21, 21, 21, 21, 21, 21);
    protected override byte needRarity { get; set; }
        = 2;

}
