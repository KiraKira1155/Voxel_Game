using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgainstBlcks
{
    private float checkIncrement = 0.05f;

    private const float maxDestructionTime = 0.05f;
    private const float coolTime = 0.25f;
    private float destructionTime;
    private float coolTimeCnt;
    private float time;
    private bool startBlockDestroy;

    private GameObject highlightBlock;
    private GameObject placeBlock;
    private GameObject cam;

    public void DoStart()
    {
        highlightBlock = PlayerManager.I.highlightBlock;
        placeBlock = PlayerManager.I.placeBlock;
        cam = PlayerManager.I.cam;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void DoUpdate()
    {
        PlayerInputKey();
        PlaceCursorBlocks();
    }

    Vector3 highlightPos;
    private void PlayerInputKey()
    {
        if (highlightBlock.gameObject.activeSelf)
        {
            //ブロックの破壊
            if (KeyConfig.GetKeyDown(KeyConfig.KeyName.LeftClick))
            {
                highlightPos = highlightBlock.transform.position;
                destructionTime = DestructionTime(World.I.BlockNeedDestructionTime(highlightPos));
                coolTimeCnt = coolTime;
            }
            if (KeyConfig.GetKey(KeyConfig.KeyName.LeftClick) && World.I.BlockNeedRarity(highlightPos) != -1)
            {
                if (highlightPos == highlightBlock.transform.position)
                {
                    startBlockDestroy = true;
                }
                else
                {
                    time = 0;
                    highlightPos = highlightBlock.transform.position;
                    destructionTime = DestructionTime(World.I.BlockNeedDestructionTime(highlightPos));
                    startBlockDestroy = false;
                }

                if (startBlockDestroy && BlockDestroyCoolTime())
                {
                    if (BlockDestroyInterval(destructionTime))
                    {
                        //ここで破壊が確定
                        DropItemManager.I.DropWhenDestroyBlock((EnumGameData.BlockID)World.I.CheckIfVoxel(highlightPos), highlightPos);
                        coolTimeCnt = 0;
                        World.I.GetChunkFromVector3(highlightPos).EditVoxel(highlightPos, EnumGameData.BlockID.air);
                        if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack != null)
                            if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.durability > -1)
                            {
                                PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.UseTool(1);
                            }
                    }
                }
            }
            if (KeyConfig.GetKeyUp(KeyConfig.KeyName.LeftClick) || highlightPos != highlightBlock.transform.position)
            {
                startBlockDestroy = false;
                time = 0;
            }

            //ブロックの設置
            if (KeyConfig.GetKeyDown(KeyConfig.KeyName.RightClick))
            {
                if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack != null)
                {
                    if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.kinds == EnumGameData.ItemKinds.item)
                        return;
                }

                if (placeBlock.transform.position.y < VoxelData.Hight - 4 && (PlayerManager.I.playerBodyUpper.transform.localPosition != PlayerManager.I.placeBlock.transform.localPosition && PlayerManager.I.playerBodyLower.transform.localPosition != PlayerManager.I.placeBlock.transform.localPosition))
                {
                    if (!World.I.CheckForVoxel(PlayerManager.I.playerBodyLower.transform.position) && !World.I.CheckForVoxel(PlayerManager.I.playerBodyUpper.transform.position))
                    {
                        if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].HasItem)
                        {
                            if (!PlayerStatus.isGrounded && ((PlayerManager.I.playerBodyUpper.transform.localPosition.y + 1.0f) != PlayerManager.I.placeBlock.transform.localPosition.y))
                            {
                                World.I.GetChunkFromVector3(PlayerManager.I.placeBlock.transform.position).EditVoxel(PlayerManager.I.placeBlock.transform.position, PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.blockID);
                                PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.Take(1);
                            }
                            if (PlayerStatus.isGrounded)
                            {
                                World.I.GetChunkFromVector3(PlayerManager.I.placeBlock.transform.position).EditVoxel(PlayerManager.I.placeBlock.transform.position, PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.blockID);
                                PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.Take(1);
                            }
                        }
                    }
                }
            }
        }
    }

    private void GetDestructionBlockItem(Vector3 destroyPos)
    {
        for (int i = 0; i < Inventory.I.toolBarSlots.Count; i++)
        {
            if (Inventory.I.toolBarSlots[i].stack == null)
            {
                ItemStack getItem = new ItemStack(EnumGameData.ItemKinds.blockItem, (EnumGameData.BlockID)World.I.CheckIfVoxel(destroyPos), 1);
                Inventory.I.toolBarSlots[i].InsertStack(getItem);
                return;
            }
            if (Inventory.I.toolBarSlots[i].stack.blockID == (EnumGameData.BlockID)World.I.CheckIfVoxel(destroyPos)
                && Inventory.I.toolBarSlots[i].stack.amount < BlockManager.I.MaxAmount((EnumGameData.BlockID)World.I.CheckIfVoxel(destroyPos)))
            {
                Inventory.I.toolBarSlots[i].GetItem(1);
                return;
            }
        }
    }
    
    public float DestructionTime(float intervalTime)
    {
        //所持アイテムによって採掘必要時間を変更
        if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack != null)
        {
            if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.kinds == EnumGameData.ItemKinds.item
                && PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.itemType == World.I.BlockEfficientTool(highlightBlock.transform.position))
            {
                int rarity = World.I.BlockNeedRarity(highlightBlock.transform.position) - ItemManager.I.GetRarity(PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.itemType, PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.itemID);

                if (World.I.BlockNeedRarity(highlightBlock.transform.position) == 0)
                {
                    rarity++;
                }

                if (rarity > 0)
                {
                    if (World.I.BlockEfficientTool(highlightBlock.transform.position) == EnumGameData.ItemType.pickaxe)
                        rarity += 4;
                    return intervalTime *= (rarity + 1);
                }
                else if (rarity < 0)
                {
                    float t = 1;
                    for (int i = 0; rarity < i; i--)
                    {
                        t *= 0.85f;
                    }

                    if (intervalTime * t <= maxDestructionTime)
                    {
                        return maxDestructionTime;
                    }

                    return intervalTime * t;
                }
                else
                {
                    return intervalTime;
                }
            }
            else
            {
                int rarity = World.I.BlockNeedRarity(highlightBlock.transform.position);
                if (World.I.BlockEfficientTool(highlightBlock.transform.position) == EnumGameData.ItemType.pickaxe)
                    rarity += 5;
                return intervalTime *= (rarity + 1.5f);
            }
        }
        else
        {
            int rarity = World.I.BlockNeedRarity(highlightBlock.transform.position);
            if (World.I.BlockEfficientTool(highlightBlock.transform.position) == EnumGameData.ItemType.pickaxe)
                rarity += 5;
            return intervalTime  *= (rarity + 1.5f);
        }
    }
 
    private bool BlockDestroyInterval(float intervalTime)
    {
        time += Time.deltaTime;

        if (PlayerStatus.Creative)
        {
            if (time >= maxDestructionTime)
            {
                time = 0;
                return true;
            }
        }
        else
        {
            if (time >= maxDestructionTime && time >= intervalTime)
            {
                time = 0;
                return true;
            }
        }
        return false;
    }

    private bool BlockDestroyCoolTime()
    {
        coolTimeCnt += Time.deltaTime;
        if (PlayerStatus.Creative)
            return true;
        if (destructionTime <= maxDestructionTime)
            return true;
        if (coolTimeCnt >= coolTime)
            return true;
        else
            return false;
    }

    private void PlaceCursorBlocks()
    {
        float step = checkIncrement;
        Vector3 pos = cam.transform.position + (cam.transform.forward * step);
        Vector3 lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
        while (step < PlayerStatus.Reach)
        {
            pos = cam.transform.position + (cam.transform.forward * step);
            if (World.I.CheckForVoxel(pos))
            {
                highlightBlock.transform.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                PlayerManager.I.placeBlock.transform.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                PlayerManager.I.placeBlock.gameObject.SetActive(true);

                return;
            }
            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }

        highlightBlock.gameObject.SetActive(false);
        PlayerManager.I.placeBlock.gameObject.SetActive(false);
    }
}
