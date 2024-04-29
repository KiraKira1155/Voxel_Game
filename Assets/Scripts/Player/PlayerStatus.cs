using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatus
{
    public static bool Creative { get; private set; }
    public static int MoveSpeed { get; private set; } = 4;
    public static int RunSpeed { get; private set; } = 2;   //”{—¦
    public static float Gravity { get; private set; } = -9.8f;
    public static float JumpForce { get; private set; } = 4.7f;
    public static float AirForwardForce { get; private set; } = 0.7f;
    public static float Width { get; private set; } = 0.35f;
    public static float Height { get; private set; } = 1.8f;
    public static float Reach { get; private set; } = 6.0f;
    public static bool Jump { get; set; }
    public static bool isGrounded { get; set; }
    public static bool isRunning { get; set; }
    public static int SlotNum { get; private set; } = 9;

    public static void CreativeMode()
    {
        Creative = true;
    }
    public static void SurvivalMode()
    {
        Creative = false;
    }
}
