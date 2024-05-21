using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMonoBehaviour : Singleton<MyMonoBehaviour>
{
    public ConfigManager configManager;

    [SerializeField] private PlayerManager player;
    [SerializeField] private World world;
    [SerializeField] private DebugScreen debugScreen;
    [SerializeField] private ItemManager item;
    [SerializeField] private BlockManager block;
    [SerializeField] private DropItemManager dropItem;
    [SerializeField] private GenerateMap generateMap;

    KeyConfig keyConfig = new KeyConfig();

    private bool init = false;
    private bool successStart = false;
    private bool successUpdate = false;
    private bool successFixedUpdate = false;

    private void Awake()
    {
        Init();
        if (!init)
        {
            init = true;
            configManager.DoAwake();
            keyConfig.DoAwake();

            player.DoAwake();
            debugScreen.DoAwake();
            debugScreen.gameObject.SetActive(false);

            generateMap.DoAwake();

            StartCoroutine(DoAwake());
        }
    }

    private void Start()
    {
        StartCoroutine(DoStart());
    }

    private void Update()
    {
        if (successUpdate)
        {
            world.DoUpdate();
            player.DoUpdate();


            if (KeyConfig.GetKeyUp(KeyConfig.KeyName.DebugScreen))
                debugScreen.gameObject.SetActive(!debugScreen.gameObject.activeSelf);
            if (debugScreen.gameObject.activeSelf)
                debugScreen.DoUpdate();
        }
        else
        {
            StartCoroutine(DoUpdate());
        }
    }

    private void FixedUpdate()
    {
        if (successFixedUpdate)
        {
            player.DoFixedUpDate();
        }
        else
        {
            StartCoroutine(DoFixedUpdate());
        }
    }

    IEnumerator DoAwake()
    {
        yield return new WaitUntil(() => generateMap.successGenerateMap);
        world.DoAwake();
    }

    IEnumerator DoStart()
    {
        yield return new WaitUntil(() => world.successGenerateWorld);
        successStart = true;

        world.DoStart();
        player.DoStart();
    }
    IEnumerator DoUpdate()
    {
        yield return new WaitUntil(() => successStart);
        successUpdate = true;
    }

    IEnumerator DoFixedUpdate()
    {
        yield return new WaitUntil(() => successUpdate);
        successFixedUpdate = true;
    }
}
