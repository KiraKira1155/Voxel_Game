using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfigManager
{
    [SerializeField] public int setSeed;
    [ReadOnly] private float _seed;
    [SerializeField] private bool creative;

    public static bool successGenerateMap = false;
    public static bool successGenerateWorld = false;
    public static int thisWorldSeed { get; private set; }
    public static float seed { get; private set; }
    public static short WorldSizeInChunks { get; private set; } = 8192;
    public static byte ViewDistanceInChunk { get; private set; } = 15;

    public static int HorizontalSpeed { get; private set; }
    public static int VerticalSpeed { get; private set; }
    [SerializeField] private int setHorizontalSpeed;
    [SerializeField] private int setVerticalSpeed;

    public static KeyCode Jump { get; private set; } = KeyCode.Space;
    public static KeyCode RightMove { get; private set; } = KeyCode.D;
    public static KeyCode LeftMove { get; private set; } = KeyCode.A;
    public static KeyCode FrontMove { get; private set; } = KeyCode.W;
    public static KeyCode BackMove { get; private set; } = KeyCode.S;
    public static KeyCode Squat { get; private set; } = KeyCode.LeftShift;
    public static KeyCode Run { get; private set; } = KeyCode.LeftControl;
    public static KeyCode DebugScreen { get; private set; } = KeyCode.F3;
    public static KeyCode RightClick { get; private set; } = KeyCode.Mouse0;
    public static KeyCode LeftClick { get; private set; } = KeyCode.Mouse1;
    public static KeyCode Inventory { get; private set; } = KeyCode.E;

    public void DoAwake()
    {
        Random();
        HorizontalSpeed = setHorizontalSpeed;
        VerticalSpeed = setVerticalSpeed;
        if (creative)
            PlayerStatus.CreativeMode();
        else
            PlayerStatus.SurvivalMode();
    }

    private void Random()
    {
        System.Random rnd;
        if (setSeed == 0)
        {
            int timeSeed = DateTime.Now.Millisecond + DateTime.Now.Second * 1000 + DateTime.Now.Minute * 100000;
            rnd = new System.Random(timeSeed);
            setSeed = rnd.Next(1, 2147483647);
        }
        else
        {
            rnd = new System.Random(setSeed);
        }
        thisWorldSeed = setSeed;
        float s = rnd.Next(10000, 1000000);
        seed = s / 100000;
        _seed = seed;
    }
}
