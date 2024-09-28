using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockNetheriteBlock : BaseBlockOreBlock
{
    protected override EnumGameData.BlockID id { get; set; }
           = EnumGameData.BlockID.netheriteBlock;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (36, 36, 36, 36, 36, 36);
    protected override byte needRarity { get; set; }
        = 4;
}