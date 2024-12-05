using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
    private Vector3 velocity;
    private float verticalMomentum;
    private int horizontal;
    private int vertical;

    private bool falling;
    private int fallCal;
    private bool fallinStart;
    private float fallingStartPos;
    private float fallingEndPos;
    private bool terminalVelocity; 

    public void DoUpdate()
    {
        MoveByInputKey();
    }

    public void DoFixedUpDate()
    {
        CalculateVelocity();
        if (PlayerStatus.Jump)
            Jump();
        if (PlayerManager.I.inUI)
        {
            velocity.x = 0;
            velocity.z = 0;
        }
        PlayerManager.I.transform.Translate(velocity, Space.World);
    }

    private void CalculateVelocity()
    {
        if (verticalMomentum > PlayerStatus.Gravity)
            verticalMomentum += Time.fixedDeltaTime * PlayerStatus.Gravity;

        velocity = ((PlayerManager.I.transform.forward * vertical) + (PlayerManager.I.transform.right * horizontal)) * PlayerStatus.MoveSpeed * Time.fixedDeltaTime;
        if (PlayerStatus.isRunning)
            velocity *= PlayerStatus.RunSpeed;

        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;
        
        if (velocity.y < 0)
        {
            if (fallCal < 25)
            {
                verticalMomentum += Time.fixedDeltaTime * PlayerStatus.Gravity;
                fallCal += 1;
            }
            velocity.y = PlayerDownSpeed(velocity.y);
        }
        else if (velocity.y > 0)
            velocity.y = PlayerUpSpeed(velocity.y);

        if (!PlayerStatus.isGrounded)
        {
            if (!falling && verticalMomentum <= 0)
            {
                verticalMomentum = 0;
            }
            if(velocity.y < 0 && !fallinStart)
            {
                fallinStart = true;
                fallCal = 0;
            }
            falling = true;
            velocity.z *= PlayerStatus.AirForwardForce;
            velocity.x *= PlayerStatus.AirForwardForce;
        }
        else
        {
            if (falling)
            {
                fallinStart = false;
                //落下ダメージ判定用
                fallingEndPos = PlayerManager.I.transform.position.y;
                float fallingDistance = fallingStartPos - fallingEndPos;
                if (fallingDistance - (int)fallingDistance < 0.5f)
                {
                    fallingDistance = (int)fallingDistance;
                }
                else if (fallingDistance - (int)fallingDistance >= 0.5f)
                {
                    fallingDistance = (int)fallingDistance + 1;
                }
            }
            fallingStartPos = PlayerManager.I.transform.position.y;
            verticalMomentum = PlayerStatus.Gravity;
            terminalVelocity = false;
            falling = false;

            PlayerManager.I.transform.position = new Vector3(PlayerManager.I.transform.position.x, Mathf.FloorToInt(PlayerManager.I.transform.position.y), PlayerManager.I.transform.position.z);
        }

        if (falling && fallingStartPos > PlayerManager.I.transform.position.y && !terminalVelocity)
        {
            if (verticalMomentum <= PlayerStatus.Gravity)
            {
                verticalMomentum += Time.fixedDeltaTime * PlayerStatus.Gravity;
                if (verticalMomentum < -180)
                {
                    terminalVelocity = true;
                }
            }
        }
    }

    private void Jump()
    {
        fallingStartPos = PlayerManager.I.transform.position.y + 1.0f;
        verticalMomentum = PlayerStatus.JumpForce;
        PlayerStatus.isGrounded = false;
        PlayerStatus.Jump = false;
    }

    private void MoveByInputKey()
    {
        vertical = 0;
        horizontal = 0;
        if (KeyConfig.GetKey(KeyConfig.KeyName.FrontMove))
        {
            vertical = 1;
        }
        if (KeyConfig.GetKey(KeyConfig.KeyName.BackMove))
        {
            vertical = -1;
        }
        if (KeyConfig.GetKey(KeyConfig.KeyName.FrontMove) && KeyConfig.GetKey(KeyConfig.KeyName.BackMove))
        {
            vertical = 0;
        }

        if (KeyConfig.GetKey(KeyConfig.KeyName.RightMove))
        {
            horizontal = 1;
        }
        if (KeyConfig.GetKey(KeyConfig.KeyName.LeftMove))
        {
            horizontal = -1;
        }
        if (KeyConfig.GetKey(KeyConfig.KeyName.RightMove) && KeyConfig.GetKey(KeyConfig.KeyName.LeftMove))
        {
            horizontal = 0;
        }


        if (KeyConfig.GetKey(KeyConfig.KeyName.Run) && !PlayerStatus.isGrounded && PlayerStatus.isRunning)
        {
            PlayerStatus.isRunning = true;
        }
        else if (KeyConfig.GetKey(KeyConfig.KeyName.Run) && PlayerStatus.isGrounded)
        {
            PlayerStatus.isRunning = true;
        }
        else
        {
            PlayerStatus.isRunning = false;
        }

        if (PlayerStatus.isGrounded && KeyConfig.GetKeyDown(KeyConfig.KeyName.Jump))
        {
            PlayerStatus.Jump = true;
        }
    }

    private float PlayerDownSpeed(float downSpeed)
    {
        if (
              (WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x - PlayerStatus.Width, PlayerManager.I.transform.position.y + downSpeed, PlayerManager.I.transform.position.z - PlayerStatus.Width)) && (!left && !back)) ||
              (WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x + PlayerStatus.Width, PlayerManager.I.transform.position.y + downSpeed, PlayerManager.I.transform.position.z - PlayerStatus.Width)) && (!right && !back)) ||
              (WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x + PlayerStatus.Width, PlayerManager.I.transform.position.y + downSpeed, PlayerManager.I.transform.position.z + PlayerStatus.Width)) && (!right && !front)) ||
              (WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x - PlayerStatus.Width, PlayerManager.I.transform.position.y + downSpeed, PlayerManager.I.transform.position.z + PlayerStatus.Width)) && (!left && !front))
             )
        {
            PlayerStatus.isGrounded = true;
            return 0;
        }
        else
        {
            PlayerStatus.isGrounded = false;
            return downSpeed;
        }
    }
    private float PlayerUpSpeed(float upSpeed)
    {
        if (
              (WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x - PlayerStatus.Width, PlayerManager.I.transform.position.y + PlayerStatus.Height + upSpeed, PlayerManager.I.transform.position.z - PlayerStatus.Width)) && (!left && !back)) ||
              (WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x + PlayerStatus.Width, PlayerManager.I.transform.position.y + PlayerStatus.Height + upSpeed, PlayerManager.I.transform.position.z - PlayerStatus.Width)) && (!right && !back)) ||
              (WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x + PlayerStatus.Width, PlayerManager.I.transform.position.y + PlayerStatus.Height + upSpeed, PlayerManager.I.transform.position.z + PlayerStatus.Width)) && (!right && !front)) ||
              (WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x - PlayerStatus.Width, PlayerManager.I.transform.position.y + PlayerStatus.Height + upSpeed, PlayerManager.I.transform.position.z + PlayerStatus.Width)) && (!left && !front))
             )
        {
            verticalMomentum = 0; //プレイヤーの頭上にブロックがあるときに落下するように
            return 0;
        }
        else
        {
            return upSpeed;
        }
    }

    public bool front
    {
        get
        {
            if (
                WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x, PlayerManager.I.transform.position.y, PlayerManager.I.transform.position.z + PlayerStatus.Width)) ||
                WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x, PlayerManager.I.transform.position.y + 1f, PlayerManager.I.transform.position.z + PlayerStatus.Width))
                )
                return true;
            else 
                return false;
        }
    }
    public bool back
    {
        get
        {
            if (
                WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x, PlayerManager.I.transform.position.y, PlayerManager.I.transform.position.z - PlayerStatus.Width)) ||
                WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x, PlayerManager.I.transform.position.y + 1f, PlayerManager.I.transform.position.z - PlayerStatus.Width))
                )
                return true;
            else
                return false;
        }
    }
    public bool right
    {
        get
        {
            if (
                WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x + PlayerStatus.Width, PlayerManager.I.transform.position.y, PlayerManager.I.transform.position.z )) ||
                WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x + PlayerStatus.Width, PlayerManager.I.transform.position.y + 1f, PlayerManager.I.transform.position.z))
                )
                return true;
            else
                return false;
        }
    }
    public bool left
    {
        get
        {
            if (
                WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x - PlayerStatus.Width, PlayerManager.I.transform.position.y, PlayerManager.I.transform.position.z)) ||
                WorldManager.I.CheckForVoxel(new Vector3(PlayerManager.I.transform.position.x - PlayerStatus.Width, PlayerManager.I.transform.position.y + 1f, PlayerManager.I.transform.position.z))
                )
                return true;
            else
                return false;
        }
    }
}
