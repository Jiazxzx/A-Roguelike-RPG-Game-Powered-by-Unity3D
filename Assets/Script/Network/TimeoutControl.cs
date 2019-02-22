using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TimeoutControl : MonoBehaviour {

    public bool IsSearching;
    public float TimeOutAmount = 15f;

    private float TimeOutTimer;
    private bool IsTimeOut;

    private void Start()
    {
        TimeOutTimer = TimeOutAmount;
        IsSearching = false;
        IsTimeOut = false;

    }

    // Update is called once per frame
    void Update () {

        if (IsSearching)
        {
            TimeOutTimer -= Time.deltaTime;
            if(TimeOutTimer < 0)
            {
                IsTimeOut = true;
            }
        }

        if (IsTimeOut)
        {
            GetComponent<MyNetManager>().SearchTimeout();
        }

	}

}
