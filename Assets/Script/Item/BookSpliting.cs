using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BookSpliting : Chest
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

    //void OnCollisionEnter2D(Collision2D col)
    //{
    //    if (col.gameObject.tag == "Player")
    //    {
    //        Debug.Log("Hit！");
    //        Vector3 playerPivot = col.gameObject.transform.position;
    //        Vector3 force = transform.position - playerPivot;

    //        CmdPopItem(force);
    //        //CmdSpliting();
    //        Destroy(gameObject);
    //    }
    //}

    //[Command]
    //void CmdPopItem(Vector3 force)
    //{

    //    //var tempChildren = temp.GetComponentsInChildren<Rigidbody2D>();
    //    foreach (var item in ItemsInside)
    //    {
    //        var temp = (GameObject)Instantiate(item, transform.position, transform.rotation);
    //        Vector3 randomDirection = force + new Vector3(force.x * Random.Range(-1, 3), force.y * Random.Range(-1, 3));
    //        temp.GetComponent<Rigidbody2D>().velocity = randomDirection * 30;
    //        NetworkServer.Spawn(temp);
    //        Debug.Log("产生书本");
    //        Debug.Log(temp.name);
    //    }

    //}

}
