using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ConfigManager configManager = new ConfigManager();

    [SerializeField] private PlayerManager player;
    [SerializeField] private WorldManager world;
    [SerializeField] private DebugScreenManager debugScreen;
    [SerializeField] private ItemManager item;
    [SerializeField] private BlockManager block;
    [SerializeField] private DropItemManager dropItem;
    [SerializeField] private GenerateMap generateMap;
    [SerializeField] private TitleManager titleManager;
    [SerializeField] private Inventory inventory;

    KeyConfig keyConfig = new KeyConfig();
    public bool successStart { get; private set; } = false;
    public bool successUpdate { get; private set; } = false;
    public bool successFixedUpdate { get; private set; } = false;

    [SerializeField][ReadOnly] private GameScene gameScene;

    public GameScene GetGameScene()
    {
        return gameScene;
    }

    public void SetGameScene(GameScene gameScene)
    {
        this.gameScene = gameScene;
    }

    public enum GameScene
    {
        Title,
        Option,
        MainGame
    }

    private void Awake()
    {
        Init();
        gameScene = GameScene.Title;
        keyConfig.DoAwake();
        StartCoroutine(DoInit());
    }

    private void Start()
    {
        titleManager.DoStart();
        StartCoroutine(DoStart());
    }

    private void Update()
    {
        if (KeyConfig.GetKeyDown(KeyConfig.KeyName.GameEnd))
            EndGame();

        switch (gameScene)
        {
            case GameScene.Title:
                titleManager.DoUpdate();
                break;

            case GameScene.Option:
                break;

            case GameScene.MainGame:
                if (successUpdate)
                {
                    world.DoUpdate();
                    player.DoUpdate();


                    if (KeyConfig.GetKeyDown(KeyConfig.KeyName.DebugScreen))
                        debugScreen.gameObject.SetActive(!debugScreen.gameObject.activeSelf);
                    if (debugScreen.gameObject.activeSelf)
                        debugScreen.DoUpdate();
                }
                else
                {
                    titleManager.LoadCnt();
                    StartCoroutine(DoUpdate());
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (gameScene)
        {
            case GameScene.Title:
                break;

            case GameScene.Option:

                break;

            case GameScene.MainGame:
                if (successFixedUpdate)
                {
                    world.DoFixedUpdate();
                    player.DoFixedUpDate();
                }
                else
                {
                    StartCoroutine(DoFixedUpdate());
                }
                break;
        }
    }

    IEnumerator DoInit()
    {
        yield return new WaitUntil(() => gameScene == GameScene.MainGame);
        configManager.DoAwake();
        player.DoAwake();
        debugScreen.DoAwake();
        debugScreen.gameObject.SetActive(false);

        generateMap.DoAwake();

        StartCoroutine(DoAwake());
        yield break;
    }

    IEnumerator DoAwake()
    {
        yield return new WaitUntil(() => generateMap.successGenerateMap);
        world.DoAwake();

        yield break;
    }

    IEnumerator DoStart()
    {
        yield return new WaitUntil(() => world.successGenerateWorld);
        successStart = true;

        world.DoStart();
        player.DoStart();

        yield break;
    }
    IEnumerator DoUpdate()
    {
        yield return new WaitUntil(() => successStart);
        successUpdate = true;

        yield break;
    }

    IEnumerator DoFixedUpdate()
    {
        yield return new WaitUntil(() => successUpdate);
        successFixedUpdate = true;

        yield break;
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
}

