using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockIronBlock : BaseBlockOreBlock
{
    protected override EnumGameData.BlockID id { get; set; }
           = EnumGameData.BlockID.ironBlock;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (30, 30, 30, 30, 30, 30);
    protected override byte needRarity { get; set; }
        = 2;
}