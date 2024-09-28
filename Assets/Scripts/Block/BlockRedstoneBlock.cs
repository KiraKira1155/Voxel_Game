using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRedstoneBlock : BaseBlockOreBlock
{
    protected override EnumGameData.BlockID id { get; set; }
           = EnumGameData.BlockID.redstoneBlock;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (33, 33, 33, 33, 33, 33);
    protected override byte needRarity { get; set; }
        = 3;
}
