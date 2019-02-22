using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : MonoBehaviour {

	public Sprite toilet_closed;
	public Sprite toilet_open;

    public bool hasItem;
	public GameObject obj;

    private bool IsOpen;

    public void interact()
    {

        if (IsOpen)
        {
            transform.GetComponent<SpriteRenderer>().sprite = toilet_closed;
            IsOpen = false;
        }
        else
        {
            transform.GetComponent<SpriteRenderer>().sprite = toilet_open;
            IsOpen = true;
        }
    }


}
