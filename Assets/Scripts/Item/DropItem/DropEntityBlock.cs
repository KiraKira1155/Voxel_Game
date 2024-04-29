using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEntityBlock : MonoBehaviour
{
    public BlockMesh blockMesh = new BlockMesh();
    public EnumGameData.ItemKinds kinds { get; private set; } = EnumGameData.ItemKinds.blockItem;
    private EnumGameData.BlockID blockID;
    public bool pickUp { get; private set; }

    private Vector3 thisPos;
    private Vector3 playerPos;
    private const float serchPos = 1.5f;
    private const float body = 0.22f;
    private bool serch;

    public void PickUp()
    {
        pickUp = true;
    }
    public void DoFixedUpDate()
    {
        thisPos = transform.position;
        playerPos = PlayerManager.I.transform.position;
        if (!serch)
            Conduct();
        SerchPlayer();
    }

    private void SerchPlayer()
    {
        float distance = Vector3.Distance(thisPos, playerPos);
        serch = false;
        if (distance <= serchPos)
        {
            serch = true;
            GetCloser(distance);
            if (distance <= 0.5f)
            {
                PickUp();
            }
        }
    }

    private void Conduct()
    {
        float gravity = Time.fixedDeltaTime * PlayerStatus.Gravity / 2;
        if (World.I.CheckForVoxel(new Vector3(thisPos.x, thisPos.y + gravity, thisPos.z)))
        {
            transform.position = new Vector3(thisPos.x, Mathf.FloorToInt(thisPos.y), thisPos.z);
            gravity = 0;
        }
        else
        {
            transform.position += new Vector3(0, gravity, 0);
        }
    }

    private void GetCloser(float distance)
    {
        float speed = (3 - distance) * Time.fixedDeltaTime;
        Vector3 distancePos = playerPos - thisPos;
        if (distancePos.y >= 0)
        {
            if (!World.I.CheckForVoxel(thisPos + new Vector3(0, speed, 0)))
            {
                transform.position += new Vector3(0, Time.fixedDeltaTime * -PlayerStatus.Gravity / 2, 0);
            }
        }
        else if (distancePos.y < 0)
        {
            serch = false;
        }

        if (distancePos.x < 0)
        {
            if (!World.I.CheckForVoxel(thisPos + new Vector3(-speed -body, 0, 0)))
            {
                transform.position += new Vector3(-speed, 0, 0);
            }
        }
        else if (distancePos.x > 0)
        {
            if (!World.I.CheckForVoxel(thisPos + new Vector3(speed + body, 0, 0)))
            {
                transform.position += new Vector3(speed, 0, 0);
            }
        }

        if (distancePos.z < 0)
        {
            if (!World.I.CheckForVoxel(thisPos + new Vector3(0, 0, -speed - body)))
            {
                transform.position += new Vector3(0, 0, -speed);
            }
        }
        else if (distancePos.z > 0)
        {
            if (!World.I.CheckForVoxel(thisPos + new Vector3(0, 0, speed + body)))
            {
                transform.position += new Vector3(0, 0, speed);
            }
        }
    }

    public void IsDestroy(GameObject obj)
    {
        blockMesh = null;
        GetDestructionBlockItem();
        Destroy(obj, 0.2f);
    }
    private void GetDestructionBlockItem()
    {
        for (int i = 0; i < Inventory.I.toolBarSlots.Count; i++)
        {
            if (Inventory.I.toolBarSlots[i].stack == null)
            {
                ItemStack getItem = new ItemStack(EnumGameData.ItemKinds.blockItem, blockID, 1);
                Inventory.I.toolBarSlots[i].InsertStack(getItem);
                return;
            }
            if (Inventory.I.toolBarSlots[i].stack.blockID == blockID
                && Inventory.I.toolBarSlots[i].stack.amount < BlockManager.I.MaxAmount(blockID))
            {
                Inventory.I.toolBarSlots[i].GetItem(1);
                return;
            }
        }
    }

    public void Init(EnumGameData.BlockID blockID)
    {
        this.blockID = blockID;
    }
}
