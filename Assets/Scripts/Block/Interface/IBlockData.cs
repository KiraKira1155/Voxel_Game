using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlockData
{

    /// <summary>
    /// 255は破壊不可
    /// </summary>
    /// <returns></returns>
    abstract byte NeedRarity();

    /// <summary>
    /// 適正ツール
    /// </summary>
    /// <returns></returns>
    abstract EnumGameData.ItemType EfficientTool();

    /// <summary>
    /// 基本破壊時間
    /// </summary>
    /// <returns></returns>
    abstract float DestructionTime();

    /// <summary>
    /// 重力によってブロックが動くか
    /// </summary>
    /// <returns></returns>
    abstract bool Gravity();

    /// <summary>
    /// ブロックが燃焼するか
    /// </summary>
    abstract bool Burning();
}
