using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : Singleton<BlockManager>
{
    private IBaseBlock[] blockTypes;
    public int allBlockNum { get; private set; }

    public BlockAir air = new BlockAir();
    public BlockBedrock bedrock = new BlockBedrock();
    public BlockDirt dirt = new BlockDirt();
    public BlockGrass grass = new BlockGrass();
    public BlockCoarseDirt coarseDirt = new BlockCoarseDirt();
    public BlockSnow snow = new BlockSnow();
    public BlockSnowDirt snowDirt = new BlockSnowDirt();
    public BlockSand sand = new BlockSand();
    public BlockRedSand redSand = new BlockRedSand();
    public BlockGravel gravel = new BlockGravel();
    public BlockStone stone = new BlockStone();
    public BlockCobble cobble = new BlockCobble();
    public BlockDeepslate deepslate = new BlockDeepslate();
    public BlockCobbleDeepslate cobbleDeepslate = new BlockCobbleDeepslate();
    public BlockAndesite andesite = new BlockAndesite();
    public BlockGranite granite = new BlockGranite();
    public BlockDiorite diorite = new BlockDiorite();
    public BlockCoalOre coalOre = new BlockCoalOre();
    public BlockIronOre ironOre = new BlockIronOre();
    public BlockGoldOre goldOre = new BlockGoldOre();
    public BlockDiamondOre diamondOre = new BlockDiamondOre();
    public BlockRedstoneOre redstoneOre = new BlockRedstoneOre();
    public BlockCopperOre copperOre = new BlockCopperOre();
    public BlockEmeraldOre emeraldOre = new BlockEmeraldOre();
    public BlockAncientDebris ancientDebris = new BlockAncientDebris();
    public BlockCoalBlock coalBlock = new BlockCoalBlock();
    public BlockIronBlock ironBlock = new BlockIronBlock();
    public BlockGoldBlock goldBlock = new BlockGoldBlock();
    public BlockDiamondBlock diamondBlock = new BlockDiamondBlock();
    public BlockRedstoneBlock redstoneBlock = new BlockRedstoneBlock();
    public BlockCopperBlock copperBlock = new BlockCopperBlock();
    public BlockEmeraldBlock emeraldBlock = new BlockEmeraldBlock();
    public BlockNetheriteBlock netheriteBlock = new BlockNetheriteBlock();
    public BlockOakLog oakLog = new BlockOakLog();
    public BlockOakLeaf oakLeaf = new BlockOakLeaf();
    public BlockGlass glass = new BlockGlass();

    private void Awake()
    {
        Init();
        InitBlockSetting();
    }

    public int MaxAmount(EnumGameData.BlockID blockID)
    {
        return 64;
    }

    private void InitBlockSetting()
    {
        blockTypes = new IBaseBlock[]
        {
            air,
            bedrock,
            dirt,
            grass,
            coarseDirt,
            snow,
            snowDirt,
            sand,
            redSand,
            gravel,
            stone,
            cobble,
            deepslate,
            cobbleDeepslate,
            andesite,
            granite,
            diorite,
            coalOre,
            ironOre,
            goldOre,
            diamondOre,
            redstoneOre,
            copperOre,
            emeraldOre,
            ancientDebris,
            coalBlock,
            ironBlock,
            goldBlock,
            diamondBlock,
            redstoneBlock,
            copperBlock,
            emeraldBlock,
            netheriteBlock,
            oakLog,
            oakLeaf,
            glass
        };

        foreach (var block in blockTypes) 
        {
            block.Init();
        }

        allBlockNum = blockTypes.Length;
    }

    public IBaseBlock GetBlockData(EnumGameData.BlockID blockID)
    {
        foreach (var block in blockTypes)
        {
            if(block.ID() == blockID)
                return block;
        }
        return null;
    }

    public IBaseBlock GetBlockData(int blockID)
    {
        EnumGameData.BlockID id = (EnumGameData.BlockID)blockID;
        foreach (var block in blockTypes)
        {
            if (block.ID() == id)
                return block;
        }
        return null;
    }

    public bool GetRightClickIvent(int blockID)
    {
        EnumGameData.BlockID id = (EnumGameData.BlockID)blockID;
        foreach (var block in blockTypes)
        {
            if (block.ID() == id)
                return block.CheckRightClickIvent();
        }
        return false;
    }
    public void StartRightClickIvent(int blockID)
    {
        EnumGameData.BlockID id = (EnumGameData.BlockID)blockID;
        foreach (var block in blockTypes)
        {
            if (block.ID() == id)
                block.RightClickIvent();
        }
    }

    public enum faceIndex
    {
        back,
        front,
        top,
        bottom,
        left,
        right
    }
    public enum BlockDirection
    {
        Down,
        Up,
        East,
        West,
        South,
        North
    }
}