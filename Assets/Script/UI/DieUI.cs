using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieUI : MonoBehaviour {

    private GameObject player;
    private GameObject dieText;
    private float timer = 0f;
    private int intFlag = 1;
    private float speed = 0.1f;

    private float timeAtLastFrame;
    private float timeAtCurrentFrame;
    private float realTimeDeltaTime;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player(Clone)");
        dieText = GameObject.Find("YouDieText");
        timeAtLastFrame = 0f;
    }
	
	// Update is called once per frame
	void Update () {

        timeAtCurrentFrame = Time.realtimeSinceStartup;
        realTimeDeltaTime = timeAtCurrentFrame - timeAtLastFrame;
        //上下浮动
        dieText.transform.Translate(new Vector3(0, intFlag, 0) * realTimeDeltaTime * speed);

        timer -= realTimeDeltaTime;
        if (timer <= 0)
        {
            intFlag *= -1;
            timer = 2.0f;
        }

        timeAtLastFrame = timeAtCurrentFrame;
    }
}
