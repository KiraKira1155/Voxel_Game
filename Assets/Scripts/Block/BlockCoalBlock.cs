using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCoalBlock : BaseBlockOreBlock
{
    protected override EnumGameData.BlockID id { get; set; }
           = EnumGameData.BlockID.coalBlock;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (29, 29, 29, 29, 29, 29);
    protected override byte needRarity { get; set; }
        = 1;
}
