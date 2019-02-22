using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private GameObject player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            player = GameObject.Find("Player(Clone)");
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

    }
}
