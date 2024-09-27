using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlockData
{

    /// <summary>
    /// 255�͔j��s��
    /// </summary>
    /// <returns></returns>
    abstract byte NeedRarity();

    /// <summary>
    /// �K���c�[���Apickaxe�̏ꍇ�͕K�v���Ԃ��L�т�
    /// </summary>
    /// <returns></returns>
    abstract EnumGameData.ItemType EfficientTool();

    /// <summary>
    /// NeedRarity�������Ή��c�[���g�p���̔j�󎞊ԁB
    /// <para>
    /// NeedRarity���O�̏ꍇ�̓��A���e�B��1�̑Ή��c�[���g�p��
    /// </para>
    /// </summary>
    /// <returns></returns>
    abstract float DestructionTime();

    /// <summary>
    /// �d�͂ɂ���ău���b�N��������
    /// </summary>
    /// <returns></returns>
    abstract bool Gravity();

    /// <summary>
    /// �u���b�N���R�Ă��邩
    /// </summary>
    abstract bool Burning();
}
