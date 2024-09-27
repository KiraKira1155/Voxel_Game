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
    /// 適正ツール、pickaxeの場合は必要時間が伸びる
    /// </summary>
    /// <returns></returns>
    abstract EnumGameData.ItemType EfficientTool();

    /// <summary>
    /// NeedRarityが同じ対応ツール使用時の破壊時間。
    /// <para>
    /// NeedRarityが０の場合はレアリティが1の対応ツール使用時
    /// </para>
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
