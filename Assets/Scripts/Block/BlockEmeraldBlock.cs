using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEmeraldBlock : BaseBlockOreBlock
{
    protected override EnumGameData.BlockID id { get; set; }
           = EnumGameData.BlockID.emeraldBlock;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (35, 35, 35, 35, 35, 35);
    protected override byte needRarity { get; set; }
        = 3;
}
