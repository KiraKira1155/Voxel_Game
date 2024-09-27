using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEmeraldOre : BaseBlockOre
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.emeraldBlock;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (26, 26, 26, 26, 26, 26);
    protected override byte needRarity { get; set; }
        = 3;
}
