using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDiamondBlock : BaseBlockOreBlock
{
    protected override EnumGameData.BlockID id { get; set; }
           = EnumGameData.BlockID.diamondBlock;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (32, 32, 32, 32, 32, 32);
    protected override byte needRarity { get; set; }
        = 3;
}
