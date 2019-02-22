using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightJoystick : MonoBehaviour
{

    private ETCJoystick myJoy;
    public Camera playerCamera;
    PlayerMovementControl playerScript;
    public Vector3 direction;
    void Start()
    {
        var temp = this.gameObject;
        playerScript = temp.GetComponent<PlayerMovementControl>();
        playerCamera = temp.GetComponentInChildren<Camera>();
        myJoy = ETCInput.GetControlJoystick("rightJoystick");

    }


    void Update()
    {

        //获取摇杆水平轴的值

        float h = myJoy.axisX.axisValue;

        //获取摇杆垂直轴的值

        float v = myJoy.axisY.axisValue;


        //获取摇杆的方向
        direction = new Vector3(h, v, 0);
            
        direction = playerCamera.transform.TransformDirection(direction);
        direction.z = 0;
        //Vector3 rotate = new Vector3(direction.x, direction.y);
        //playerScript.Rotate(rotate);



    }
}
