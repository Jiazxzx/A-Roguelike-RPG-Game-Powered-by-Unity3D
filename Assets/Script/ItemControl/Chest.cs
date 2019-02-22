using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Chest : Item
{
    public GameObject[] ItemsInside;
    public GameObject HitObject;

    [Command]
    protected void CmdPopItem(GameObject _other)
    {
        HitObject = _other;
        Vector3 HitPivot = HitObject.transform.position;
        Vector3 force = transform.position - HitPivot;

        //var tempChildren = temp.GetComponentsInChildren<Rigidbody2D>();
        foreach (var item in ItemsInside)
        {
            var temp = (GameObject)Instantiate(item, transform.position, transform.rotation);
            Vector3 randomDirection = force + new Vector3(force.x * Random.Range(-30, 30), force.y * Random.Range(-30, 30));
            temp.GetComponent<Rigidbody2D>().velocity = randomDirection;
            NetworkServer.Spawn(temp);
            //Debug.Log("产生书本");
            //Debug.Log(temp.name);
        }

    }


    //[Command]
    //public void CmdPopItem()
    //{
    //    //Vector3 BasePos = _other.transform.position;
    //    foreach (GameObject i in ItemsInside)
    //    {
    //        //i.transform.position = new Vector3(BasePos.x + Random.Range(-2, 2), BasePos.y + Random.Range(-2, 2), BasePos.z + Random.Range(-2, 2));
    //        var item = Instantiate(i, transform.position, transform.rotation);
    //        item.GetComponent<Rigidbody2D>().AddForce(Vector2.up * item.GetComponent<Rigidbody2D>().mass * 300);
    //        NetworkServer.Spawn(item);
    //    }
    //}


}
