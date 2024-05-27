using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : Singleton<BlockManager>
{
    public BlockType[] blocktype;

    private void Awake()
    {
        if (!init)
            Init();
    }

    public int MaxAmount(EnumGameData.BlockID blockID)
    {
        return blocktype[(int)blockID].stackMaxSize;
    }
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
    [Tooltip("採取に対応したツールが必要か")]
    [SerializeField] private bool _needTool;
    [Tooltip("これがないと採取できない、もしくは別アイテムが落ちる")]
    [SerializeField] private bool _perfectMiner;
    [SearchableEnum][SerializeField] EnumGameData.ItemType _efficientTool;
    public DropItemAttribute[] dropItem;

    public string name { get { return _name; } }
    public bool isSolid { get { return _isSolid; } }
    public bool isTransparent { get { return _isTransparent; } }
    public int stackMaxSize { get { return _stackMaxSize; } }
    public float destructionTime { get { return _destructionTime; } }
    public int needRarity {  get { return _needRarity; } }
    public bool needTool { get { return _needTool; } }
    public bool perfectMiner { get { return _perfectMiner; } }
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

[System.Serializable]
public class DropItemAttribute
{
    [SearchableEnum][SerializeField] private EnumGameData.ItemID _itemID;
    [Range(0.01f, 1.0f)][SerializeField] private float _probability;
    [SerializeField] private int dropMinNum;
    [SerializeField] private int dropMaxNum;
}
