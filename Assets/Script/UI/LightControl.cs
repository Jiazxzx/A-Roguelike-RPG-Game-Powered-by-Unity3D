using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour {

    private float timer = 0.1f;
    private float timer_2 = 0;
    private int intFlag;
    private int intFlag_2 = 1;
    //private float speed = 0.1f;

    // Update is called once per frame
    void Update()
    {

        timer -= Time.deltaTime;


        if (timer <= 0)
        {
            intFlag = Random.Range(-1, 7);
            if(intFlag >= 0)
            {
                this.GetComponent<Light>().intensity = Random.Range(13, 15);
            }
            else
            {
                this.GetComponent<Light>().intensity = 0;
            }
            timer = 0.1f;
        }


        transform.Rotate(Vector3.up, intFlag_2 * 0.1f * Mathf.Cos(Time.realtimeSinceStartup));


    }
}
