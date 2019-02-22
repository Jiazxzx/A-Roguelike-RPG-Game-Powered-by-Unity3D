using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AmmoUI : MonoBehaviour {

    public GameObject player;
    private int ammo;
    private string ammoShow;

	// Use this for initialization
	void Start () {

        player = transform.parent.parent.GetComponent<GameUI>().getCurrentPlayer();
        ammo = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (player == null)
        {
            player = transform.parent.parent.GetComponent<GameUI>().getCurrentPlayer();
        }
        else
        {

            ammo = player.GetComponent<PlayerStatusControl>().getCurrentAmmo();
            ammoShow = string.Format("{0:00}", ammo);
            transform.Find("AmmoAmount").GetComponent<Text>().text = ammoShow;
        }
    }
}
