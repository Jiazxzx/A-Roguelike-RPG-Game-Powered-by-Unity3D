using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rg;
    private float originalSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.isTrigger == false)
        {

            player = collision.gameObject;
            originalSpeed = player.GetComponent<PlayerMovementControl>().Mspeed;
            player.GetComponent<PlayerMovementControl>().Mspeed = 4.5f;
            //rg = player.GetComponent<Rigidbody2D>();
            //Vector3 playerPos = player.transform.position;
            //Vector3 force = transform.position - playerPos;

            //rg.drag = 1f;
            //rg.AddForce(force*2000);

        }
        else
        {
            //collision.gameObject.GetComponent<Player>().Mspeed = 10f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.isTrigger == false)
        {
            player.GetComponent<PlayerMovementControl>().Mspeed = originalSpeed;

            //collision.gameObject.GetComponent<Rigidbody2D>().drag = 10;
        }
        else
        {
            //rg.drag = 10;
            //collision.gameObject.GetComponent<Player> ().Mspeed = 5f;
        }
    }
}
