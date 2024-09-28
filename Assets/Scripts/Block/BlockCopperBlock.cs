using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCopperBlock : BaseBlockOreBlock
{
    protected override EnumGameData.BlockID id { get; set; }
           = EnumGameData.BlockID.copperBlock;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (34, 34, 34, 34, 34, 34);
    protected override byte needRarity { get; set; }
        = 2;
}