using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyConfig
{
    public static Dictionary<KeyName, KeyCode> Config = new Dictionary<KeyName, KeyCode>();

    public enum KeyName
    {
        Jump,
        RightMove,
        LeftMove,
        FrontMove,
        BackMove,
        Run,
        Squat,

        RightClick,
        LeftClick,

        DebugScreen,

        Inventory
    }

    public void DoAwake()
    {
        Config[KeyName.Jump] = ConfigManager.Jump;
        Config[KeyName.RightMove] = ConfigManager.RightMove;
        Config[KeyName.LeftMove] = ConfigManager.LeftMove;
        Config[KeyName.FrontMove] = ConfigManager.FrontMove;
        Config[KeyName.BackMove] = ConfigManager.BackMove;
        Config[KeyName.Run] = ConfigManager.Run;
        Config[KeyName.Squat] = ConfigManager.Squat;

        Config[KeyName.RightClick] = ConfigManager.RightClick;
        Config[KeyName.LeftClick] = ConfigManager.LeftClick;

        Config[KeyName.DebugScreen] = ConfigManager.DebugScreen;

        Config[KeyName.Inventory] = ConfigManager.Inventory;
    }

    public void DoUpdate()
    {
        DownKeyCheck();
    }

    public static bool GetKey(KeyName key)
    {
        return Input.GetKey(Config[key]);
    }
    public static bool GetKeyUp(KeyName key)
    {
        return Input.GetKeyUp(Config[key]);
    }
    public static bool GetKeyDown(KeyName key)
    {
        return Input.GetKeyDown(Config[key]);
    }
    private void DownKeyCheck()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    Debug.Log(code);
                    break;
                }
            }
        }
    }
}
