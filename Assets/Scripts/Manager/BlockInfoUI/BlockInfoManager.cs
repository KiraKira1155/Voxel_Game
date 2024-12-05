using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockInfoManager : Singleton<BlockInfoManager>
{
    [SerializeField] private ItemBlockUI blockIcon;
    [SerializeField] private ItemUI toolIcon;

    [SerializeField] private ItemBlockUI[] dropBlockIcon;
    [SerializeField] private ItemUI[] dropItemIcon;

    [SerializeField] private Slider destructionTimeUI;
    [SerializeField] private TextMeshProUGUI blockNameTex;
    [SerializeField] private TextMeshProUGUI destructionTimeTex;
    [SerializeField] private Canvas infoCanvas;
    private Vector3 previousHighlightBlockPos;
    private EnumGameData.ItemID previousItem;
    private int previousSlotIndex;

    private int blockID;

    private int needRearity;
    private EnumGameData.ItemType efficientTool;

    private float destructionTime;

    private BlockDropItem.DropItem[] dropItem;

    private const int itemIDChangeBlockID = 600;

    public void DoAwake()
    {
        Init();
        blockIcon.DoAwake();
        toolIcon.DoAwake();
        foreach (var icon in dropBlockIcon)
        {
            icon.DoAwake();
            icon.enabled = false;
        }
        foreach (var icon in dropItemIcon)
        {
            icon.DoAwake();
            icon.enabled = false;
        }
    }

    private void InitBlockInfo()
    {

        previousHighlightBlockPos = PlayerManager.I.highlightBlock.transform.position;
        blockID = WorldManager.I.CheckForBlockID(previousHighlightBlockPos);

        blockIcon.UpdateUI((EnumGameData.BlockID)blockID);

        needRearity = BlockManager.I.GetBlockData(blockID).NeedRarity(); ;
        efficientTool = BlockManager.I.GetBlockData(blockID).EfficientTool();
        int rearity = (needRearity == 0) ? 0 : needRearity - 1;
        toolIcon.UpdateUI(efficientTool, (EnumGameData.ItemID)rearity);

        destructionTime = PlayerManager.I.playerAgainstBlocks.destructionTime;
        destructionTimeUI.value = 0;

        InfoText();

        //LoadDropItemData();
    }

    private void LoadDropItemData()
    {
        if(!BlockDropItem.LoadData.CheckExistsFile((EnumGameData.BlockID)blockID))
            return;

        var data = JsonUtility.FromJson<BlockDropItemData>(BlockDropItem.LoadData.Load((EnumGameData.BlockID)blockID));
        if(data.dropItem == null)
            return;
        
        dropItem = new BlockDropItem.DropItem[data.dropItem.Length];
        dropItem = data.dropItem;

        for(int i = 0; i < data.dropItem.Length; i++)
        {
            dropItem[i].entries = new BlockDropItem.ItemEntry[data.dropItem[i].entries.Length];
            dropItem[i].entries = data.dropItem[i].entries;
            dropItem[i].matchToolEntry = new BlockDropItem.MatchToolItemEntry[data.dropItem[i].entries.Length];
            dropItem[i].matchToolEntry = data.dropItem[i].matchToolEntry;
        }

        foreach (var icon in dropBlockIcon)
            icon.enabled = false;
        foreach (var icon in dropItemIcon)
            icon.enabled = false;

        foreach(var item in dropItem)
        {
            if(item.entries == null)
                return;

            int x = 0;
            foreach (var drop in item.entries)
            {
                var type = ItemManager.I.TypeFromID((EnumGameData.ItemID)drop.dropItemID);
                if(type == EnumGameData.ItemType.block)
                {
                    dropBlockIcon[x].UpdateUI(ItemManager.I.BlockIDFromItemID((EnumGameData.ItemID)drop.dropItemID));
                    dropBlockIcon[x].enabled = true;
                }
                else
                {
                    dropItemIcon[x].UpdateUI(type, (EnumGameData.ItemID)drop.dropItemID);
                    dropItemIcon[x].enabled = true;
                }
                x++;
                if (x == 4)
                    break;
            }
        }
    }

    private void UpdateBlockInfo()
    {
        destructionTimeUI.value = PlayerManager.I.playerAgainstBlocks.miningTime / destructionTime;
    }

    public void DoUpdate()
    {
        infoCanvas.gameObject.SetActive(true);
        if (!PlayerManager.I.highlightBlock.gameObject.activeSelf)
        {
            HideBlockInfo();
            return;
        }

        if (PlayerManager.I.highlightBlock.transform.position != previousHighlightBlockPos)
            InitBlockInfo();
        else 
            UpdateBlockInfo();

        CheckChengeItem();
    }

    private void CheckChengeItem()
    {
        if (!PlayerManager.I.toolBar.CheckHaveItem())
        {
            previousItem = EnumGameData.ItemID.None;
        }
        else if (previousItem == PlayerManager.I.toolBar.CheckHaveItemID()
            && previousSlotIndex == PlayerManager.I.toolBar.CheckSlotIndex())
        {
            return;
        }
        else
        {
            previousSlotIndex = PlayerManager.I.toolBar.CheckSlotIndex();
            previousItem = PlayerManager.I.toolBar.CheckHaveItemID();
        }
        destructionTime = PlayerManager.I.playerAgainstBlocks.destructionTime;
        InfoText();
    }

    private void InfoText()
    {
        blockNameTex.text = BlockManager.I.GetBlockData(blockID).ID().ToString();
        destructionTimeTex.text = destructionTime.ToString();
    }

    public void HideBlockInfo()
    {
        infoCanvas.gameObject.SetActive(false);
    }
}
