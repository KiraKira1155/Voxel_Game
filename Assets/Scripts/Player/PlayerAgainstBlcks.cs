using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAgainstBlcks
{
    private float checkIncrement = 0.05f;

    private const float maxDestructionTime = 0.05f;
    private const float coolTime = 0.25f;
    public float destructionTime { get; private set; }
    private float coolTimeCnt;
    public float miningTime {  get; private set; }
    private bool startBlockDestroy;

    private int blockID;


    public enum CursorFaceDirection
    {
        top,
        bottom,
        north,
        east,
        west,
        south
    }

    public void DoStart()
    {
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
        if (PlayerManager.I.highlightBlock.gameObject.activeSelf)
        {
            DestructionBlock();
            InstallationBlock();
        }
    }

    private void DestructionBlock()
    {
        if (KeyConfig.GetKeyDown(KeyConfig.KeyName.LeftClick))
        {
            highlightPos = PlayerManager.I.highlightBlock.transform.position;
            blockID = World.I.CheckForBlockID(highlightPos);
            destructionTime = DestructionTime(BlockManager.I.blocktype[blockID].destructionTime);
            coolTimeCnt = coolTime;
        }
        if (KeyConfig.GetKey(KeyConfig.KeyName.LeftClick) && BlockManager.I.blocktype[blockID].needRarity != -1)
        {
            if (highlightPos == PlayerManager.I.highlightBlock.transform.position)
            {
                startBlockDestroy = true;
            }
            else
            {
                miningTime = 0;
                highlightPos = PlayerManager.I.highlightBlock.transform.position;
                blockID = World.I.CheckForBlockID(highlightPos);
                destructionTime = DestructionTime(BlockManager.I.blocktype[blockID].destructionTime);
                startBlockDestroy = false;
            }

            if (startBlockDestroy && BlockDestroyCoolTime())
            {
                if (BlockDestroyInterval(destructionTime))
                {

                    //ここで破壊が確定
                    DropItemManager.I.DropWhenDestroyBlock((EnumGameData.BlockID)blockID, highlightPos);
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
        if (KeyConfig.GetKeyUp(KeyConfig.KeyName.LeftClick) || highlightPos != PlayerManager.I.highlightBlock.transform.position)
        {
            startBlockDestroy = false;
            miningTime = 0;
        }
    }

    private void InstallationBlock()
    {
        if (KeyConfig.GetKeyDown(KeyConfig.KeyName.RightClick))
        {
            if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack != null)
            {
                if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.kinds == EnumGameData.ItemKinds.item)
                    return;
            }

            if (PlayerManager.I.placeBlock.transform.position.y < VoxelData.Hight - 4 && (PlayerManager.I.playerBodyUpper.transform.localPosition != PlayerManager.I.placeBlock.transform.localPosition && PlayerManager.I.playerBodyLower.transform.localPosition != PlayerManager.I.placeBlock.transform.localPosition))
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
    
    /// <summary>
    /// 採掘にかかる時間
    /// </summary>
    /// <param name="intervalTime"></param>
    /// <returns></returns>
    public float DestructionTime(float intervalTime)
    {
        //所持アイテムによって採掘必要時間を変更
        if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack != null)
        {
            if (PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.kinds == EnumGameData.ItemKinds.item
                && PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.itemType == BlockManager.I.blocktype[blockID].efficientTool)
            {
                int rarity = BlockManager.I.blocktype[blockID].needRarity - ItemManager.I.GetRarity(PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.itemType, PlayerManager.I.toolBar.slot.toolbarSlots[PlayerManager.I.toolBar.slotIndex].itemSlot.stack.itemID);

                if (BlockManager.I.blocktype[blockID].needRarity == 0)
                {
                    rarity++;
                }

                if (rarity > 0)
                {
                    if (BlockManager.I.blocktype[blockID].efficientTool == EnumGameData.ItemType.pickaxe)
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
                int rarity = BlockManager.I.blocktype[blockID].needRarity;
                if (BlockManager.I.blocktype[blockID].efficientTool == EnumGameData.ItemType.pickaxe)
                    rarity += 5;
                return intervalTime *= (rarity + 1.5f);
            }
        }
        else
        {
            int rarity = BlockManager.I.blocktype[blockID].needRarity;
            if (BlockManager.I.blocktype[blockID].efficientTool == EnumGameData.ItemType.pickaxe)
                rarity += 5;
            return intervalTime  *= (rarity + 1.5f);
        }
    }
 
    private bool BlockDestroyInterval(float intervalTime)
    {
        miningTime += Time.deltaTime;

        if (PlayerStatus.Creative)
        {
            if (miningTime >= maxDestructionTime)
            {
                miningTime = 0;
                return true;
            }
        }
        else
        {
            if (miningTime >= maxDestructionTime && miningTime >= intervalTime)
            {
                miningTime = 0;
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
        Vector3 pos = PlayerManager.I.cam.transform.position + (PlayerManager.I.cam.transform.forward * step);
        Vector3 lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
        while (step < PlayerStatus.Reach)
        {
            pos = PlayerManager.I.cam.transform.position + (PlayerManager.I.cam.transform.forward * step);
            if (World.I.CheckForVoxel(pos))
            {

                PlayerManager.I.highlightBlock.gameObject.SetActive(true);
                PlayerManager.I.placeBlock.gameObject.SetActive(true);

                PlayerManager.I.highlightBlock.transform.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                CheckCursorBlockFaceDirection(lastPos);
                if (World.I.CheckForVoxel(PlayerManager.I.highlightBlock.transform.position))
                {
                    Vector3 HighlightPos = new Vector3(PlayerManager.I.placeBlock.transform.position.x, PlayerManager.I.highlightBlock.transform.position.y, PlayerManager.I.placeBlock.transform.position.z);
                    if (World.I.CheckForVoxel(HighlightPos))
                    {
                        PlayerManager.I.highlightBlock.transform.position = HighlightPos;
                        PlayerManager.I.placeBlock.transform.position = new Vector3(PlayerManager.I.highlightBlock.transform.position.x, PlayerManager.I.placeBlock.transform.position.y, PlayerManager.I.highlightBlock.transform.position.z);
                    }
                }
                blockID = World.I.CheckForBlockID(PlayerManager.I.highlightBlock.transform.position);
                destructionTime = DestructionTime(BlockManager.I.blocktype[blockID].destructionTime);
                return;
            }
            lastPos = pos;
            step += checkIncrement;
        }

        PlayerManager.I.highlightBlock.gameObject.SetActive(false);
        PlayerManager.I.placeBlock.gameObject.SetActive(false);
    }

    private void CheckCursorBlockFaceDirection(Vector3 placePos)
    {
        Vector3 targetPos = PlayerManager.I.highlightBlock.transform.position;
        Vector3 targetVoxelPos = World.I.CheckVoxelPos(targetPos);
        float check= checkIncrement + 0.01f;

        PlayerManager.I.placeBlock.transform.position = new Vector3(Mathf.FloorToInt(placePos.x), Mathf.FloorToInt(placePos.y), Mathf.FloorToInt(placePos.z));

        var task = Task.Run(() =>
        {
            if (targetVoxelPos == World.I.CheckVoxelPos(placePos + new Vector3(0, -check, 0)))
            {
                PlayerManager.I.cursorFaceDirection = CursorFaceDirection.top;
                return;
            }
            else if (targetVoxelPos == World.I.CheckVoxelPos(placePos + new Vector3(0, check, 0)))
            {
                PlayerManager.I.cursorFaceDirection = CursorFaceDirection.bottom;
                return;
            }
            else if (targetVoxelPos == World.I.CheckVoxelPos(placePos + new Vector3(check, 0, 0)))
            {
                PlayerManager.I.cursorFaceDirection = CursorFaceDirection.east;
                return;
            }
            else if (targetVoxelPos == World.I.CheckVoxelPos(placePos + new Vector3(0, 0, check)))
            {
                PlayerManager.I.cursorFaceDirection = CursorFaceDirection.south;
                return;
            }
            else if (targetVoxelPos == World.I.CheckVoxelPos(placePos + new Vector3(-check, 0, 0)))
            {
                PlayerManager.I.cursorFaceDirection = CursorFaceDirection.west;
                return;
            }
            else if (targetVoxelPos == World.I.CheckVoxelPos(placePos + new Vector3(0, 0, -check)))
            {
                PlayerManager.I.cursorFaceDirection = CursorFaceDirection.north;
                return;
            }
        });

        if(targetVoxelPos.y < World.I.CheckVoxelPos(placePos).y || targetVoxelPos.y > World.I.CheckVoxelPos(placePos).y)
        {
            PlayerManager.I.placeBlock.transform.position = new Vector3(Mathf.FloorToInt(targetPos.x), Mathf.FloorToInt(placePos.y), Mathf.FloorToInt(targetPos.z));
            if (World.I.CheckForVoxel(PlayerManager.I.placeBlock.transform.position))
            {
                Vector3 currentPlacePos = PlayerManager.I.placeBlock.transform.position;

                if (World.I.CheckVoxelPos(currentPlacePos) == World.I.CheckVoxelPos(placePos + new Vector3(check, 0, 0)))
                {
                    PlayerManager.I.cursorFaceDirection = CursorFaceDirection.east;
                    PlayerManager.I.placeBlock.transform.position += new Vector3(-1, 0, 0);
                    return;
                }
                else if (World.I.CheckVoxelPos(currentPlacePos) == World.I.CheckVoxelPos(placePos + new Vector3(0, 0, check)))
                {
                    PlayerManager.I.cursorFaceDirection = CursorFaceDirection.south;
                    PlayerManager.I.placeBlock.transform.position += new Vector3(0, 0, -1);
                    return;
                }
                else if (World.I.CheckVoxelPos(currentPlacePos) == World.I.CheckVoxelPos(placePos + new Vector3(-check, 0, 0)))
                {
                    PlayerManager.I.cursorFaceDirection = CursorFaceDirection.west;
                    PlayerManager.I.placeBlock.transform.position += new Vector3(1, 0, 0);
                    return;
                }
                else if (World.I.CheckVoxelPos(currentPlacePos) == World.I.CheckVoxelPos(placePos + new Vector3(0, 0, -check)))
                {
                    PlayerManager.I.cursorFaceDirection = CursorFaceDirection.north;
                    PlayerManager.I.placeBlock.transform.position += new Vector3(0, 0, 1);
                    return;
                }
            }
            return;
        }
    }
}
