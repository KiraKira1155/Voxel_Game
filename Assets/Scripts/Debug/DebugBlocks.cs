using System.Collections;
using System.Collections.Generic;

public class DebugBlocks
{
    public void DoUpdate()
    {
        DestructionTime();
    }

    public void DestructionTime()
    {
        if (PlayerManager.I.highlightBlock.activeSelf)
            UnityEngine.Debug.Log(PlayerManager.I.playerAgainstBlcks.DestructionTime(World.I.BlockNeedDestructionTime(PlayerManager.I.highlightBlock.transform.position)));
    }
}
