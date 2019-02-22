using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.name == "head")
                collision.transform.parent.GetComponent<PlayerStatusControl>().CmdSetHidden();
            else
                collision.GetComponent<PlayerStatusControl>().CmdSetHidden();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.tag == "Player")
        {
            if (collision.name == "head")
                collision.transform.parent.GetComponent<PlayerStatusControl>().CmdSetUnHidden();
            else
                collision.GetComponent<PlayerStatusControl>().CmdSetUnHidden();
        }
    }

}
