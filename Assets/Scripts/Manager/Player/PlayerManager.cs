using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] BlockInfoManager blockInfoManager;
    private PlayerVision playerVision = new PlayerVision();
    private PlayerMove playerMove = new PlayerMove();
    public PlayerAgainstBlocks playerAgainstBlocks { get; private set; } = new PlayerAgainstBlocks();
    public PlayerToolBar toolBar { get; private set; } = new PlayerToolBar();

    [SerializeField] private GameObject _cam;
    public GameObject cam { get { return _cam; } private set { _cam = value; } }
    [SerializeField] private GameObject _highlightBlock;
    public GameObject highlightBlock { get { return _highlightBlock; } private set { _highlightBlock = value; } }
    [SerializeField] private GameObject _placeBlock;
    public GameObject placeBlock { get { return _placeBlock; } private set { _placeBlock = value; } }

    [SerializeField] private Slot slot;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject crosshair;
    public GameObject playerBodyUpper;
    public GameObject playerBodyLower;

    private bool _inUI = false;

    [SerializeField] private DragAndDropHandlar dragAndDropHandlar;

    [SerializeField] private GameObject inventoryWindow;
    [SerializeField] private GameObject cursorSlot;

    public PlayerDirection playerDirection;
    public PlayerAgainstBlocks.CursorFaceDirection cursorFaceDirection;
    [SerializeField] private float viewingAngle;
    public enum PlayerDirection
    {
        north,
        east,
        west,
        south
    }

    private void Awake()
    {
        Init();
    }

    public void DoAwake()
    {
        playerVision.DoAwake();
        slot.DoAwake();
        toolBar.DoAwake(slot);
        dragAndDropHandlar.DoAwake();
        inventory.DoAwake(slot);

        blockInfoManager.DoAwake();
    }

    public void DoStart()
    {
        playerAgainstBlocks.DoStart();
        slot.DoStart();
        toolBar.DoStart();
        dragAndDropHandlar.DoStart();

        inventory.DoStart();
        inventoryWindow.SetActive(false);
        cursorSlot.SetActive(false);
    }

    public void DoUpdate()
    {
        PlayerPos();

        if (KeyConfig.GetKeyDown(KeyConfig.KeyName.Inventory))
        {
            inUI = !inUI;
            if (!inUI)
            {
                dragAndDropHandlar.ChangeToInactive();
            }
        }

        if (inUI)
        {
            dragAndDropHandlar.DoUpdate();
            crosshair.SetActive(false);
            BlockInfoManager.I.HideBlockInfo();
        }
        else
        {
            playerVision.DoUpdate();
            ViewingAngle();
            playerMove.DoUpdate();
            playerAgainstBlocks.DoUpdate();
            BlockInfoManager.I.DoUpdate();
            crosshair.SetActive(true);
        }
        toolBar.DoUpdate();
    }
    public void DoFixedUpDate()
    {
        playerMove.DoFixedUpDate();
        DropItemManager.I.DoFixedUpDate();
    }

    public bool inUI
    {
        get { return _inUI; } 
        set 
        {
            _inUI = value;
            if (_inUI)
            {
                Cursor.lockState = CursorLockMode.None;
                inventoryWindow.SetActive(true);
                cursorSlot.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                inventoryWindow.SetActive(false);
                cursorSlot.SetActive(false);
            }
        }
    }

    private void PlayerPos()
    {
        playerBodyUpper.transform.position = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 1.5f, Mathf.FloorToInt(transform.position.z) + 0.5f);
        playerBodyLower.transform.position = new Vector3(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f, Mathf.FloorToInt(transform.position.z) + 0.5f);
    }

    private void ViewingDirection()
    {
        if (viewingAngle >= 0 && viewingAngle < 45 ||
            viewingAngle >= 315 &&  viewingAngle < 360)
            playerDirection = PlayerDirection.north;

        if(viewingAngle >= 45 &&  viewingAngle < 135)
            playerDirection = PlayerDirection.west;

        if(viewingAngle >= 135 && viewingAngle < 225)
            playerDirection = PlayerDirection.south;

        if(viewingAngle >= 225 &&  viewingAngle < 315)
            playerDirection = PlayerDirection.east;
    }

    private void ViewingAngle()
    {
        viewingAngle = transform.rotation.eulerAngles.y;

        ViewingDirection();
    }
}
