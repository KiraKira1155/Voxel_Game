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

    KeyConfig keyConfig = new KeyConfig();

    private bool init = false;

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
        }
    }

    private void Start()
    {
        world.DoStart();
        player.DoStart();
    }

    private void Update()
    {
        world.DoUpdate();
        player.DoUpdate();


        if (KeyConfig.GetKeyUp(KeyConfig.KeyName.DebugScreen))
            debugScreen.gameObject.SetActive(!debugScreen.gameObject.activeSelf);
        if (debugScreen.gameObject.activeSelf)
            debugScreen.DoUpdate();
    }

    private void FixedUpdate()
    {
        player.DoFixedUpDate();
    }
}
