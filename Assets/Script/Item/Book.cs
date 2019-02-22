using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : Item {


    public Sprite bookClose;
    public Sprite bookOpen;

    private bool IsOpen;

    // Use this for initialization
    void Start()
    {
        IsOpen = false;
    }

    public override void interact(GameObject other)
    {
        state = !state;
    }

    public override void OnChangeState(bool state)
    {
        if(state)
            this.GetComponent<SpriteRenderer>().sprite = bookOpen;
        if(!state)
            this.GetComponent<SpriteRenderer>().sprite = bookClose;
    }

}
