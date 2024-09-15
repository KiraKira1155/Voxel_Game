using System.Collections;
using System.Collections.Generic;

public interface IBaseBlock : IBlockData
{
    abstract void Init();
    abstract EnumGameData.BlockID ID();

    /// <summary>
    /// �e�N�X�`���̐ݒ�A���������ɐݒ肳���
    /// </summary>
    /// <returns></returns>
    abstract (int back, int front, int top, int bottom, int left, int right) SetTexture();

    /// <summary>
    /// �`����s����
    /// </summary>
    /// <returns></returns>
    abstract bool IsDisplay();

    /// <summary>
    /// ���ߏ������s����
    /// </summary>
    abstract bool IsTransparent();

    /// <summary>
    /// �v���C���[�Ƃ̓����蔻�肪���邩
    /// </summary>
    abstract bool IsSolid();

    /// <summary>
    /// �ݒu�����̊m�F���s����
    /// </summary>
    abstract bool CheckDirection();

    abstract EnumGameData.BlockID GetTextureFace(BlockManager.faceIndex faceIndex);
}
