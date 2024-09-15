using System.Collections;
using System.Collections.Generic;

public interface IBaseBlock : IBlockData
{
    abstract void Init();
    abstract EnumGameData.BlockID ID();

    /// <summary>
    /// テクスチャの設定、初期化時に設定される
    /// </summary>
    /// <returns></returns>
    abstract (int back, int front, int top, int bottom, int left, int right) SetTexture();

    /// <summary>
    /// 描画を行うか
    /// </summary>
    /// <returns></returns>
    abstract bool IsDisplay();

    /// <summary>
    /// 透過処理を行うか
    /// </summary>
    abstract bool IsTransparent();

    /// <summary>
    /// プレイヤーとの当たり判定があるか
    /// </summary>
    abstract bool IsSolid();

    /// <summary>
    /// 設置方向の確認を行うか
    /// </summary>
    abstract bool CheckDirection();

    abstract EnumGameData.BlockID GetTextureFace(BlockManager.faceIndex faceIndex);
}
