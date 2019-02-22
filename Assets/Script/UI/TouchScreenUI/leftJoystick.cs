using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leftJoystick : MonoBehaviour
{
    //定义移动速度
    float speed = 60f;
    //定义摇杆组件
    private ETCJoystick myJoy;
    private Camera playerCamera;
    public Vector3 direction;
    PlayerMovementControl playerScript;

    void Start()
    {
        var temp = this.gameObject;
        playerScript = temp.GetComponent<PlayerMovementControl>();
        playerCamera = temp.GetComponentInChildren<Camera>();
        //获取指定摇杆，ETCInput.GetControlJoystick（摇杆名字）;

        //注意获取指定摇杆的方法，不要放在Awake里面，必须在Start里面，

        //因为放在Awake里面的时候会报null的错，可能跟它底部逻辑顺序有关吧！

        myJoy = ETCInput.GetControlJoystick("leftJoystick");

    }


    void Update()
    {

        //获取摇杆水平轴的值

        float h = myJoy.axisX.axisValue;

        //获取摇杆垂直轴的值

        float v = myJoy.axisY.axisValue;


        //获取摇杆的方向

        direction = new Vector3(h, v, 0);

        //把方向转换到相机的坐标系中

        direction = playerCamera.transform.TransformDirection(direction);
        //dir = Camera.main.transform.TransformDirection(dir);

        //方向的z轴值始终设为0

        direction.z = 0;

        //把方向向量归一化

        direction.Normalize();

        //x10倍

        //dir = dir * 1;

        ////控制模型的朝向

        //this.transform.LookAt(sp + this.transform.forward);

        //用角色控制器控制移动
        //Vector3 movement = new Vector3(dir.x,dir.y);

        ////Debug.Log(movement* speed * Time.deltaTime);
        //playerScript.Move(movement* speed * Time.deltaTime);
            
        //else
        //    playerScript.Stay();

    }

}
