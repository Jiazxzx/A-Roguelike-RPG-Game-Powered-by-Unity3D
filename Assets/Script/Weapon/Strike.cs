using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strike : MonoBehaviour
{
    bool can = false;
    protected Transform m_transform;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (can)
            Debug.Log("jinll");
        else
            Debug.Log("meijinll");


        if (can)
        {
            Debug.Log("kanren");

            if (other.tag == "Item")
            {
                other.gameObject.GetComponent<EnemyStatusControl>().damageEnmey(1);
            }
            if (other.tag == "monster")
            {
                other.gameObject.GetComponent<EnemyStatusControl>().damageEnmey(1);
            }
        }


    }


    // Use this for initialization
    void Start()
    {
        m_transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            can = true;
            GetComponent<PolygonCollider2D>().enabled = true;
            //Debug.Log("按下T");
        }
        if (Input.GetMouseButtonUp(0))
        {
            can = false;
            GetComponent<PolygonCollider2D>().enabled = false;
            //Debug.Log("松开T");
        }
    }
}
