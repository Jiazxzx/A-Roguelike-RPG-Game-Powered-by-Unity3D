using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {
    public float RotationSpeed = 15f;
    ETCJoystick rightJoystick;
    // Use this for initialization
    void Start () {
        rightJoystick = ETCInput.GetControlJoystick("rightJoystick");

    }
	
	// Update is called once per frame
	void Update () {
        //得到摇杆的向量
        float rightValueX = rightJoystick.axisX.axisValue;
        float rightValueY = rightJoystick.axisY.axisValue;
        if(Mathf.Abs(rightValueX)>0.2 || Mathf.Abs(rightValueY) > 0.2)
        { 
            Vector2 direction = new Vector2(rightValueX, rightValueY);
            //计算角度（tan = y/x）
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 270;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //根据角度进行旋转
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
        }
    }
}
