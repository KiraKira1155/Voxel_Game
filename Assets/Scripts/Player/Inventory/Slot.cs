using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public UIItemSlot[] toolbarSlots;
    public GameObject[] eachSlot;
    public GameObject highlight;

    public void DoAwake()
    {
        eachSlot = new GameObject[PlayerStatus.SlotNum];
        int childObjCnt = gameObject.transform.childCount - 1;  //�n�C���C�g�p�I�u�W�F��������
        for (int i = 0; i < childObjCnt; i++)
        {
            Transform childTransform = gameObject.transform.GetChild(i);
            eachSlot[i] = childTransform.gameObject;
        }
        Transform highlightTransform = gameObject.transform.GetChild(childObjCnt);
        highlight = highlightTransform.gameObject;
    }

    public void DoStart()
    {
        foreach (UIItemSlot s in toolbarSlots)
        {
            ItemStack stack = null;
            s.itemSlot = new ItemSlot(s, stack);
        }
    }
}
