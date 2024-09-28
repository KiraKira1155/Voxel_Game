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

    //private void SampleDropItem()
    //{

    //    dropItemData.dropItem = new BlockDropItem.DropItem[2];
    //    dropItemData.dropItem[0].entries = new BlockDropItem.ItemEntry[2];
    //    dropItemData.dropItem[0].matchToolEntry = new BlockDropItem.MatchToolItemEntry[1];
    //    dropItemData.dropItem[0].roll.min = 1;
    //    dropItemData.dropItem[0].roll.max = 1;
    //    dropItemData.dropItem[0].entries[0].dropItemID = EnumGameData.ItemID.dirt;
    //    dropItemData.dropItem[0].entries[0].dropMinNum = 1;
    //    dropItemData.dropItem[0].entries[0].dropMaxNum = 1;

    //    BlockDropItem.LoadData.Save(dropItemData, EnumGameData.BlockID.dirt);
    //}
}

[System.Serializable]
public class BlockType
{
    [SerializeField] private string _name;
    [SearchableEnum][SerializeField] private EnumGameData.BlockType blockType;
    [SerializeField] private bool _gravity;
    [SerializeField] private bool _isSolid;
    [SerializeField] private bool _isTransparent;
    [SerializeField] private int _stackMaxSize;

    [Tooltip("レアリティが同じ対応ツール使用時の破壊時間" +
        "\nneedRarityが0の場合はレアリティが1の対応ツール使用時")]
    [Range(0.05f, 100)][SerializeField] private float _destructionTime;
    [Tooltip("-1は、破壊不可")]
    [Range(-1, 10)][SerializeField] private int _needRarity;
    [Tooltip("適正ツール")]
    [SearchableEnum][SerializeField] EnumGameData.ItemType _efficientTool;

    public string name { get { return _name; } }
    public bool isSolid { get { return _isSolid; } }
    public bool isTransparent { get { return _isTransparent; } }
    public int stackMaxSize { get { return _stackMaxSize; } }
    public float destructionTime { get { return _destructionTime; } }
    public int needRarity {  get { return _needRarity; } }
    public EnumGameData.ItemType efficientTool { get { return _efficientTool; } }
    public EnumGameData.ItemKinds kinds { get; private set; } = EnumGameData.ItemKinds.blockItem;

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    // Back, Front, Top, Bottom, Left, Right

    public enum faceIndex
    {
        back,
        front,
        top,
        bottom,
        left,
        right
    }

    public EnumGameData.BlockID GetTextureFace(faceIndex faceIndex)
    {
        switch (faceIndex)
        {
            case faceIndex.back:
                return (EnumGameData.BlockID)backFaceTexture;
            case faceIndex.front:
                return (EnumGameData.BlockID)frontFaceTexture;
            case faceIndex.top:
                return (EnumGameData.BlockID)topFaceTexture;
            case faceIndex.bottom:
                return (EnumGameData.BlockID)bottomFaceTexture;
            case faceIndex.left:
                return (EnumGameData.BlockID)leftFaceTexture;
            case faceIndex.right:
                return (EnumGameData.BlockID)rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }
}
