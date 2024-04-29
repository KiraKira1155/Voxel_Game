using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVision
{
    private float HorizontalSpeed;
    private float VerticalSpeed;
    private GameObject cam;

    public void DoAwake()
    {
        HorizontalSpeed = ConfigManager.HorizontalSpeed + 1;
        VerticalSpeed = ConfigManager.VerticalSpeed + 1;
        cam = PlayerManager.I.cam;
    }

    public void DoUpdate()
    {
        float mx = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * HorizontalSpeed;
        float my = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * VerticalSpeed;
        PlayerManager.I.transform.Rotate(Vector3.up * mx);
        cam.transform.Rotate(Vector3.right * -my);
    }
}
