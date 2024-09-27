using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRedstoneOre : BaseBlockOre
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.redstoneOre;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (24, 24, 24, 24, 24, 24);
    protected override byte needRarity { get; set; }
        = 3;

}