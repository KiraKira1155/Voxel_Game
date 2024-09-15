using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBar
{
    public GameObject[] eachSlot;
    public Slot slot;
    public int slotIndex = 0;
    private GameObject highlight;

    public void DoAwake(Slot slot)
    {
        this.slot = slot;
        eachSlot = slot.eachSlot;
        int i = 0;
        foreach (GameObject s in eachSlot)
        {
            s.transform.GetChild(0).GetComponent<ItemBlockUI>().DoAwake();
            s.transform.GetChild(1).GetComponent<ItemUI>().DoAwake();
            i++;
        }
    }

    public void DoStart()
    {
        highlight = slot.highlight;
    }

    public void DoUpdate()
    {
        int scroll = (int)Input.mouseScrollDelta.y;

        if (scroll != 0)
        {
            if (scroll < 0)
                slotIndex++;
            else
                slotIndex--;

            if (slotIndex > eachSlot.Length -1)
                slotIndex = 0;
            if (slotIndex < 0)
                slotIndex = eachSlot.Length - 1;

            highlight.transform.position = eachSlot[slotIndex].transform.position;
        }

    }
}
