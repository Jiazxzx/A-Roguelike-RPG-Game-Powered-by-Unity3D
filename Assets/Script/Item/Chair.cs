using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Chair : Chest
{
    //public GameObject[] remains;
    //GameObject temp;

    // Use this for initialization
    void Start()
    {
        state = true;
    }

    public override void interact(GameObject _other)
    {
        HitObject = _other;
        if (state)
        {
            state = false;
        }
    }

    public override void OnChangeState(bool state)
    {
        if (!state)
        {
            CmdPopItem(HitObject);
            Destroy(gameObject);
        }
    }

    
}
