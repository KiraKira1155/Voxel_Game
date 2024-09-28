using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockOakLog : BaseBlockLog
{
    protected override EnumGameData.BlockID id { get; set; }
        = EnumGameData.BlockID.oakLog;
    protected override (int back, int front, int top, int bottom, int left, int right) texture { get; set; }
    = (65, 65, 66, 66, 65, 65);
}
