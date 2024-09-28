using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGoldBlock : BaseBlockOreBlock
{
    protected override EnumGameData.BlockID id { get; set; }
           = EnumGameData.BlockID.goldBlock;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (31, 31, 31, 31, 31, 31);
    protected override byte needRarity { get; set; }
        = 3;
}