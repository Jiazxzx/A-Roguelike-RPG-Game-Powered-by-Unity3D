using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMoving : MonoBehaviour {

    private float timer = 0f;
    private int intFlag = 1;
    private float speed = 0.1f;


    // Update is called once per frame
    void Update()
    {

        //上下浮动
        transform.Translate(new Vector3(0, intFlag, 0) * Time.deltaTime * speed);

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            intFlag *= -1;
            timer = 1f;
        }

    }
}
