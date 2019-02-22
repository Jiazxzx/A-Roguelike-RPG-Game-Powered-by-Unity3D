using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Can : Chest
{
    public Sprite can_up;
    public Sprite can_down;

    // Use this for initialization
    void Start()
    {
        state = true;
    }

    public override void interact(GameObject _other)
    {
        HitObject = _other;
        if (state == true)
        {
            state = false;
        }
    }

    public override void OnChangeState(bool state)
    {
        if (state == true)
            this.GetComponent<SpriteRenderer>().sprite = can_up;
        if (state == false)
        {
            this.GetComponent<SpriteRenderer>().sprite = can_down;
            CmdPopItem(HitObject);
        }
    }

    //void OnCollisionEnter2D(Collision2D col)
    //{

    //    Debug.Log("Hit！");
    //    state = false;
    //    Vector3 playerPivot = col.gameObject.transform.position;
    //    Vector3 force = transform.position - playerPivot;
    //    CmdForcePopItem(force);
    //    //CmdSpliting();
    //}
}
